using System;
using UnityEngine;

namespace Skyloft.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerHealth : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        public event Action<float, float> HealthChanged;
        public event Action Died;
        private bool damageEnabled = true;
        public void SetDamageEnabled(bool value) => damageEnabled = value;

        private void Awake()
        {
            OnValidate();
            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (!damageEnabled || IsDead || !isActiveAndEnabled || damage <= 0f ||
                float.IsNaN(damage) || float.IsInfinity(damage))
                return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            bool died = CurrentHealth <= 0f;
            if (died)
                IsDead = true;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
            if (died)
                Died?.Invoke();
        }

        private void OnValidate()
        {
            maxHealth = float.IsNaN(maxHealth) || float.IsInfinity(maxHealth)
                ? 100f : Mathf.Max(1f, maxHealth);
        }
    }
}
