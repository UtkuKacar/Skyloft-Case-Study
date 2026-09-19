using UnityEngine;

namespace Skyloft.Core
{
    /// <summary>
    /// Lightweight camera follower providing a smooth top-down isometric view of the player.
    /// Operates in LateUpdate to avoid camera jitter after movement calculations.
    /// </summary>
    public class SimpleCameraFollow : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform target;

        [Header("Positioning")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 9.5f, -6.5f);
        [SerializeField] private float smoothSpeed = 10f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
