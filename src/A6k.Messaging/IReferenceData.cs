using System.Collections.Generic;

namespace A6k.Messaging
{
    /// <summary>
    /// A marker interface representing a read-only Dictionary of reference data
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReferenceData<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> { }
}
