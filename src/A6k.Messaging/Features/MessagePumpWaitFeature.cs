using System.Collections.Generic;
using System.Linq;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// Holds the set of "names" that a pump should wait on before starting
    /// </summary>
    public class MessagePumpWaitFeature : IMessagePumpWaitFeature
    {
        public MessagePumpWaitFeature(params string[] names)
        {
            Names = names;
        }

        public MessagePumpWaitFeature(IEnumerable<string> names)
        {
            Names = names.ToArray();
        }

        public string[] Names { get; }
    }
}
