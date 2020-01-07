using System.Collections.Generic;

namespace A6k.Messaging.Kafka.Features
{
    public interface IConfigFeature
    {
        void Configure(IDictionary<string, string> config);
    }
}
