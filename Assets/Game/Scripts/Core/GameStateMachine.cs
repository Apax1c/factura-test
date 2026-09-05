using System;

namespace Factura.Core
{
    /// <summary>
    /// Single source of truth for the round state.
    /// Subsystems react to <see cref="StateChanged"/> instead of referencing each other,
    /// so a new subsystem can join the round lifecycle without the game flow knowing about it.
    /// </summary>
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; } = GameState.Ready;

        public event Action<GameState> StateChanged;

        public bool IsPlaying => Current == GameState.Playing;

        public void Set(GameState next)
        {
            if (Current == next)
                return;

            Current = next;
            StateChanged?.Invoke(next);
        }
    }
}
