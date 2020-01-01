using System;
using A6k.Messaging.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static bool validateKafkaOptionsConfigured = false;

        public static IServiceCollection ConfigureKafkaOptions(this IServiceCollection services, IConfiguration configuration, string name)
        {
            services.AddHealthChecks().AddCheck<KafkaHealthCheck>(name, tags: new[] { "kafka" });

            return services
                .Configure<KafkaOptions>(name, configuration.GetSection(name))
                .AddKafkaOptionsPostConfigure()
                .AddKafkaOptionsValidation();
        }

        /// <summary>
        /// Register for validation of KafkaOptions.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaOptionsValidation(this IServiceCollection services)
        {
            services.TryAddSingleton<IValidateOptions<KafkaOptions>>(new KafkaOptionsValidation());
            return services;
        }

        private static IServiceCollection AddKafkaOptionsPostConfigure(this IServiceCollection services)
        {
            if (validateKafkaOptionsConfigured)
                return services;
            validateKafkaOptionsConfigured = true;

            return services.AddSingleton((Func<IServiceProvider, IPostConfigureOptions<KafkaOptions>>)(sp =>
            {
                var appName = sp.GetService<IHostingEnvironment>()?.ApplicationName;

                return new PostConfigureOptions<KafkaOptions>(null, o =>
                {
                    // ensure "group.id" is set. Default to name of the current app
                    if (string.IsNullOrWhiteSpace(o.GroupId) && !string.IsNullOrWhiteSpace(appName))
                        o.GroupId = appName;
                });
            }));
        }

        public static IServiceCollection AddKafkaMessageProviders(this IServiceCollection services)
        {
            return services.AddMessageProviders(KafkaOptions.ProviderName, typeof(KafkaProducer<,>), typeof(KafkaConsumer<,>));
        }
    }
}
