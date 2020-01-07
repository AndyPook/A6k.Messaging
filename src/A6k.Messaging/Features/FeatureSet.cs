using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace A6k.Messaging.Features
{
    public abstract class FeatureSet : IFeatureSet
    {
        private List<(Type Type, Func<object, object> Get, Action<object, object> Set)> features = new List<(Type Type, Func<object, object> Get, Action<object, object> Set)>();

        public FeatureSet()
        {
            foreach (var pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var getter = GetGetMethod(pi);
                var setter = GetSetMethod(pi);
                features.Add((pi.PropertyType, getter, setter));
            }
        }

        public IServiceProvider ServiceProvider { get; private set; }
        public StateFeature State { get; private set; }

        public T Get<T>()
        {
            var f = features.FirstOrDefault(x => x.Type == typeof(T));
            if (f.Type == null)
                return default;
            return (T)f.Get(this);
        }

        public void Set<T>(T feature) where T : class
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            switch (feature)
            {
                case ServiceProviderFeature sp:
                    ServiceProvider = sp.ServiceProvider;
                    break;

                case StateFeature sf:
                    State = sf;
                    break;

                case IRequiresState rs:
                    rs.SetState(State);
                    break;

                default:
                    var f = features.FirstOrDefault(x => x.Type.IsAssignableFrom(typeof(T)));
                    if (f.Type == null)
                        throw new ArgumentException($"Feature {typeof(T).Name} not registered");

                    var prop = (T)f.Get(this);
                    if (prop == null)
                        f.Set(this, feature);
                    else if (prop is CompositeFeature<T> composite)
                        composite.Add(feature);
                    else
                        throw new InvalidOperationException($"Feature already set: {typeof(T).Name}");

                    break;
            }
        }

        private static Action<object, object> GetSetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            UnaryExpression instanceCast;
            if (property.DeclaringType.IsValueType)
                instanceCast = Expression.Convert(instance, property.DeclaringType);
            else
                instanceCast = Expression.TypeAs(instance, property.DeclaringType);

            UnaryExpression valueCast;
            if (property.PropertyType.IsValueType)
                valueCast = Expression.Convert(value, property.PropertyType);
            else
                valueCast = Expression.TypeAs(value, property.PropertyType);

            var call = Expression.Call(instanceCast, property.GetSetMethod(true), valueCast);

            return Expression.Lambda<Action<object, object>>(call, new[] { instance, value }).Compile();
        }

        private static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instanceCast;
            if (property.DeclaringType.IsValueType)
                instanceCast = Expression.Convert(instance, property.DeclaringType);
            else
                instanceCast = Expression.TypeAs(instance, property.DeclaringType);

            var call = Expression.Call(instanceCast, property.GetGetMethod());
            var typeAs = Expression.TypeAs(call, typeof(object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
    }
}
