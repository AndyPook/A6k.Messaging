namespace A6k.Messaging.Kafka
{
    public class KafkaMessage<TKey, TValue> : Message<TKey, TValue>, IKafkaMetaData
    {
        public KafkaMessage(string topic, int partition, long offset)
        {
            Topic = topic;
            Partition = partition;
            Offset = offset;
        }

        public string Topic { get; }

        public int Partition { get; }

        public long Offset { get; }
    }
}
