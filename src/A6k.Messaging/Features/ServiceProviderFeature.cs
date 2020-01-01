using System;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// Holds the <see cref="IServiceProvider"/> useful in places where a feature/helper needs to be constructed inside a consumer, producer
    /// </summary>
    public class ServiceProviderFeature : IFeature
    {
        public ServiceProviderFeature(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
