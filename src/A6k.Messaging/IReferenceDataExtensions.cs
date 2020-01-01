using System.Collections.Generic;

namespace A6k.Messaging
{
    public static class IReferenceDataExtensions
    {
        /// <summary>
        /// Add an item to a reference data collection
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="data">The reference data object</param>
        /// <param name="key">The key to add to</param>
        /// <param name="value">The item to add to the collection</param>
        public static void Add<TKey, TValue>(this IReferenceData<TKey, IReadOnlyCollection<TValue>> data, TKey key, TValue value)
        {
            if (!data.TryGetValue(key, out var items))
                items = new HashSet<TValue>();
            ((HashSet<TValue>)items).Add(value);
        }


        /// <summary>
        /// Add an item to a reference data collection
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="data">The reference data object</param>
        /// <param name="key">The key to add to</param>
        /// <param name="value">The item to add to the collection</param>
        public static void Add<TKey, TValue>(this IReferenceData<TKey, ICollection<TValue>> data, TKey key, TValue value)
        {
            if (!data.TryGetValue(key, out var items))
                items = new HashSet<TValue>();
            ((HashSet<TValue>)items).Add(value);
        }
    }
}
