using System.Collections.Generic;

namespace A6k.Messaging.Features
{
    public interface IConfigFeature : IFeature
    {
        void Configure(IDictionary<string, string> config);
    }
}
