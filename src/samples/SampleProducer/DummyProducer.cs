using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using A6k.Messaging;

namespace SampleProducer
{
    public class DummyProducer : BackgroundService
    {
        private readonly IMessageProducer<string, ProductEvent> producer;

        public DummyProducer(IMessageProducer<string, ProductEvent> producer)
        {
            this.producer = producer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int productId = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                productId++;
                var product = new ProductEvent
                {
                    Identifier = productId.ToString(CultureInfo.InvariantCulture),
                    Name = "fred - " + Guid.NewGuid(),
                };

                Console.WriteLine($"sending: {product.Identifier} - {product.Name}");
                await producer.ProduceAsync(product.Identifier, product);

                Console.WriteLine("pausing...");
                try { await Task.Delay(1_000, stoppingToken); } catch (TaskCanceledException) { }
            }
        }
    }
}
