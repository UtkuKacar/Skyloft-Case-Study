using UnityEngine;

namespace Skyloft.Player
{
    /// <summary>
    /// Contract for providing movement input on the 2D ground plane (XZ).
    /// Decouples movement physics from the concrete input hardware/UI.
    /// </summary>
    public interface IMovementInput
    {
        Vector2 MoveInput { get; }
        bool HasInput { get; }
    }
}
