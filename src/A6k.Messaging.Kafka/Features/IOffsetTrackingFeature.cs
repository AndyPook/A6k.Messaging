using System.Threading.Tasks;

namespace A6k.Messaging.Kafka.Features
{
    public interface IOffsetTrackingFeature<TKey, TValue>
    {
        Task AcceptAsync(IMessage message);
        Task RejectAsync(IMessage<TKey, TValue> message);
    }
}
