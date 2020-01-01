using System.Threading.Tasks;

namespace A6k.Messaging.Features
{
    public interface IOffsetTrackingFeature<TKey, TValue> : IFeature
    {
        Task AcceptAsync(IMessage message);
        Task RejectAsync(IMessage<TKey, TValue> message);
    }
}
