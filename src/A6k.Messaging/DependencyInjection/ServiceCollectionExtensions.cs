using System;
using A6k.Messaging;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a Message Pump (ie a Consumer) using a Builder
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="builder">Action to configure builder</param>
        /// <returns>A Fluent Interface to define options. End with `.Build()` to construct the pump.</returns>
        public static IServiceCollection AddMessagePump<TKey, TValue>(
            this IServiceCollection services,
            string configName,
            Action<MessagePumpBuilder<TKey, TValue>> builder)
            => AddMessagePump(services, configName, null, builder);

        /// <summary>
        /// Add a Message Pump (ie a Consumer) using a Builder
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="providerType">Name of the Messaging Provider/></param>
        /// <param name="builder">Action to configure builder</param>
        /// <returns>A Fluent Interface to define options. End with `.Build()` to construct the pump.</returns>
        public static IServiceCollection AddMessagePump<TKey, TValue>(
            this IServiceCollection services,
            string configName,
            string providerType,
            Action<MessagePumpBuilder<TKey, TValue>> builder)
        {
            var b = new MessagePumpBuilder<TKey, TValue>(services, configName, providerType);
            builder(b);
            return b.AddMessagePump();
        }

        // --- the following are just overloads or syntactic sugar on top of the above MessagePumpBuilder
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Add a Message Pump (ie a Consumer)
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <typeparam name="THandler">Type of the Handler</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="featureConfig">Features to add to this Consumer</param>
        /// <returns></returns>
        public static IServiceCollection AddMessagePump<TKey, TValue, THandler>(
            this IServiceCollection services,
            string configName,
            Action<IFeatureCollection> featureConfig = null)
            where THandler : IMessageHandler<TKey, TValue>
            => services.AddMessagePump<TKey, TValue, THandler>(configName, null, featureConfig);

        /// <summary>
        /// Add a Message Pump (ie a Consumer)
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <typeparam name="THandler">Type of the Handler</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="providerType">Name of the Messaging Provider/></param>
        /// <param name="featureConfig">Features to add to this Consumer</param>
        /// <returns></returns>
        public static IServiceCollection AddMessagePump<TKey, TValue, THandler>(
            this IServiceCollection services,
            string configName,
            string providerType,
            Action<IFeatureCollection> featureConfig = null)
            where THandler : IMessageHandler<TKey, TValue>
            => services.AddMessagePump<TKey, TValue>(configName, providerType, b =>
                {
                    b.WithHandler<THandler>();
                    b.WithFeatures(featureConfig);
                });
    }
}
