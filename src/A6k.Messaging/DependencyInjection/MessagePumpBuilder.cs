using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using A6k.Messaging;
using A6k.Messaging.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Fluent interface for constructing a message consumer
    /// </summary>
    /// <typeparam name="TKey">Key type of the message</typeparam>
    /// <typeparam name="TValue">Value type of the message</typeparam>
    public class MessagePumpBuilder<TKey, TValue>
    {
        private readonly IServiceCollection services;
        private readonly string configName;
        private readonly string providerType;

        private Action<IServiceProvider, CompositeMessageHandler<TKey, TValue>> handlerConfig;
        private Action<IFeatureSet> featureConfig;

        private readonly Dictionary<object, object> state = new Dictionary<object, object>();

        public MessagePumpBuilder(IServiceCollection services, string configName, string providerType = null)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.configName = configName;
            this.providerType = providerType;
            
            Name = configName;
            featureConfig = f => f.Set(new StateFeature(state));
        }

        public string Name { get; set; }

        /// <summary>
        /// Add a handler to process messages
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandler<THandler>()
            where THandler : IMessageHandler<TKey, TValue>
        {
            handlerConfig += (sp, h) =>
            {
                var handler = (IMessageHandler<TKey, TValue>)ActivatorUtilities.CreateInstance<THandler>(sp);
                h.AddHandler(handler);
            };
            return this;
        }

        /// <summary>
        /// Add a handler to process messages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandler(Func<IMessage<TKey, TValue>, Task> handler, string name = null)
        {
            handlerConfig += (_, h) => h.AddHandler(handler, name);
            return this;
        }
        /// <summary>
        /// Add a handler to process messages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandler(Func<IServiceProvider, IMessage<TKey, TValue>, Task> handler, string name = null)
        {
            handlerConfig += (sp, h) => h.AddHandler(m => handler(sp, m), name);
            return this;
        }

        /// <summary>
        /// Add a handler to process messages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandler(Action<IMessage<TKey, TValue>> handler, string name = null)
        {
            handlerConfig = (_, h) => h.AddHandler(handler, name);
            return this;
        }

        /// <summary>
        /// Add a handler to process messages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandler(Action<IServiceProvider, IMessage<TKey, TValue>> handler, string name = null)
        {
            handlerConfig = (sp, h) => h.AddHandler(m => handler(sp, m), name);
            return this;
        }

        /// <summary>
        /// Add handlers to process messages
        /// </summary>
        /// <param name="handlerConfig"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandlers(Action<CompositeMessageHandler<TKey, TValue>> handlerConfig)
        {
            this.handlerConfig += (_, h) => handlerConfig(h);
            return this;
        }

        /// <summary>
        /// Add handlers to process messages
        /// </summary>
        /// <param name="handlerConfig"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithHandlers(Action<IServiceProvider, CompositeMessageHandler<TKey, TValue>> handlerConfig)
        {
            this.handlerConfig += handlerConfig;
            return this;
        }

        /// <summary>
        /// Add Consumer Features
        /// </summary>
        /// <param name="featureConfig"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithFeatures(Action<IFeatureSet> featureConfig)
        {
            this.featureConfig += featureConfig;
            return this;
        }

        /// <summary>
        /// Add a Consumer Feature
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithFeature<TFeature>()
            where TFeature : class
        {
            featureConfig += f => f.Set<TFeature>();
            return this;
        }

        /// <summary>
        /// Add a Consumer Feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public MessagePumpBuilder<TKey, TValue> WithFeature(object feature)
        {
            featureConfig += f => f.Set(feature);
            return this;
        }

        /// <summary>
        /// Build the MessagePump and add to Services
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddMessagePump()
        {
            return services.AddTransient<IHostedService>(sp =>
            {
                var consumerFactory = sp.GetMessageConsumerFactory<TKey, TValue>(configName, providerType, featureConfig);
                var handler = GetHandler(sp);
                var pump = ActivatorUtilities.CreateInstance<MessagePump<TKey, TValue>>(sp, consumerFactory, handler);
                pump.Name = Name;
                pump.Configure(featureConfig);

                return pump;
            });
        }

        private IMessageHandler<TKey, TValue> GetHandler(IServiceProvider sp)
        {
            var handler = new CompositeMessageHandler<TKey, TValue>();
            handlerConfig?.Invoke(sp, handler);
            if (handler.Count == 0)
                throw new ArgumentException("At least one handler must be specified", nameof(handlerConfig));

            // if there is only one, just use that one without the composite facade (the mopre normal case)
            if (handler.Count == 1)
                return handler.First;
            return handler;
        }
    }
}
