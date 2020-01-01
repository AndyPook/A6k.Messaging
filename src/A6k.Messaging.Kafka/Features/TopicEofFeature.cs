using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace A6k.Messaging.Kafka.Features
{
    /// <summary>
    /// Captures the moment when a consumer reaches the "end" of a topic/queue.
    /// Allows another pump to "wait" for this to complete
    /// </summary>
    public class TopicEofFeature : IPartitionEofFeature, IPartitionTrackingFeature
    {
        private readonly ConcurrentDictionary<(string Topic, int Partition), bool> partitions = new ConcurrentDictionary<(string, int), bool>();
        private readonly TaskCompletionSource<byte> isCompleteSource = new TaskCompletionSource<byte>();

        public void PartitionAssigned(string topic, int partition) => partitions.TryAdd((topic, partition), false);

        public void PartitionRevoked(string topic, int partition, long offset) => partitions.TryRemove((topic, partition), out _);

        public void PartitionIsEof(string topic, int partition)
        {
            partitions[(topic, partition)] = true;
            if (partitions.All(x => x.Value) && !isCompleteSource.Task.IsCompleted)
                isCompleteSource.SetResult(0);
        }

        public Task IsComplete => isCompleteSource.Task;
    }
}
