using System;
using System.Collections.Concurrent;

namespace A6k.Messaging
{
    public static class FakeMessageBusProvider
    {
        public const string ProviderName = "fake";

        private static readonly ConcurrentDictionary<string, Lazy<object>> topics = new ConcurrentDictionary<string, Lazy<object>>();

        public static BlockingCollection<IMessage<TKey, TValue>> GetTopic<TKey, TValue>(string topicName)
        {
            var item = topics.GetOrAdd(topicName, key => new Lazy<object>(() => new BlockingCollection<IMessage<TKey, TValue>>()));
            return (BlockingCollection<IMessage<TKey, TValue>>)item.Value;
        }

        public static void Clear() => topics.Clear();
    }
}
