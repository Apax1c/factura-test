using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Factura.Input
{
    /// <summary>
    /// Adapter over the New Input System pointer. <see cref="Pointer"/> unifies mouse in the
    /// Editor and touch on device.
    /// </summary>
    public sealed class PointerInputService : IInputService
    {
        public bool PressedThisFrame => Pointer.current != null && Pointer.current.press.wasPressedThisFrame;

        public bool IsPressed => Pointer.current != null && Pointer.current.press.isPressed;

        public float HorizontalDelta => Pointer.current == null ? 0f : Pointer.current.delta.ReadValue().x;

        public UniTask WaitForTapAsync(CancellationToken cancellation) =>
            UniTask.WaitUntil(() => PressedThisFrame, cancellationToken: cancellation);
    }
}
