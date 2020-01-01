using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using App.Metrics;
using App.Metrics.Infrastructure;
using App.Metrics.Internal.NoOp;
using App.Metrics.Timer;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using A6k.Messaging.Features;
using A6k.Messaging.Internal;

namespace A6k.Messaging
{
    /// <summary>
    /// Drives the message loop for a Consumer/Handler
    /// </summary>
    /// <typeparam name="TKey">Message Key type</typeparam>
    /// <typeparam name="TValue">Message Value type</typeparam>
    public class MessagePump<TKey, TValue> : BackgroundService, IHasFeatures
    {
        public const string ProcessActivityName = "Game.Eventing.ProcessMessage";

        private readonly Func<IMessageConsumer<TKey, TValue>> consumerFactory;
        private readonly IMessageHandler<TKey, TValue> handler;
        //TODO: private readonly ServicePauseUtil pauseUtil;
        private readonly IEnumerable<WaitToken> waitTokens;
        private readonly string handlerName;
        private readonly ILogger<MessagePump<TKey, TValue>> logger;

        private readonly ITimer successTimer;
        private readonly ITimer failTimer;
        private readonly IClock clock;

        public MessagePump(
            Func<IMessageConsumer<TKey, TValue>> consumerFactory,
            IMessageHandler<TKey, TValue> handler,
            //ServicePauseUtil pauseUtil = null,
            IEnumerable<WaitToken> waitTokens = null,
            ILogger<MessagePump<TKey, TValue>> logger = null,
            IMetrics metrics = null)
        {
            this.consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            //this.pauseUtil = pauseUtil ?? new ServicePauseUtil();
            this.waitTokens = waitTokens;
            this.logger = logger ?? NullLogger<MessagePump<TKey, TValue>>.Instance;

            handlerName = GetHandlerName();
            clock = metrics?.Clock ?? new StopwatchClock();
            successTimer = metrics?.HandlerRequestTimer(handlerName) ?? new NullTimer();
            failTimer = metrics?.HandlerFailRequestTimer(handlerName) ?? new NullTimer();
        }

        public string Name { get; set; }

        private readonly FeaturesCollection features = new FeaturesCollection();
        public IFeatureCollection Features => features;

        public void Configure(Action<IFeatureCollection> configureFeatures = null) => configureFeatures?.Invoke(features);

        private string GetHandlerName()
        {
            if (handler is IMessageHandlerName n && !string.IsNullOrEmpty(n.Name))
                return n.Name;

            return handler.GetType().Name;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // yielding here allows the BackgroundService.Start to capture this as a Task and not block other processing
            // some impl of Consumer never release the context, so we just get stuck in the while loop
            await Task.Yield();
            // If configured to wait for other parts before processing (ie IMessagePumpWaitFeature) then wait for that set of tokens
            await WaitForOthers(stoppingToken);

            logger.MessagePumpStarted(Name, handlerName);
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: await pauseUtil.Wait(stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                    break;

                await RunMessagePump(stoppingToken);
            }
            logger.MessagePumpStopped(Name, handlerName);
        }

        private async Task RunMessagePump(CancellationToken stoppingToken)
        {
            var consumer = GetConsumer();

            using (consumer)
            {
                // main message loop
                while (!stoppingToken.IsCancellationRequested)//TODO: && !pauseUtil.IsPaused)
                {
                    try
                    {
                        var message = await consumer.ConsumeAsync();
                        if (message == null)
                            continue;

                        var activity = new Activity(ProcessActivityName);
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

        private IMessageConsumer<TKey, TValue> GetConsumer()
        {
            try
            {
                return consumerFactory();
            }
            catch (OptionsValidationException oex)
            {
                logger.LogCritical(oex, "Config Error: {Name}:{OptionsName} {Failures}", oex.OptionsType.Name, oex.OptionsName, string.Join("; ", oex.Failures));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, ex.Message);
                throw;
            }
        }

        private async Task WaitForOthers(CancellationToken cancellationToken)
        {
            // If a IMessagePumpWaitFeature is configured then wait for that set of tokens
            var waitForFeature = features.MessagePumpWaitFeature;
            if (waitForFeature == null || waitForFeature.Names.Length == 0)
                return;
            if (waitTokens == null)
                throw new InvalidOperationException("No WaitTokens defined");

            logger.MessagePumpWaiting(Name, handlerName, waitForFeature.Names);

            var start = DateTime.UtcNow;
            await waitTokens.WhenAll(cancellationToken, waitForFeature.Names);
            var duration = DateTime.UtcNow - start;

            logger.MessagePumpResumed(Name, handlerName, duration, waitForFeature.Names);
        }

        /// <summary>
        /// Specialised <see cref="IFeatureCollection"/> for the pump
        /// </summary>
        private partial class FeaturesCollection : FeatureCollectionBase
        {
            private IMessagePumpWaitFeature messagePumpWaitFeature;
            public IMessagePumpWaitFeature MessagePumpWaitFeature => messagePumpWaitFeature;

            public override TFeature Get<TFeature>()
            {
                return TryGet<TFeature>(
                    messagePumpWaitFeature
                );
            }

            public override void Set<TFeature>(TFeature feature)
            {
                base.Set(feature);

                TrySet(feature, ref messagePumpWaitFeature);
            }
        }
    }
}