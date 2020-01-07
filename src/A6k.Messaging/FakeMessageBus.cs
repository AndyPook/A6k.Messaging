using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using A6k.Messaging.Features;

namespace A6k.Messaging
{
    /// <summary>
    /// A pair of Producer/Consumer joined via an in-memory collection. 
    /// Useful for testing.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public sealed class FakeMessageBus<TKey, TValue> : IMessageProducer<TKey, TValue>, IMessageConsumer<TKey, TValue>
    {
        private readonly BlockingCollection<IMessage<TKey, TValue>> topic;

        private Task completion;

        public FakeMessageBus(string topicName = null)
        {
            TopicName = topicName ?? Guid.NewGuid().ToString();
            topic = FakeMessageBusProvider.GetTopic<TKey, TValue>(TopicName);
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
        public Task AcceptAsync(IMessage message) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task RejectAsync(IMessage<TKey, TValue> message) => AcceptAsync(message);


        /// <inheritdoc/>
        public Task<IMessage<TKey, TValue>> ConsumeAsync()
        {
            if (topic.TryTake(out var message, 100))
                return Task.FromResult(message);

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
            // The Fake Bus has no custom features
        }
    }
}
