namespace Factura.Core
{
    /// <summary>
    /// Restores a system to its round-start condition without reloading the scene.
    /// The game flow resets every registered implementation as a group, so a new
    /// stateful system joins the restart path simply by being registered in the container.
    /// </summary>
    public interface IResettable
    {
        void ResetState();
    }
}
