﻿using System;
using Confluent.Kafka;
using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public class ConsumerFactoryFeature
    {
        public static ConsumerFactoryFeature<TKey, TValue> Create<TKey, TValue>(Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> consumerFactory)
        {
            return new ConsumerFactoryFeature<TKey, TValue>(consumerFactory);
        }
    }
    
    public class ConsumerFactoryFeature<TKey, TValue> : IFeature
    {
        public ConsumerFactoryFeature(Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> consumerFactory)
        {
            ConsumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
        }

        public Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> ConsumerFactory { get; }
    }
}
