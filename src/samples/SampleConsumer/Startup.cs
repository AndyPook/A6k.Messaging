using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleConsumer
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureKafkaOptions(config, "Products");
            services.AddKafkaMessageProviders();

            // simplest way of registering a MessagePump with a typed Handler
            services.AddMessagePump<string, ProductEvent, ProductEventHandler>("Products");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure() { }
    }
}
