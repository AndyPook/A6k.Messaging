using System;
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
            if (!(message is IKafkaMetaData km))
                throw new ArgumentException("message is not a Kafka message", nameof(message));

            return new TopicPartitionOffset(new TopicPartition(km.Topic, new Partition(km.Partition)), km.Offset + 1);
        }
    }
}
