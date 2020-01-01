using System;
using System.Collections.Generic;

namespace A6k.Messaging.Features
{
    public class CompositeConfigFeature : CompositeFeature<IConfigFeature>, IConfigFeature
    {
        public void Configure(IDictionary<string, string> config) => Invoke(f => f.Configure(config));
    }
}
