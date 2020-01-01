using System;
using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// A <see cref="IMessageHandler{TKey, TValue}"/> that wraps a delegate.
    /// A convenience, mainly for writing tests but can be useful elsewhere too.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DelegateMessageHandler<TKey, TValue> : IMessageHandler<TKey, TValue>, IMessageHandlerName
    {
        private readonly Func<IMessage<TKey, TValue>, Task> handler;

        public DelegateMessageHandler(Action<IMessage<TKey, TValue>> handler)
        {
            _ = handler ?? throw new ArgumentNullException(nameof(handler));
            this.handler = m => { handler(m); return Task.CompletedTask; };
        }

        public DelegateMessageHandler(Func<IMessage<TKey, TValue>, Task> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public string Name { get; set; }

        /// <inheritdoc/>
        public Task HandleAsync(IMessage<TKey, TValue> message) => handler(message);
    }
}
