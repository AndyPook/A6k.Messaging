using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using A6k.Messaging.Features;
using A6k.Messaging.Kafka.Features;

namespace A6k.Messaging.Kafka
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

        private CustomFeatures features = new CustomFeatures();
        public IFeatureSet Features => features;

        public void Configure(Action<IFeatureSet> configureFeatures = null)
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
        /// Specialised <see cref="IFeatureSet"/> for the consumer
        /// </summary>
        private class CustomFeatures : FeatureSet
        {
            public IOffsetTrackingFeature<TKey, TValue> OffsetTracking { get; private set; }

            public IPartitionTrackingFeature PartitionTracking { get; private set; }

            public IPartitionAssignmentFeature PartitionAssignment { get; private set; }

            public IPartitionEofFeature PartitionEof { get; private set; }
        }
    }
}
