using System;
using System.Collections;
using System.Collections.Generic;

namespace A6k.Messaging
{
    public sealed class MessageHeaders : IMessageHeaders
    {
        public const string ActivityId = "activity-id";

        private MessageHeader headers;

        /// <inheritdoc/>
        public void ForEach(Action<IMessageHeader> action)
        {
            for (var header = headers; header != default; header = header.Next)
                action(header);
        }

        /// <inheritdoc/>
        public object GetHeader(string key)
        {
            for (var header = headers; header != default; header = header.Next)
                if (key == header.Key)
                    return header.Value;

            return null;
        }

        public MessageHeaders AddHeader(string key, object value)
        {
            headers = new MessageHeader(key, value, headers);
            return this;
        }

        public void Clear() => headers = null;

        public IEnumerator<IMessageHeader> GetEnumerator()
        {
            for (var h = headers; h != null; h = h.Next)
                yield return h;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
