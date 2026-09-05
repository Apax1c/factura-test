using System;
using System.Collections.Generic;

namespace Factura.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Publish<T>(T message) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out Delegate stored) && stored is Action<T> handler)
                handler.Invoke(message);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            Type key = typeof(T);
            _handlers[key] = _handlers.TryGetValue(key, out Delegate stored)
                ? Delegate.Combine(stored, handler)
                : handler;

            return new Subscription(() => Unsubscribe(handler));
        }

        private void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            Type key = typeof(T);
            if (!_handlers.TryGetValue(key, out Delegate stored))
                return;

            Delegate remaining = Delegate.Remove(stored, handler);
            if (remaining == null)
                _handlers.Remove(key);
            else
                _handlers[key] = remaining;
        }

        private sealed class Subscription : IDisposable
        {
            private Action _unsubscribe;

            public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;

            public void Dispose()
            {
                _unsubscribe?.Invoke();
                _unsubscribe = null;
            }
        }
    }
}
