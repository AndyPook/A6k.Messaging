using A6k.Messaging.Features;
using System;
using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// Defines the api of a message consumer
    /// </summary>
    /// <typeparam name="TKey">Key type of the message</typeparam>
    /// <typeparam name="TValue">Value type of the message</typeparam>
    public interface IMessageConsumer<TKey, TValue> : IHasFeatures, IDisposable
    {
        /// <summary>
        /// Get a message from the infrastructure
        /// </summary>
        /// <returns>a message or null is none available</returns>
        Task<IMessage<TKey, TValue>> ConsumeAsync();

        /// <summary>
        /// Indicates that the message has been handled successfully
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task AcceptAsync(IMessage message);

        /// <summary>
        /// Indicates that a message handling failed
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task RejectAsync(IMessage<TKey, TValue> message);
    }
}
