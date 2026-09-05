using System;

namespace Factura.Events
{
    public interface IEventBus
    {
        void Publish<T>(T message) where T : struct;

        IDisposable Subscribe<T>(Action<T> handler) where T : struct;
    }
}
