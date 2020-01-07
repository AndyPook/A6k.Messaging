using System;
using A6k.Messaging;
using A6k.Messaging.Kafka.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TopicMapServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a special MessagePump/Handler for reading a Topic into a "map".
        /// Provides a mechanism for allowing other parts to wait for the reading to the end of the Topic so the map is complete.
        /// See <see cref="FeatureSetExtensions.WaitFor"/>, in combination with "AddMessagePump".
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="mapConfig">Sets up a Handler with options to "map" the Messages being consumed</param>
        /// <returns></returns>
        public static IServiceCollection AddTopicMap<TKey, TValue>(this IServiceCollection services, string configName, Action<CompositeMessageHandler<TKey, TValue>> mapConfig)
            => services.AddTopicMap<TKey, TValue>(configName, null, mapConfig);

        /// <summary>
        /// Adds a special MessagePump/Handler for reading a Topic into a "map".
        /// Provides a mechanism for allowing other parts to wait for the reading to the end of the Topic so the map is complete.
        /// See <see cref="FeatureSetExtensions.WaitFor"/>, in combination with "AddMessagePump".
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="providerType">Name of the Messaging Provider/></param>
        /// <param name="mapConfig">Sets up a Handler with options to "map" the Messages being consumed</param>
        /// <returns></returns>
        public static IServiceCollection AddTopicMap<TKey, TValue>(this IServiceCollection services, string configName, string providerType, Action<CompositeMessageHandler<TKey, TValue>> mapConfig)
        {
            var eof = new TopicEofFeature();
            services.AddWaitToken(configName, eof.IsComplete);
            services.AddMessagePump<TKey, TValue>(configName, providerType, b =>
            {
                b.WithHandlers(mapConfig);
                b.WithFeatures(features =>
                {
                    features.StartAtBeginning();
                    features.UniqueGroupId();
                    features.Set(eof);
                });
            });

            return services;
        }

        /// <summary>
        /// Adds a special MessagePump/Handler for reading a Topic into a "map".
        /// Provides a mechanism for allowing other parts to wait for the reading to the end of the Topic so the map is complete.
        /// See <see cref="FeatureSetExtensions.WaitFor"/>, in combination with "AddMessagePump".
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="mapConfig">Sets up a Handler with options to "map" the Messages being consumed</param>
        /// <returns></returns>
        public static IServiceCollection AddTopicMap<TKey, TValue>(this IServiceCollection services, string configName, Action<IServiceProvider, CompositeMessageHandler<TKey, TValue>> mapConfig)
            => services.AddTopicMap<TKey, TValue>(configName, null, mapConfig);

        /// <summary>
        /// Adds a special MessagePump/Handler for reading a Topic into a "map".
        /// Provides a mechanism for allowing other parts to wait for the reading to the end of the Topic so the map is complete.
        /// See <see cref="FeatureSetExtensions.WaitFor"/>, in combination with "AddMessagePump".
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="providerType">Name of the Messaging Provider/></param>
        /// <param name="mapConfig">Sets up a Handler with options to "map" the Messages being consumed</param>
        /// <returns></returns>
        public static IServiceCollection AddTopicMap<TKey, TValue>(this IServiceCollection services, string configName, string providerType, Action<IServiceProvider, CompositeMessageHandler<TKey, TValue>> mapConfig)
        {
            var eof = new TopicEofFeature();
            services.AddWaitToken(configName, eof.IsComplete);
            services.AddMessagePump<TKey, TValue>(configName, providerType, b =>
            {
                b.WithHandlers(mapConfig);
                b.WithFeatures(features =>
                {
                    features.StartAtBeginning();
                    features.UniqueGroupId();
                    features.Set(eof);
                });
            });

            return services;
        }
    }
}
