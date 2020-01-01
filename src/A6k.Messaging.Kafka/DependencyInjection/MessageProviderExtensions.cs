using A6k.Messaging.Kafka;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessageProviderExtensions
    {
        /// <summary>
        /// Sets up "Fake" providers for testing purposes
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFakeMessageTopicProviders(this IServiceCollection services)
            => services.AddMessageProviders(FakeMessageTopicProvider.ProviderName, typeof(FakeMessageTopic<,>), typeof(FakeMessageTopic<,>));
    }
}
