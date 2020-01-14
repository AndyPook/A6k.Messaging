using Confluent.Kafka;
using A6k.Messaging.Features;
using A6k.Messaging.Kafka.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace A6k.Messaging.Kafka
{
    /// <summary>
    /// An <see cref="IMessageProducer{TKey, TValue}"/> implemented on Kafka
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KafkaProducer<TKey, TValue> : IMessageProducer<TKey, TValue>
    {
        private readonly KafkaOptions options;
        private readonly ILogger logger;
        private IProducer<TKey, TValue> producer;

        public KafkaProducer(string configName, IOptionsMonitor<KafkaOptions> options, ILogger<KafkaProducer<TKey, TValue>> logger) : this(options.Get(configName), logger) { }

        public KafkaProducer(IOptions<KafkaOptions> options, ILogger<KafkaProducer<TKey, TValue>> logger) : this(options?.Value, logger) { }

        public KafkaProducer(KafkaOptions options, ILogger<KafkaProducer<TKey, TValue>> logger)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private readonly CustomFeatures features = new CustomFeatures();
        public IFeatureSet Features => features;

        public void Configure(Action<IFeatureSet> configureFeatures = null)
        {
            configureFeatures?.Invoke(features);
            features.Config.Configure(options.Configuration);

            var config = new ProducerConfig(options.Configuration);
            features.KafkaConfig.Configure(config);

            var builder =
                new ProducerBuilderFactory(config)
                .SetFactory(features.FactoryFeature)
                .SetSerializers(features.SerializationFeature);
            features.BuilderFeature?.Configure(builder);

            builder
                .SetErrorHandler((_, e) =>
                {
                    if (e.IsFatal)
                        logger.LogCritical(e.Reason, new { Code = (int)e.Code });
                    else
                        logger.LogError(e.Reason, new { Code = (int)e.Code });
                })
                .SetLogHandler((_, m) =>
                {
                    // ignore these messages. It just means the connection is "idle"
                    if (m.Message?.EndsWith("brokers are down") == true)
                        return;
                    logger.Log(m.Level.GetLogLevel(), m.Message + ": {@details}", new { m.Name, m.Facility, SyslogLevel = m.Level });
                });
            //.SetStatisticsHandler((p, stats) => MessageDiagnostics.Listener.IfEnabled(MessageDiagnostics.ProducerStatistics)?.Write(new { Producer = p, Statistics = stats }));
            if (features.StatisticsHandling != null)
                builder.SetStatisticsHandler((c, stats) => features.StatisticsHandling.OnStatistics(stats));

            producer = builder.Build();
        }

        /// <inheritdoc/>
        public async Task ProduceAsync(IMessage<TKey, TValue> message)
        {
            var kafkaMessage = new Confluent.Kafka.Message<TKey, TValue>
            {
                Key = message.Key,
                Value = message.Value,
                Headers = new Headers()
            };

            kafkaMessage.Headers.Add(MessageHeaders.ActivityId, Encoding.UTF8.GetBytes(message.ActivityId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)));
            message.ForEachHeader(header => kafkaMessage.Headers.Add(header.Key, GetHeaderBytes(header.Value)));

            await producer.ProduceAsync(options.Topic, kafkaMessage);
        }

        private byte[] GetHeaderBytes(object value)
        {
            switch (value)
            {
                case byte[] b: return b;
                case string s: return Encoding.UTF8.GetBytes(s);
                case byte b: return new byte[] { b };
                case sbyte b: return new byte[] { (byte)b };
                case bool b: return b ? new byte[] { 1 } : new byte[] { 0 };
                case short i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case int i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case long i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case ushort i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case uint i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case ulong i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case float i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case double i:
                    {
                        var bytes = BitConverter.GetBytes(i);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);
                        return bytes;
                    }
                case decimal d:
                    {
                        var result = new byte[128];
                        var b = decimal.GetBits(d);
                        for (int i = 0; i < 4; i++)
                        {
                            var bytes = BitConverter.GetBytes(b[i]);
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(bytes);
                            Array.Copy(bytes, 0, result, i * 32, 32);
                        }
                        return result;
                    }
            }

            throw new InvalidOperationException("Header value type not handled: " + value.GetType().Name);
        }

        private class CustomFeatures : FeatureSet
        {
            public ProducerFactoryFeature<TKey, TValue> FactoryFeature { get; private set; }

            public IConfigFeature Config { get; private set; } = new CompositeConfigFeature();

            public IKafkaConfigFeature<ProducerConfig> KafkaConfig { get; private set; } = new CompositeKafkaConfigFeature<ProducerConfig>();

            public IKafkaConfigBuilderFeature<ProducerBuilder<TKey, TValue>> BuilderFeature { get; private set; }

            public IStatisticsHandlingFeature StatisticsHandling { get; private set; }

            public KafkaSerializationFeature<TKey, TValue> SerializationFeature { get; private set; }
        }

        private class ProducerBuilderFactory : ProducerBuilder<TKey, TValue>
        {
            public ProducerBuilderFactory(IEnumerable<KeyValuePair<string, string>> config) : base(config) { }

            public Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> ProducerFactory { get; set; }

            public ProducerBuilder<TKey, TValue> SetFactory(Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> producerFactory)
            {
                ProducerFactory = producerFactory;
                return this;
            }
            public ProducerBuilder<TKey, TValue> SetFactory(ProducerFactoryFeature<TKey, TValue> factoryFeature)
            {
                ProducerFactory = factoryFeature?.ProducerFactory;
                return this;
            }

            public override IProducer<TKey, TValue> Build()
            {
                return ProducerFactory?.Invoke(this) ?? base.Build();
            }
        }
    }
}
