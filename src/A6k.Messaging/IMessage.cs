using System;
using System.Collections.Generic;

namespace A6k.Messaging
{
    /// <summary>
    /// Basic details of a Message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The ActivityId associated with this message (will be set automatically)
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        /// Time the message was produced
        /// </summary>
        DateTime Timestamp { get; }
        string Topic { get; }
        int Partition { get; }
        long Offset { get; }

        IEnumerable<KeyValuePair<string, object>> Headers { get; }
        object GetHeader(string key);
        IMessage AddHeader(string key, object value);
        void ForEachHeader(Action<KeyValuePair<string, object>> action);
    }

    /// <summary>
    /// Access the details and Key/Value of a message
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMessage<TKey, TValue> : IMessage
    {
        TKey Key { get; }
        TValue Value { get; }
    }
}
