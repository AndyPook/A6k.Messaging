using System;

namespace A6k.Messaging.Features
{
    public interface IFeatureSet
    {
        IServiceProvider ServiceProvider { get; }
        StateFeature State { get; }

        /// <summary>
        /// Retrieves the requested feature from the collection.
        /// </summary>
        /// <typeparam name="TFeature">The feature key.</typeparam>
        /// <returns>The requested feature, or null if it is not present.</returns>
        TFeature Get<TFeature>();

        /// <summary>
        /// Sets the given feature in the collection.
        /// </summary>
        /// <typeparam name="TFeature">The feature type</typeparam>
        /// <param name="feature">The feature instance</param>
        void Set<TFeature>(TFeature feature) where TFeature : class;
    }
}
