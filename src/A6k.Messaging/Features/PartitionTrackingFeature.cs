using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace A6k.Messaging.Features
{
    public class PartitionTrackingFeature : IPartitionTrackingFeature, IEnumerable<KeyValuePair<string, IEnumerable<int>>>
    {
        private readonly ConcurrentDictionary<(string Topic, int Partition), byte> partitions = new ConcurrentDictionary<(string, int), byte>();

        public void PartitionAssigned(string topic, int partition)
        {
            partitions.TryAdd((topic, partition), 0);
        }

        public void PartitionRevoked(string topic, int partition, long offset) => partitions.TryRemove((topic, partition), out _);

        public IEnumerator<KeyValuePair<string, IEnumerable<int>>> GetEnumerator()
        {
            return
                (
                    from p in partitions.Keys
                    group p by p.Topic into g
                    select new KeyValuePair<string, IEnumerable<int>>(g.Key, (IEnumerable<int>)g.ToList())
                )
                .ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
