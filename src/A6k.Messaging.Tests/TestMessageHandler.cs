using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A6k.Messaging.Tests
{
    public partial class MessagePumpTests
    {
        /// <summary>
        /// Message hanlder that captures messages for later Assertions
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public class TestMessageHandler<TKey, TValue> : IMessageHandler<TKey, TValue>
        {
            private ConcurrentBag<IMessage<TKey, TValue>> messages = new ConcurrentBag<IMessage<TKey, TValue>>();
            public IReadOnlyCollection<IMessage<TKey, TValue>> Messages => messages;

            public Task HandleAsync(IMessage<TKey, TValue> message)
            {
                messages.Add(message);
                return Task.CompletedTask;
            }
        }
    }
}
