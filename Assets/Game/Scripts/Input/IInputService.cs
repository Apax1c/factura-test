using System.Threading;
using Cysharp.Threading.Tasks;

namespace Factura.Input
{
    /// <summary>
    /// Gameplay-level view of player input. Keeping device APIs behind this seam means
    /// aiming and the round flow can be driven by a stub in a test or by a different
    /// control scheme without either of them changing.
    /// </summary>
    public interface IInputService
    {
        bool PressedThisFrame { get; }

        bool IsPressed { get; }

        float HorizontalDelta { get; }

        UniTask WaitForTapAsync(CancellationToken cancellation);
    }
}
