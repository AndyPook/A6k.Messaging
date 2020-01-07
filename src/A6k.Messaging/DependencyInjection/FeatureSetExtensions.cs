using System;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FeatureSetExtensions
    {
        /// <summary>
        /// Add a Feature constructed via the <see cref="IServiceProvider"/> 
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="f"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IFeatureSet Set<TFeature>(this IFeatureSet f, params object[] args) where TFeature : class
        {
            if (f.ServiceProvider != null)
            {
                var feature = ActivatorUtilities.CreateInstance<TFeature>(f.ServiceProvider, args);
                f.Set(feature);
            }
            else
                throw new ArgumentException("ServiceProvider not initialized");

            return f;
        }

        /// <summary>
        /// Add a Feature constructed via the <see cref="IServiceProvider"/> 
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="f"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IFeatureSet Set<TFeature>(this IFeatureSet f, Func<IServiceProvider, TFeature> factory) where TFeature : class
        {
            if (f.ServiceProvider != null)
            {
                var feature = factory(f.ServiceProvider);
                f.Set(feature);
            }
            else
                throw new ArgumentException("ServiceProvider not initialized");

            return f;
        }

        /// <summary>
        /// Gets a state bag for this consumer or producer
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static StateFeature GetState(this IFeatureSet f)
        {
            if (f.State != null)
                return f.State;
            else
                throw new ArgumentException("State not initialized");
        }

        /// <summary>
        /// Catch errors that occur when consuming messages from the Topic/Queue
        /// </summary>
        /// <param name="f"></param>
        /// <param name="onError">A method taking the <see cref="Exception"/> thrown while consuming</param>
        /// <returns></returns>
        public static IFeatureSet OnConsumeError(this IFeatureSet f, Action<Exception> onError)
            => f.OnConsumeError(new DelegateConsumeErrorFeature(onError));

        /// <summary>
        /// Catch errors that occur when consuming messages from the Topic/Queue
        /// </summary>
        /// <param name="f"></param>
        /// <param name="onError">A <see cref="IConsumeErrorFeature"/> taking the <see cref="Exception"/> thrown while consuming</param>
        /// <returns></returns>
        public static IFeatureSet OnConsumeError(this IFeatureSet f, IConsumeErrorFeature onError)
        {
            if (onError == null)
                throw new ArgumentNullException(nameof(onError));

            f.Set(onError);

            return f;
        }

        /// <summary>
        /// Indicate which other Consumers to wait for before starting to process
        /// </summary>
        /// <param name="f"></param>
        /// <param name="names">The config names of the consumers to wait for</param>
        /// <returns></returns>
        public static IFeatureSet WaitFor(this IFeatureSet f, params string[] names)
        {
            if (names == null || names.Length == 0)
                return f;

            f.Set(new MessagePumpWaitFeature(names));

            return f;
        }
    }
}
