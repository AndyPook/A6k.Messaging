using System.Threading.Tasks;

using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public interface IOffsetTrackingFeature<TKey, TValue> : IFeature
    {
        Task AcceptAsync(IMessage message);
        Task RejectAsync(IMessage<TKey, TValue> message);
    }
}
