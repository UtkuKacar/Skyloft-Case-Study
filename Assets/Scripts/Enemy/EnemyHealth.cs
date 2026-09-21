using System;
using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>Owns health and publishes the alive-to-dead transition once.</summary>
    [DisallowMultipleComponent]
    public sealed class EnemyHealth : MonoBehaviour
    {
        private const float DefaultMaxHealth = 100f;
        [SerializeField, Min(1f)] private float maxHealth = DefaultMaxHealth;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsTargetable => isActiveAndEnabled && !IsDead && CurrentHealth > 0f;
        public Transform CachedTransform { get; private set; }
        public event Action<float, float> HealthChanged;
        public event Action Died;
        private bool damageEnabled = true;
        public void SetDamageEnabled(bool value) => damageEnabled = value;

        private void Awake()
        {
            ValidateMaxHealth();
            CachedTransform = transform;
            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void OnEnable() => EnemyRegistry.Register(this);

        private void OnDisable() => EnemyRegistry.Unregister(this);

        public void TakeDamage(float damage)
        {
            if (!damageEnabled || IsDead || !isActiveAndEnabled || damage <= 0f ||
                float.IsNaN(damage) || float.IsInfinity(damage))
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            bool died = CurrentHealth <= 0f;
            if (died)
            {
                IsDead = true;
                EnemyRegistry.Unregister(this);
            }
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
            if (died)
            {
                EnemyRegistry.NotifyDeath(this);
                Died?.Invoke();
            }
        }

        private void OnValidate()
        {
            ValidateMaxHealth();
        }

        private void ValidateMaxHealth()
        {
            maxHealth = float.IsNaN(maxHealth) || float.IsInfinity(maxHealth)
                ? DefaultMaxHealth
                : Mathf.Max(1f, maxHealth);
        }
    }
}
