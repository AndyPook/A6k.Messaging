using System.Collections.Generic;
using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public interface IConfigFeature : IFeature
    {
        void Configure(IDictionary<string, string> config);
    }
}
