using UnityEngine;

namespace Skyloft.UI
{
    /// <summary>A single camera update keeps world bars readable without per-enemy Update methods.</summary>
    [DefaultExecutionOrder(100)]
    public sealed class WorldHealthBarFacing : MonoBehaviour
    {
        private void LateUpdate() => EnemyHealthBar.FaceCamera(transform.rotation);
    }
}
