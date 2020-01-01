using System;
using System.Threading.Tasks;
using A6k.Messaging.Internal;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WaitTokenExtensions
    {
        /// <summary>
        /// Add a HostedService that is hooked into the WaitFor feature
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="name">Name to be used in WaitFor feature</param>
        /// <returns></returns>
        public static IServiceCollection AddHostedServiceWithWaitToken<T>(this IServiceCollection services, string name)
                where T : IHostedService
        {
            var trigger = services.GetWaitTokenTrigger(name);
            return services.AddTransient<IHostedService>(sp => ActivatorUtilities.CreateInstance<T>(sp, trigger));
        }

        /// <summary>
        /// Add a named token that can be used in the WaitFor feature
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">Name to be used in WaitFor feature</param>
        /// <param name="task">Task to wait for</param>
        /// <returns></returns>
        public static IServiceCollection AddWaitToken(this IServiceCollection services, string name, Task task)
        {
            var trigger = services.GetWaitTokenTrigger(name);
            if (task.IsCompleted)
                trigger.Trigger();
            else
                task.ContinueWith(t => trigger.Trigger());
            return services.AddSingleton(trigger.Token);
        }

        /// <summary>
        /// Add a named token that can be used in the WaitFor feature
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">Name to be used in WaitFor feature</param>
        /// <returns>An Action that, when invoked, will complete the WaitToken (null if name missing)</returns>
        public static WaitTokenSource GetWaitTokenTrigger(this IServiceCollection services, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var trigger = new WaitTokenSource(name);
            services.AddSingleton(trigger.Token);

            return trigger;
        }
    }
}
