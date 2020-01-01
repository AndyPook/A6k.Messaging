using System;

namespace A6k.Messaging.Features
{
    public interface IHasFeatures
    {
        IFeatureCollection Features { get; }
        void Configure(Action<IFeatureCollection> configureFeatures = null);
    }
}
