using Skyloft.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Skyloft.UI
{
    public sealed class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth health;
        [SerializeField] private Image fill;
        [SerializeField] private Text label;

        public float DisplayedFraction { get; private set; }

        private void OnEnable()
        {
            if (health == null)
                return;
            health.HealthChanged += Refresh;
            Refresh(health.CurrentHealth, health.MaxHealth);
        }

        private void OnDisable()
        {
            if (health != null)
                health.HealthChanged -= Refresh;
        }

        private void Refresh(float current, float maximum)
        {
            DisplayedFraction = maximum > 0f ? Mathf.Clamp01(current / maximum) : 0f;
            if (fill != null)
            {
                fill.rectTransform.anchorMax = new Vector2(DisplayedFraction, 1f);
                fill.color = Color.Lerp(new Color(0.9f, 0.15f, 0.15f), new Color(0.12f, 0.85f, 0.5f), DisplayedFraction);
            }
            if (label != null)
                label.text = "HP  " + Mathf.CeilToInt(current) + " / " + Mathf.CeilToInt(maximum);
        }
    }
}
