using System;
using Factura.Events;

namespace Factura.Core
{
    /// <summary>Counts neutralised enemies, which is the score the brief asks the player to maximise.</summary>
    public sealed class ScoreService : IResettable, IDisposable
    {
        private readonly IDisposable _subscription;

        public int Kills { get; private set; }

        public event Action<int> KillsChanged;

        public ScoreService(IEventBus events) =>
            _subscription = events.Subscribe<EnemyKilled>(_ => RegisterKill());

        public void ResetState()
        {
            Kills = 0;
            KillsChanged?.Invoke(Kills);
        }

        public void Dispose() => _subscription.Dispose();

        private void RegisterKill()
        {
            Kills++;
            KillsChanged?.Invoke(Kills);
        }
    }
}
