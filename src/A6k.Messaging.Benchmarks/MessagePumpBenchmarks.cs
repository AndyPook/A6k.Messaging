using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

using App.Metrics;
using App.Metrics.Timer;
using BenchmarkDotNet.Attributes;

using A6k.Messaging.Features;
using A6k.Messaging.Internal;

namespace A6k.Messaging.Benchmarks
{
    public class DelegateConsumer<TKey, TValue> : IMessageConsumer<TKey, TValue>
    {
        private readonly Func<IMessage<TKey, TValue>> messageFactory;

        public DelegateConsumer(Func<IMessage<TKey, TValue>> messageFactory)
        {
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }

        public IFeatureCollection Features { get; } = new FeaturesCollection();

        public Task AcceptAsync(IMessage message) => Task.CompletedTask;

        public void Configure(Action<IFeatureCollection> configureFeatures = null) { }

        public Task<IMessage<TKey, TValue>> ConsumeAsync() => Task.FromResult(messageFactory());

        public Task RejectAsync(IMessage<TKey, TValue> message) => Task.CompletedTask;

        public void Dispose() { }

        private class FeaturesCollection : FeatureCollectionBase
        {
            public override TFeature Get<TFeature>() => default;
        }
    }

    public class Handler : IMessageHandler<byte[], byte[]>
    {
        public Task HandleAsync(IMessage<byte[], byte[]> message) => Task.CompletedTask;
    }

    [MemoryDiagnoser]
    //[SimpleJob(invocationCount: 100)]
    public class MessagePumpBenchmarks
    {
        private IMessageConsumer<byte[], byte[]> consumer;
        private IMessageHandler<byte[], byte[]> handler;
        private string handlerName;
        private ILogger<MessagePump<byte[], byte[]>> logger;

        private ITimer successTimer;
        private ITimer failTimer;
        private IClock clock;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection()
                  .AddLogging(logging =>
                  {
                      logging.SetMinimumLevel(LogLevel.Trace);
                      logging.AddConsole();
                  });

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment { ApplicationName = "MessagePumpBenchmark" });

            //services.AddFakeMessageProviders();
            //services.AddMessageProducer<byte[], byte[]>("Dest");
            //services.AddMessagePump<byte[], byte[], Handler>("Source");

            var sp = services.BuildServiceProvider();

            consumer = new DelegateConsumer<byte[], byte[]>(() => new Message<byte[], byte[]>
            {
                Key = Encoding.UTF8.GetBytes("123"),
                Value = Encoding.UTF8.GetBytes("hello world")
            });

            var metrics = AppMetrics.CreateDefaultBuilder().Build();

            handler = new Handler();
            handlerName = handler.GetType().Name;
            clock = metrics.Clock;
            successTimer = metrics.HandlerRequestTimer(handlerName);
            failTimer = metrics.HandlerFailRequestTimer(handlerName);
            logger = sp.GetRequiredService<ILogger<MessagePump<byte[], byte[]>>>();
        }

        [Benchmark]
        public async Task MessagePump()
        {
            await FakePump();
        }

        public async Task FakePump()
        {
            try
            {
                var message = await consumer.ConsumeAsync();
                if (message == null)
                    return;

                var activity = new Activity("Messaging.ProcessMessage");
                if (!string.IsNullOrEmpty(message.ActivityId))
                    activity.SetParentId(message.ActivityId);
                activity.Start();

                var start = clock.Nanoseconds;
                try
                {
                    await handler.HandleAsync(message);
                    await consumer.AcceptAsync(message);
                    successTimer.Record(clock.Nanoseconds - start, TimeUnit.Nanoseconds);
                }
                catch (Exception ex)
                {
                    logger.MessageHandlerFailed(handlerName, activity, ex);

                    await consumer.RejectAsync(message);
                    failTimer.Record(clock.Nanoseconds - start, TimeUnit.Nanoseconds);
                }
                finally
                {
                    activity.Stop();
                }
            }
            catch (Exception ex)
            {
                logger.ConsumerFailed(handlerName, Activity.Current, ex);
            }
        }
    }
}
