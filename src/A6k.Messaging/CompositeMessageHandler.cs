using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// A <see cref="IMessageHandler{TKey, TValue}"/> that wraps a set of <see cref="IMessageHandler{TKey, TValue}"/>s.
    /// Allows several handlers to respond to a message.
    /// A convenience, primarily to support a fluent interface.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CompositeMessageHandler<TKey, TValue> : IMessageHandler<TKey, TValue>, IMessageHandlerName
    {
        private readonly LinkedList<IMessageHandler<TKey, TValue>> handlers = new LinkedList<IMessageHandler<TKey, TValue>>();

        public CompositeMessageHandler<TKey, TValue> AddHandler(Func<IMessage<TKey, TValue>, Task> handler, string name = null)
        {
            handlers.AddLast(new DelegateMessageHandler<TKey, TValue>(handler) { Name = name });
            return this;
        }

        public CompositeMessageHandler<TKey, TValue> AddHandler(Action<IMessage<TKey, TValue>> handler, string name = null)
        {
            handlers.AddLast(new DelegateMessageHandler<TKey, TValue>(handler) { Name = name });
            return this;
        }

        public CompositeMessageHandler<TKey, TValue> AddHandler(IMessageHandler<TKey, TValue> handler)
        {
            handlers.AddLast(handler);
            return this;
        }

        string name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    name = string.Join(",", handlers.Select(h => (h as IMessageHandlerName)?.Name ?? h.GetType().Name));

                return name;
            }
            set => name = value;
        }

        public int Count => handlers.Count;
        public IMessageHandler<TKey, TValue> First => handlers.First?.Value;

        /// <inheritdoc/>
        public async Task HandleAsync(IMessage<TKey, TValue> message)
        {
            for (var node = handlers.First; node != null; node = node.Next)
                await node.Value.HandleAsync(message);
        }
    }
}
