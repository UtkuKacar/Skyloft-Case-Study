using System;
using Skyloft.Enemy;
using UnityEngine;

namespace Skyloft.Player
{
    /// <summary>Nearest-target acquisition and fire timing; direct damage is isolated in Fire.</summary>
    [DefaultExecutionOrder(-50)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHealth), typeof(PlayerMovement))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        private const float AimAlignment = 0.9781476f; // Within 12 degrees before firing.
        [SerializeField, Min(0.1f)] private float attackRange = 8f;
        [SerializeField, Min(0f)] private float damage = 15f;
        [SerializeField, Min(0.05f)] private float fireInterval = 0.6f;
        [SerializeField] private RifleFireFeedback fireFeedback;

        private PlayerHealth health;
        private PlayerMovement movement;
        private Transform cachedTransform;
        private EnemyHealth target;
        private float nextFireTime;

        public float AttackRange => attackRange;
        public EnemyHealth CurrentTarget => IsValid(target) ? target : null;
        public event Action ShotFired;

        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<PlayerMovement>();
            cachedTransform = transform;
        }

        private void Update()
        {
            if (health.IsDead)
            {
                ClearTarget();
                return;
            }

            target = FindNearest();
            movement.SetFacingTarget(target != null ? target.CachedTransform : null);
            if (target == null || Time.time < nextFireTime)
                return;

            Vector3 direction = target.CachedTransform.position - cachedTransform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f &&
                Vector3.Dot(cachedTransform.forward, direction.normalized) < AimAlignment)
                return;

            nextFireTime = Time.time + fireInterval;
            Fire(target);
        }

        private EnemyHealth FindNearest()
        {
            EnemyHealth nearest = null;
            float bestDistance = attackRange * attackRange;
            Vector3 origin = cachedTransform.position;
            for (int i = 0; i < EnemyRegistry.Count; i++)
            {
                EnemyHealth candidate = EnemyRegistry.GetAt(i);
                if (candidate == null || !candidate.IsTargetable)
                    continue;
                float squaredDistance = (candidate.CachedTransform.position - origin).sqrMagnitude;
                if (squaredDistance <= bestDistance)
                {
                    bestDistance = squaredDistance;
                    nearest = candidate;
                }
            }
            return nearest;
        }

        private bool IsValid(EnemyHealth candidate)
        {
            return candidate != null && candidate.IsTargetable && cachedTransform != null &&
                (candidate.CachedTransform.position - cachedTransform.position).sqrMagnitude <= attackRange * attackRange;
        }

        private void Fire(EnemyHealth victim)
        {
            if (!IsValid(victim))
                return;
            Vector3 hitPosition = victim.CachedTransform.position + Vector3.up;
            if (fireFeedback != null)
                fireFeedback.Play(hitPosition);
            victim.TakeDamage(damage);
            ShotFired?.Invoke();
            if (!victim.IsTargetable)
                ClearTarget();
        }

        private void ClearTarget()
        {
            target = null;
            if (movement != null)
                movement.SetFacingTarget(null);
        }

        private void OnDisable() => ClearTarget();

        private void OnValidate()
        {
            attackRange = Mathf.Max(0.1f, attackRange);
            damage = Mathf.Max(0f, damage);
            fireInterval = Mathf.Max(0.05f, fireInterval);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
#endif
    }
}
