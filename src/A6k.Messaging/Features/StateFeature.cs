using System;
using System.Collections.Generic;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// A feature used to hold a key/value bag for a consumer or producer.
    /// Useful for holding flags/values across consumer stop/start
    /// </summary>
    public class StateFeature : IFeature
    {
        public StateFeature(Dictionary<object, object> state)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public Dictionary<object, object> State { get; }

        public T Get<T>(object key)
        {
            if (State == null)
                return default;

            if (State.TryGetValue(key, out var v) && v is T val)
                return val;

            return default;
        }

        public void Set(object key, object value = null)
        {
            if (State == null)
                return;
            State[key] = value ?? true;
        }

        public bool IsSet(object key) => State.TryGetValue(key, out _);
    }
}
