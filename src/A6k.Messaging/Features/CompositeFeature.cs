using System;
using System.Collections.Generic;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// Helper to allow a set of features to be composed together
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CompositeFeature<T> where T : IFeature
    {
        private LinkedList<T> features;

        public int Count => features?.Count ?? 0;
        public bool IsEmpty => features == null;

        public void Add(T item)
        {
            if (features == null)
                features = new LinkedList<T>();
            features.AddLast(item);
        }

        public bool Invoke(Action<T> action)
        {
            if (features == null)
                return false;

            for (var f = features.First; f != null; f = f.Next)
                action(f.Value);

            return true;
        }
    }
}
