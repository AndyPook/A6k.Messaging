namespace A6k.Messaging.Kafka
{
    public interface IKafkaMetaData
    {
        string Topic { get; }

        int Partition { get; }

        long Offset { get; }
    }
}
