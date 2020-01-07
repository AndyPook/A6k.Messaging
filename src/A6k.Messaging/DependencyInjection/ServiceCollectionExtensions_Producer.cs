using System;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions_Producer
    {
        /// <summary>
        /// Setup a Message Producer
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="featureConfig">Features to add to this Producer</param>
        public static IServiceCollection AddMessageProducer<TKey, TValue>(
            this IServiceCollection services,
            string configName,
            Action<IFeatureSet> featureConfig = null)
            => services.AddMessageProducer<TKey, TValue>(configName, null, featureConfig);

        /// <summary>
        /// Setup a Message Producer
        /// </summary>
        /// <typeparam name="TKey">Key type of the message</typeparam>
        /// <typeparam name="TValue">Value type of the message</typeparam>
        /// <param name="services"></param>
        /// <param name="configName">Name of the configuration/></param>
        /// <param name="providerType">Name of the Messaging Provider/></param>
        /// <param name="featureConfig">Features to add to this Producer</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageProducer<TKey, TValue>(
            this IServiceCollection services,
            string configName,
            string providerType,
            Action<IFeatureSet> featureConfig = null)
            => services.AddSingleton(sp => sp.GetMessageProducer<TKey, TValue>(configName, providerType, featureConfig));
    }
}
