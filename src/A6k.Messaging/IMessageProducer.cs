using System.Threading.Tasks;
using A6k.Messaging.Features;

namespace A6k.Messaging
{
    /// <summary>
    /// Defines the api of a message producer
    /// </summary>
    /// <typeparam name="TKey">Key type of the message</typeparam>
    /// <typeparam name="TValue">Value type of the message</typeparam>
    public interface IMessageProducer<TKey, TValue>: IHasFeatures
    {
        /// <summary>
        /// Send a message to the infrastructure
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ProduceAsync(IMessage<TKey, TValue> message);
    }
}
