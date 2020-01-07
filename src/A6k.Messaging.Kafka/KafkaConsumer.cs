using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Confluent.Kafka;
using A6k.Messaging.Features;
using A6k.Messaging.Kafka.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace A6k.Messaging.Kafka
{
    /// <summary>
    /// An <see cref="IMessageConsumer{TKey, TValue}"/> implemented on Kafka
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public partial class KafkaConsumer<TKey, TValue> : IMessageConsumer<TKey, TValue>
    {
        private static readonly TimeSpan consumerTimeOut = TimeSpan.FromMilliseconds(100);

        private readonly KafkaOptions options;
        private readonly ILogger<KafkaConsumer<TKey, TValue>> logger;

        private ConsumerConfig config;
        private bool autoOffsetStoreEnabled;
        private IConsumer<TKey, TValue> consumer;

        public KafkaConsumer(string configName, IOptionsMonitor<KafkaOptions> options, ILogger<KafkaConsumer<TKey, TValue>> logger) : this(options.Get(configName), logger) { }

        public KafkaConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer<TKey, TValue>> logger) : this(options.Value, logger) { }

        public KafkaConsumer(KafkaOptions options, ILogger<KafkaConsumer<TKey, TValue>> logger)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private readonly CustomFeatures features = new CustomFeatures();
        public IFeatureSet Features => features;

        public void Configure(Action<IFeatureSet> configureFeatures = null)
        {
            configureFeatures?.Invoke(Features);
            features.Config.Configure(options.Configuration);

            config = new ConsumerConfig(options.Configuration);

            // if these are not explicitly set then apply default offset tracking strategy
            if (!config.EnableAutoCommit.HasValue && !config.EnableAutoOffsetStore.HasValue)
            {
                // see https://github.com/confluentinc/confluent-kafka-dotnet/issues/527#issuecomment-386405668
                // we are responsible for calling StoreOffset <see cref="AcceptAsync"/>
                config.EnableAutoOffsetStore = false;
                // The kafka client will periodically "commit" the offsets back to the broker
                config.EnableAutoCommit = true;
                // defaults to 5seconds
                // config.AutoCommitIntervalMs = 5000;
            }
            autoOffsetStoreEnabled = config.EnableAutoOffsetStore ?? true;

            features.KafkaConfig.Configure(config);

            if (features.PartitionEof != null)
                config.EnablePartitionEof = true;

            var builder =
                new ConsumerBuilderFactory(config)
                .SetFactory(features.FactoryFeature)
                .SetDeserializers(features.SerializationFeature);
            features.BuilderFeature?.Configure(builder);

            if (features.PartitionTracking != null || features.PartitionAssignment != null)
            {
                builder
                    .SetPartitionsAssignedHandler((_, p) => PartitionsAssigned(p))
                    .SetPartitionsRevokedHandler((_, p) => PartitionsRevoked(p));
            }

            builder
                .SetErrorHandler((_, e) =>
                {
                    if (e.IsFatal)
                        logger.LogCritical(e.Reason, new { Code = (int)e.Code, options.Topic, options.GroupId });
                    else
                        logger.LogError(e.Reason, new { Code = (int)e.Code, options.Topic, options.GroupId });
                })
                .SetLogHandler((_, m) =>
                {
                    logger.Log(m.Level.GetLogLevel(), m.Message + ": {@details}", new { m.Name, m.Facility, SyslogLevel = m.Level, options.Topic, options.GroupId });
                });
            if (features.StatisticsHandling != null)
                builder.SetStatisticsHandler((c, stats) => features.StatisticsHandling.OnStatistics(stats));

            consumer = builder.Build();
            features.FeatureRequiresConsumer.SetConsumer(consumer);
            consumer.Subscribe(options.Topic);
        }

        private IEnumerable<TopicPartitionOffset> PartitionsAssigned(List<TopicPartition> partitions)
        {
            for (int i = 0; i < partitions.Count; i++)
            {
                var tp = partitions[i];
                features.PartitionTracking?.PartitionAssigned(tp.Topic, tp.Partition.Value);
                var o = features.PartitionAssignment?.PartitionAssigned(tp.Topic, tp.Partition.Value);
                var offset = o == null ? Offset.Unset : new Offset(o.Value);
                yield return new TopicPartitionOffset(tp, offset);
            }
        }

        private void PartitionsRevoked(List<TopicPartitionOffset> partitions)
        {
            for (int i = 0; i < partitions.Count; i++)
            {
                var p = partitions[i];
                features.PartitionTracking?.PartitionRevoked(p.Topic, p.Partition.Value, p.Offset.Value);
            }
        }

        /// <inheritdoc/>
        public async Task AcceptAsync(IMessage message)
        {
            if (features.OffsetTracking != null)
                await features.OffsetTracking.AcceptAsync(message);
            else if (!autoOffsetStoreEnabled)
                consumer.StoreOffset(message.AsTopicPartitionOffset());
        }

        /// <inheritdoc/>
        public async Task RejectAsync(IMessage<TKey, TValue> message)
        {
            if (features.OffsetTracking == null)
                await AcceptAsync(message);
            else
                await features.OffsetTracking.RejectAsync(message);
        }

        /// <inheritdoc/>
        public Task<IMessage<TKey, TValue>> ConsumeAsync()
        {
            var consumeResult = GetMessage();
            if (consumeResult == null)
                return Task.FromResult<IMessage<TKey, TValue>>(null);

            if (consumeResult.Message == null)
            {
                if (consumeResult.IsPartitionEOF)
                    features.PartitionEof?.PartitionIsEof(consumeResult.Topic, consumeResult.Partition.Value);

                return Task.FromResult<IMessage<TKey, TValue>>(null);
            }

            var result = new Message<TKey, TValue>
            {
                Key = consumeResult.Message.Key,
                Value = consumeResult.Message.Value,
                Timestamp = consumeResult.Message.Timestamp.UtcDateTime,
                Topic = consumeResult.Topic,
                Partition = consumeResult.Partition.Value,
                Offset = consumeResult.Offset.Value
            };

            foreach (var header in consumeResult.Message.Headers)
            {
                if (header.Key == MessageHeaders.ActivityId)
                    result.ActivityId = Encoding.UTF8.GetString(header.GetValueBytes());
                else
                    result.AddHeader(header.Key, header.GetValueBytes());
            }

            return Task.FromResult<IMessage<TKey, TValue>>(result);
        }

        private ConsumeResult<TKey, TValue> GetMessage()
        {
            try
            {
                return consumer.Consume(consumerTimeOut);
            }
            catch (Exception ex)
            {
                features.OnConsumeError?.OnConsumeError(ex);
                throw;
            }
        }

        public void Dispose() => consumer.Close();

        /// <summary>
        /// Specialised <see cref="IFeatureSet"/> for the consumer
        /// </summary>
        private class CustomFeatures : FeatureSet
        {
            public ConsumerFactoryFeature<TKey, TValue> FactoryFeature { get; private set; }

            public IConsumeErrorFeature OnConsumeError { get; private set; }

            public IConfigFeature Config { get; private set; } = new CompositeConfigFeature();

            public IKafkaConfigFeature<ConsumerConfig> KafkaConfig { get; private set; } = new CompositeKafkaConfigFeature<ConsumerConfig>();

            public IFeatureRequiresConsumer<TKey, TValue> FeatureRequiresConsumer { get; private set; } = new CompositeFeatureRequiresConsumer<TKey, TValue>();

            public IKafkaConfigBuilderFeature<ConsumerBuilder<TKey, TValue>> BuilderFeature { get; private set; }

            public IKafkaSerializationFeature SerializationFeature { get; private set; }

            public IPartitionTrackingFeature PartitionTracking { get; private set; }

            public IPartitionAssignmentFeature PartitionAssignment { get; private set; }

            public IPartitionEofFeature PartitionEof { get; private set; }

            public IOffsetTrackingFeature<TKey, TValue> OffsetTracking { get; private set; }

            public IStatisticsHandlingFeature StatisticsHandling { get; private set; }
        }

        private class ConsumerBuilderFactory : ConsumerBuilder<TKey, TValue>
        {
            public ConsumerBuilderFactory(IEnumerable<KeyValuePair<string, string>> config) : base(config) { }

            public Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> ConsumerFactory { get; set; }

            public ConsumerBuilder<TKey, TValue> SetFactory(Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> consumerFactory)
            {
                ConsumerFactory = consumerFactory;
                return this;
            }
            public ConsumerBuilder<TKey, TValue> SetFactory(ConsumerFactoryFeature<TKey, TValue> factoryFeature)
            {
                ConsumerFactory = factoryFeature?.ConsumerFactory;
                return this;
            }

            public override IConsumer<TKey, TValue> Build()
            {
                return ConsumerFactory?.Invoke(this) ?? base.Build();
            }
        }
    }
}
