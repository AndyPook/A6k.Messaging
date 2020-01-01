using System;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// Base class for holding a set of Features used by a consumer, producer or pump
    /// </summary>
    public abstract class FeatureCollectionBase : IFeatureCollection
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public StateFeature State { get; private set; }

        public abstract TFeature Get<TFeature>() where TFeature : IFeature;

        public virtual void Set<TFeature>(TFeature feature) where TFeature : class, IFeature
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            if (feature is ServiceProviderFeature sp)
                ServiceProvider = sp.ServiceProvider;

            if (feature is StateFeature sf)
                State = sf;
            
            if (feature is IRequiresState rs)
                rs.SetState(State);
        }

        protected void TrySet<T>(object feature, ref T prop) where T : class, IFeature
        {
            if (feature is T f)
            {
                if (prop == default)
                    prop = f;
                else if (prop is CompositeFeature<T> composite)
                    composite.Add(f);
                else
                    throw new InvalidOperationException($"Feature already set: {typeof(T).Name}");
            }
        }

        protected TFeature TryGet<TFeature>(params object[] features) where TFeature : IFeature
        {
            foreach (var feature in features)
            {
                if (feature is TFeature f)
                    return f;
            }
            return default;
        }
    }
}
