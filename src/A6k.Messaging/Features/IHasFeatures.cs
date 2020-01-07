using System;

namespace A6k.Messaging.Features
{
    public interface IHasFeatures
    {
        IFeatureSet Features { get; }
        void Configure(Action<IFeatureSet> configureFeatures = null);
    }
}
