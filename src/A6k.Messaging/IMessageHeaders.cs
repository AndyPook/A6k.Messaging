using System;
using System.Collections.Generic;

namespace A6k.Messaging
{
    public interface IMessageHeaders : IEnumerable<IMessageHeader>
    {
        object GetHeader(string key);
        void ForEach(Action<IMessageHeader> action);
    }
}
