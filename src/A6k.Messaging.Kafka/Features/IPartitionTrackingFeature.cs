namespace A6k.Messaging.Kafka.Features
{
    /// <summary>
    /// This feature is used when a new consumer group partition assignment has been received by this consumer.
    /// </summary>
    public interface IPartitionTrackingFeature
    {
        /// <summary>
        /// This method is called when a new consumer group partition assignment has been received by this consumer.
        /// Note: corresponding to every call to this handler
        /// there will be a corresponding call to the partitions revoked handler (if one
        /// has been set using SetPartitionsRevokedHandler). The actual partitions to consume
        /// from and start offsets are specfied by the return value of the handler. This
        /// set of partitions is not required to match the assignment provided by the consumer
        /// group, but typically will.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        void PartitionAssigned(string topic, int partition);

        /// <summary>
        /// This method is called immediately prior to a group partition assignment being revoked.
        /// The parameters provide a partitiona the consumer is currently assigned to, 
        /// and the current position of the consumer on each of the partition.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="offset"></param>
        void PartitionRevoked(string topic, int partition, long offset);
    }

    /// <summary>
    /// This feature is used when a new consumer group partition assignment has been received by this consumer.
    /// </summary>
    public interface IPartitionAssignmentFeature
    {
        /// <summary>
        /// This method is called when a new consumer group partition assignment has been received by this consumer.
        /// Note: corresponding to every call to this handler
        /// there will be a corresponding call to the partitions revoked handler (if one
        /// has been set using SetPartitionsRevokedHandler). The actual partitions to consume
        /// from and start offsets are specfied by the return value of the handler. This
        /// set of partitions is not required to match the assignment provided by the consumer
        /// group, but typically will. Partition offsets may be a specific offset, or special
        /// value (Beginning, End or Unset). If Unset, consumption will resume from the last
        /// committed offset for each partition, or if there is no committed offset, in accordance
        /// with the `auto.offset.reset` configuration property.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <returns>The offset to associate with this assignment (return `null` for default behavior)</returns>
        long? PartitionAssigned(string topic, int partition);

        /// <summary>
        /// This method is called immediately prior to a group partition assignment being revoked.
        /// The parameters provide a partitiona the consumer is currently assigned to, 
        /// and the current position of the consumer on each of the partition.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="partition"></param>
        /// <param name="offset"></param>
        void PartitionRevoked(string topic, int partition, long offset);
    }
}
