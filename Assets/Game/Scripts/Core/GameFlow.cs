using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Factura.Input;
using Factura.Level;
using Factura.Player;
using VContainer.Unity;

namespace Factura.Core
{
    /// <summary>
    /// Drives the round lifecycle as one readable async loop rather than a web of callbacks.
    /// It owns only the ordering of a round; each system decides for itself how to react to
    /// the resulting state change.
    /// </summary>
    public sealed class GameFlow : IAsyncStartable
    {
        /// <summary>Keeps the result screen on screen long enough to read before a tap can restart.</summary>
        private const float RESULT_INPUT_LOCK_SECONDS = 0.6f;

        private readonly GameStateMachine _stateMachine;
        private readonly IInputService _input;
        private readonly LevelProgress _levelProgress;
        private readonly CarController _car;
        private readonly IReadOnlyList<IResettable> _resettables;

        public GameFlow(
            GameStateMachine stateMachine,
            IInputService input,
            LevelProgress levelProgress,
            CarController car,
            IReadOnlyList<IResettable> resettables)
        {
            _stateMachine = stateMachine;
            _input = input;
            _levelProgress = levelProgress;
            _car = car;
            _resettables = resettables;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            try
            {
                while (!cancellation.IsCancellationRequested)
                    await PlayRoundAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is how this loop is meant to end: the scope is disposed when play
                // mode stops or the scene unloads. Letting it escape would have VContainer's
                // exception handler report a normal shutdown as an error.
            }
        }

        private async UniTask PlayRoundAsync(CancellationToken cancellation)
        {
            ResetAll();
            _stateMachine.Set(GameState.Ready);

            await _input.WaitForTapAsync(cancellation);
            _stateMachine.Set(GameState.Playing);

            await UniTask.WaitUntil(
                () => _levelProgress.IsFinished || !_car.IsAlive,
                cancellationToken: cancellation);

            _stateMachine.Set(_car.IsAlive ? GameState.Win : GameState.Lose);

            await UniTask.Delay(
                TimeSpan.FromSeconds(RESULT_INPUT_LOCK_SECONDS),
                cancellationToken: cancellation);
            await _input.WaitForTapAsync(cancellation);
        }

        private void ResetAll()
        {
            for (int i = 0; i < _resettables.Count; i++)
                _resettables[i].ResetState();
        }
    }
}
