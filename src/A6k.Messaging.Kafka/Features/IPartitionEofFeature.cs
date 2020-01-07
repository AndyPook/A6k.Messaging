namespace A6k.Messaging.Kafka.Features
{
    public interface IPartitionEofFeature
    {
        void PartitionIsEof(string topic, int partition);
    }
}
