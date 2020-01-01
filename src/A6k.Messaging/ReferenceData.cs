using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace A6k.Messaging
{
    /// <summary>
    /// A Dictionary representing reference data
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReferenceData<TKey, TValue> : IReferenceData<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> items = new ConcurrentDictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            if (key == null) return;
            items[key] = value;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null) return default;
                return items[key];
            }
        }

        public IEnumerable<TKey> Keys => items.Keys;
        public IEnumerable<TValue> Values => items.Values;
        public int Count => items.Count;

        public bool ContainsKey(TKey key)
        {
            if (key == null) return false;
            return items.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }
            return items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
