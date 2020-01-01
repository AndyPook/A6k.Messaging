using Confluent.Kafka;

namespace A6k.Messaging.Kafka
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Get a <see cref="TopicPartitionOffset"/> representing the specified message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TopicPartitionOffset AsTopicPartitionOffset(this IMessage message)
        {
            return new TopicPartitionOffset(new TopicPartition(message.Topic, new Partition(message.Partition)), message.Offset + 1);
        }
    }
}
