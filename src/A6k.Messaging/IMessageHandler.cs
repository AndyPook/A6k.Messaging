using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// Implement this to perform some action for a message
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMessageHandler<TKey, TValue>
    {
        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns>An awaitable task</returns>
        Task HandleAsync(IMessage<TKey, TValue> message);
    }
}
