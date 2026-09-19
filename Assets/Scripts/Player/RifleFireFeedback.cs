using UnityEngine;

namespace Skyloft.Player
{
    /// <summary>Reuses one short-lived line; no objects or coroutines are created per shot.</summary>
    [DisallowMultipleComponent]
    public sealed class RifleFireFeedback : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private LineRenderer tracer;
        [SerializeField, Min(0.01f)] private float visibleDuration = 0.09f;
        private float hideAt;

        public bool IsVisible => tracer != null && tracer.enabled;

        private void Awake()
        {
            if (tracer != null)
                tracer.enabled = false;
        }

        public void Play(Vector3 hitPosition)
        {
            if (muzzle == null || tracer == null)
                return;
            tracer.SetPosition(0, muzzle.position);
            tracer.SetPosition(1, hitPosition);
            tracer.enabled = true;
            hideAt = Time.time + visibleDuration;
        }

        private void LateUpdate()
        {
            if (tracer == null || !tracer.enabled)
                return;
            if (Time.time >= hideAt)
                tracer.enabled = false;
            else if (muzzle != null)
                tracer.SetPosition(0, muzzle.position);
        }

        private void OnDisable()
        {
            if (tracer != null)
                tracer.enabled = false;
        }
    }
}
