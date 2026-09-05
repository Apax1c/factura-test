using System;

namespace Factura.Core
{
    /// <summary>Counts neutralised enemies, which is the score the brief asks the player to maximise.</summary>
    public sealed class ScoreService : IResettable
    {
        public int Kills { get; private set; }

        public event Action<int> KillsChanged;

        public void RegisterKill()
        {
            Kills++;
            KillsChanged?.Invoke(Kills);
        }

        public void ResetState()
        {
            Kills = 0;
            KillsChanged?.Invoke(Kills);
        }
    }
}
