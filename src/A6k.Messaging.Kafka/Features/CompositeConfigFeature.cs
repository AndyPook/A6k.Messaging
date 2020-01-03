using System;
using System.Collections.Generic;
using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public class CompositeConfigFeature : CompositeFeature<IConfigFeature>, IConfigFeature
    {
        public void Configure(IDictionary<string, string> config) => Invoke(f => f.Configure(config));
    }
}
