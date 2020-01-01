using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public interface IPartitionEofFeature : IFeature
    {
        void PartitionIsEof(string topic, int partition);
    }
}
