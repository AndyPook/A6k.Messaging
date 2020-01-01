using A6k.Messaging.Features;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// A pair of Producer/Consumer joined via an in-memory collection. 
    /// Useful for testing.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public sealed class FakeMessageTopic<TKey, TValue> : IMessageProducer<TKey, TValue>, IMessageConsumer<TKey, TValue>
    {
        private readonly BlockingCollection<IMessage<TKey, TValue>> topic;
        private readonly ConcurrentDictionary<int, byte> assignedPartitions = new ConcurrentDictionary<int, byte>();

        private Task completion;

        public FakeMessageTopic(string topicName = null)
        {
            TopicName = topicName ?? Guid.NewGuid().ToString();
            topic = FakeMessageTopicProvider.GetTopic<TKey, TValue>(TopicName);
        }

        public string TopicName { get; }

        /// <summary>
        /// Gets whether this Bus has been marked as complete for adding.
        /// </summary>
        public bool IsCompleted => topic.IsCompleted;

        /// <summary>
        /// Gets whether this Bus has been marked as complete for adding and is empty.
        /// </summary>
        public bool IsAddingCompleted => topic.IsAddingCompleted;

        /// <summary>
        /// Marks the System.Collections.Concurrent.BlockingCollection`1 instances as not accepting any more additions.
        /// </summary>
        public void CompleteAdding() => topic.CompleteAdding();

        /// <summary>
        /// An awaitable that completes when <see cref="IsCompleted" /> is true/>
        /// </summary>
        /// <returns>A Task that completes when this bus is completed and empty</returns>
        public Task CompletedAndEmpty()
        {
            return completion ?? (completion = Task.Run(async () =>
            {
                while (!topic.IsCompleted)
                    await Task.Delay(100);
            }));
        }

        private FeaturesCollection features = new FeaturesCollection();
        public IFeatureCollection Features => features;

        public void Configure(Action<IFeatureCollection> configureFeatures = null)
        {
            configureFeatures?.Invoke(features);
        }

        /// <inheritdoc/>
        public async Task AcceptAsync(IMessage message)
        {
            if (features.OffsetTracking != null)
                await features.OffsetTracking.AcceptAsync(message);
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
            if (topic.TryTake(out var message, 100))
            {
                if (assignedPartitions.TryAdd(message.Partition, 0))
                {
                    features.PartitionTracking?.PartitionAssigned(TopicName, message.Partition);
                    features.PartitionAssignment?.PartitionAssigned(TopicName, message.Partition);
                }

                return Task.FromResult(message);
            }

            features.PartitionEof?.PartitionIsEof(TopicName, 0);

            return Message<TKey, TValue>.Null;
        }

        /// <inheritdoc/>
        public Task ProduceAsync(IMessage<TKey, TValue> message)
        {
            topic.Add(message);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            topic.Dispose();
        }

        /// <summary>
        /// Specialised <see cref="IFeatureCollection"/> for the consumer
        /// </summary>
        private class FeaturesCollection : FeatureCollectionBase
        {
            private IOffsetTrackingFeature<TKey, TValue> offsetTracking;
            public IOffsetTrackingFeature<TKey, TValue> OffsetTracking => offsetTracking;

            private IPartitionTrackingFeature partitionTracking;
            public IPartitionTrackingFeature PartitionTracking => partitionTracking;

            private IPartitionAssignmentFeature partitionAssignment;
            public IPartitionAssignmentFeature PartitionAssignment => partitionAssignment;

            private IPartitionEofFeature partitionEof;
            public IPartitionEofFeature PartitionEof => partitionEof;

            private IMessagePumpWaitFeature messagePumpWaitFeature;
            public IMessagePumpWaitFeature MessagePumpWaitFeature => messagePumpWaitFeature;

            public override TFeature Get<TFeature>()
            {
                return TryGet<TFeature>(
                    offsetTracking,
                    partitionTracking,
                    partitionAssignment,
                    partitionEof,
                    messagePumpWaitFeature
                );
            }

            public override void Set<TFeature>(TFeature feature)
            {
                base.Set(feature);

                TrySet(feature, ref offsetTracking);
                TrySet(feature, ref partitionTracking);
                TrySet(feature, ref partitionAssignment);
                TrySet(feature, ref partitionEof);
                TrySet(feature, ref messagePumpWaitFeature);
            }
        }
    }
}
