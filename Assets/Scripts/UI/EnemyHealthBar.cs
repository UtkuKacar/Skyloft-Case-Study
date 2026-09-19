using System.Collections.Generic;
using Skyloft.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace Skyloft.UI
{
    /// <summary>Health changes drive fill updates; one camera component handles all facing.</summary>
    public sealed class EnemyHealthBar : MonoBehaviour
    {
        private static readonly List<EnemyHealthBar> VisibleBars = new List<EnemyHealthBar>(64);
        [SerializeField] private EnemyHealth health;
        [SerializeField] private Image fill;
        private Transform cachedTransform;

        public float DisplayedFraction { get; private set; }

        private void Awake() => cachedTransform = transform;

        private void OnEnable()
        {
            if (!VisibleBars.Contains(this))
                VisibleBars.Add(this);
            if (health != null)
            {
                health.HealthChanged += Refresh;
                Refresh(health.CurrentHealth, health.MaxHealth);
            }
        }

        private void OnDisable()
        {
            VisibleBars.Remove(this);
            if (health != null)
                health.HealthChanged -= Refresh;
        }

        private void Refresh(float current, float maximum)
        {
            DisplayedFraction = maximum > 0f ? Mathf.Clamp01(current / maximum) : 0f;
            if (fill != null)
                fill.rectTransform.anchorMax = new Vector2(DisplayedFraction, 1f);
        }

        internal static void FaceCamera(Quaternion rotation)
        {
            for (int i = 0; i < VisibleBars.Count; i++)
            {
                var bar = VisibleBars[i];
                if (bar != null && bar.cachedTransform != null)
                    bar.cachedTransform.rotation = rotation;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRegistry() => VisibleBars.Clear();
    }
}
