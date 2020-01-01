using System;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FeatureCollectionExtensions
    {
        /// <summary>
        /// Add a Feature constructed via the <see cref="IServiceProvider"/> 
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="f"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IFeatureCollection Set<TFeature>(this IFeatureCollection f, params object[] args) where TFeature : class, IFeature
        {
            if (f is FeatureCollectionBase features)
            {
                var feature = ActivatorUtilities.CreateInstance<TFeature>(features.ServiceProvider, args);
                f.Set(feature);
            }
            else
                throw new ArgumentException("FeatureCollection does not support ServiceProvider");

            return f;
        }

        /// <summary>
        /// Add a Feature constructed via the <see cref="IServiceProvider"/> 
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="f"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IFeatureCollection Set<TFeature>(this IFeatureCollection f, Func<IServiceProvider, TFeature> factory) where TFeature : class, IFeature
        {
            if (f is FeatureCollectionBase features)
            {
                var feature = factory(features.ServiceProvider);
                f.Set(feature);
            }
            else
                throw new ArgumentException("FeatureCollection does not support ServiceProvider");

            return f;
        }

        /// <summary>
        /// Gets a state bag for this consumer or producer
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static StateFeature GetState(this IFeatureCollection f)
        {
            if (f is FeatureCollectionBase features)
                return features.State;
            else
                throw new ArgumentException("FeatureCollection does not support ServiceProvider");
        }

        /// <summary>
        /// Catch errors that occur when consuming messages from the Topic/Queue
        /// </summary>
        /// <param name="f"></param>
        /// <param name="onError">A method taking the <see cref="Exception"/> thrown while consuming</param>
        /// <returns></returns>
        public static IFeatureCollection OnConsumeError(this IFeatureCollection f, Action<Exception> onError)
            => f.OnConsumeError(new DelegateConsumeErrorFeature(onError));

        /// <summary>
        /// Catch errors that occur when consuming messages from the Topic/Queue
        /// </summary>
        /// <param name="f"></param>
        /// <param name="onError">A <see cref="IConsumeErrorFeature"/> taking the <see cref="Exception"/> thrown while consuming</param>
        /// <returns></returns>
        public static IFeatureCollection OnConsumeError(this IFeatureCollection f, IConsumeErrorFeature onError)
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
        public static IFeatureCollection WaitFor(this IFeatureCollection f, params string[] names)
        {
            if (names == null || names.Length == 0)
                return f;

            f.Set(new MessagePumpWaitFeature(names));

            return f;
        }

        /// <summary>
        /// Start at the beginning of the Topic
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureCollection StartAtBeginning(this IFeatureCollection f)
        {
            f.Set(StartAtPositionFeature.Beginning);

            return f;
        }

        /// <summary>
        /// Start at the end of the Topic
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureCollection StartAtEnd(this IFeatureCollection f)
        {
            f.Set(StartAtPositionFeature.End);

            return f;
        }

        /// <summary>
        /// Set the GroupId to something unique
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureCollection UniqueGroupId(this IFeatureCollection f)
        {
            f.Set(new UniqueGroupIdFeature());

            return f;
        }
    }
}
