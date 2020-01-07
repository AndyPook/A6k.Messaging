using System;
using System.Collections.Generic;
using A6k.Messaging;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessageProviderExtensions
    {
        private static (Type ProducerType, Type ConsumerType) defaultProvider;
        private static readonly Dictionary<string, (Type ProducerType, Type ConsumerType)> types = new Dictionary<string, (Type ProducerType, Type ConsumerType)>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Sets up "Fake" providers for testing purposes
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFakeMessageProviders(this IServiceCollection services)
            => services.AddMessageProviders(FakeMessageBusProvider.ProviderName, typeof(FakeMessageBus<,>), typeof(FakeMessageBus<,>));

        /// <summary>
        /// Adds providers required for each messaging type (kafka, fake, mq ...)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="producerType"></param>
        /// <param name="consumerType"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageProviders(this IServiceCollection services, string name, Type producerType, Type consumerType)
        {
            if (!IsAssignableToGenericType(producerType, typeof(IMessageProducer<,>)))
                throw new ArgumentException($"Type must be of type {typeof(IMessageProducer<,>).Name}", nameof(producerType));
            if (!IsAssignableToGenericType(consumerType, typeof(IMessageConsumer<,>)))
                throw new ArgumentException($"Type must be of type {typeof(IMessageConsumer<,>).Name}", nameof(consumerType));

            if (defaultProvider == default)
                defaultProvider = (producerType, consumerType);

            types[name] = (producerType, consumerType);
            return services;

            bool IsAssignableToGenericType(Type givenType, Type genericType)
            {
                foreach (var it in givenType.GetInterfaces())
                {
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                        return true;
                }

                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                    return true;

                Type baseType = givenType.BaseType;
                if (baseType == null) return false;

                return IsAssignableToGenericType(baseType, genericType);
            }
        }

        private static (Type ProducerType, Type ConsumerType) GetProvider(string providerName)
        {
            if (providerName == null)
            {
                if (defaultProvider == default)
                    throw new ArgumentException("provider type not set", nameof(providerName));
                return defaultProvider;
            }

            if (types.TryGetValue(providerName, out var providerType))
                return providerType;

            throw new ArgumentException("unrecognized provider type", nameof(providerName));
        }

        /// <summary>
        /// gets a Message Producer for one of the provider types.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sp"></param>
        /// <param name="configName">Name of the configured options</param>
        /// <param name="providerName">A configured provider type name (via <see cref="AddMessageProviders(IServiceCollection, string, Type, Type)"/>). If null, then use the first configured provider</param>
        /// <param name="featureConfig"></param>
        /// <returns></returns>
        public static IMessageProducer<TKey, TValue> GetMessageProducer<TKey, TValue>(this IServiceProvider sp, string configName = null, string providerName = null, Action<IFeatureSet> featureConfig = null)
        {
            var (ProducerType, ConsumerType) = GetProvider(providerName);

            var ctype = ProducerType.MakeGenericType(typeof(TKey), typeof(TValue));
            var producer = (IMessageProducer<TKey, TValue>)ActivatorUtilities.CreateInstance(sp, ctype, configName);
            producer.Configure(f =>
            {
                f.Set(new ServiceProviderFeature(sp));
                featureConfig?.Invoke(f);
            });
            return producer;
        }

        /// <summary>
        /// Gets a Message Consumer for one of the provider types
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sp"></param>
        /// <param name="configName">Name of the configured options</param>
        /// <param name="providerName">A configured provider type name (via <see cref="AddMessageProviders(IServiceCollection, string, Type, Type)"/>). If null, then use the first configured provider</param>
        /// <param name="featureConfig"></param>
        /// <returns></returns>
        public static IMessageConsumer<TKey, TValue> GetMessageConsumer<TKey, TValue>(this IServiceProvider sp, string configName = null, string providerName = null, Action<IFeatureSet> featureConfig = null)
        {
            var (ProducerType, ConsumerType) = GetProvider(providerName);

            var ctype = ConsumerType.MakeGenericType(typeof(TKey), typeof(TValue));
            var consumer = (IMessageConsumer<TKey, TValue>)ActivatorUtilities.CreateInstance(sp, ctype, configName);
            consumer.Configure(f =>
            {
                f.Set(new ServiceProviderFeature(sp));
                featureConfig?.Invoke(f);
            });
            return consumer;
        }

        /// <summary>
        /// Gets a factory creating a Message Consumer.
        /// Useful for the stop/start feature of <see cref="MessagePump{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sp"></param>
        /// <param name="configName">Name of the configured options</param>
        /// <param name="providerName">A configured provider type name (via <see cref="AddMessageProviders(IServiceCollection, string, Type, Type)"/>). If null, then use the first configured provider</param>
        /// <param name="featureConfig"></param>
        /// <returns></returns>
        public static Func<IMessageConsumer<TKey, TValue>> GetMessageConsumerFactory<TKey, TValue>(this IServiceProvider sp, string configName = null, string providerName = null, Action<IFeatureSet> featureConfig = null)
            => () => sp.GetMessageConsumer<TKey, TValue>(configName, providerName, featureConfig);
    }
}
