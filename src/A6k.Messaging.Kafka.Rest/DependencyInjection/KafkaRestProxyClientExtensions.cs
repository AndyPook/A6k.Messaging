using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for adding API service clients to DI via HttpClientFactory
    /// </summary>
    public static class KafkaRestProxyClientExtensions
    {
        /// <summary>
        /// Add an API Client (this is typically the one to use)
        /// </summary>
        /// <param name="services">DI services</param>
        /// <param name="serviceUrl">The root url of the Kafka RestProxy service to call</param>
        /// <returns>The client builder so that it can be extended further</returns>
        public static IHttpClientBuilder AddKafkaRestProxyClient(this IServiceCollection services, string serviceUrl)
        {
            return services
              .AddHttpClient<KafkaRestProxyClient>(client => { client.BaseAddress = new Uri(serviceUrl, UriKind.Absolute); });
        }
    }
}
