using System;

namespace A6k.Messaging.Features
{
    public class NullFeatureSet : IFeatureSet
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public StateFeature State { get; private set; }

        public TFeature Get<TFeature>() => default;

        public void Set<TFeature>(TFeature feature) where TFeature : class { }
    }
}
