using Skyloft.Player;
using UnityEngine;

namespace Skyloft.Enemy
{
    [DefaultExecutionOrder(-40)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMovement), typeof(EnemyHealth))]
    public sealed class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private PlayerHealth targetHealth;
        [SerializeField, Min(0.1f)] private float attackRange = 1.2f;
        [SerializeField, Min(0f)] private float damage = 10f;
        [SerializeField, Min(0.05f)] private float attackInterval = 0.5f;

        private EnemyMovement movement;
        private EnemyHealth health;
        private float nextAttackTime;

        public float AttackRange => attackRange;
        public bool InAttackRange { get; private set; }
        public PlayerHealth Target => targetHealth;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            health = GetComponent<EnemyHealth>();
            if (targetHealth == null && movement.Target != null)
                targetHealth = movement.Target.GetComponent<PlayerHealth>();
        }

        private void OnEnable() => nextAttackTime = Time.time + attackInterval;

        public void SetTarget(PlayerHealth newTarget)
        {
            targetHealth = newTarget;
            movement.SetTarget(newTarget != null ? newTarget.transform : null);
            nextAttackTime = Time.time + attackInterval;
        }

        private void Update()
        {
            InAttackRange = false;
            if (health.IsDead || !movement.isActiveAndEnabled || targetHealth == null ||
                !targetHealth.isActiveAndEnabled || targetHealth.IsDead)
            {
                movement.SetMovementConstraint(true, null);
                return;
            }

            // This behaviour chooses the approach distance; EnemyMovement applies all translation/rotation.
            movement.SetApproachDistance(attackRange * 0.95f);
            Vector3 offset = targetHealth.transform.position - transform.position;
            offset.y = 0f;
            InAttackRange = offset.sqrMagnitude <= attackRange * attackRange;
            movement.SetMovementConstraint(InAttackRange, InAttackRange ? targetHealth.transform : null);
            if (!InAttackRange)
            {
                nextAttackTime = Time.time + attackInterval;
                return;
            }

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackInterval;
                targetHealth.TakeDamage(damage);
            }
        }

        private void OnDisable()
        {
            InAttackRange = false;
            if (movement != null)
            {
                movement.SetMovementConstraint(false, null);
                movement.ClearApproachDistance();
            }
        }

        private void OnValidate()
        {
            attackRange = Mathf.Max(0.1f, attackRange);
            damage = Mathf.Max(0f, damage);
            attackInterval = Mathf.Max(0.05f, attackInterval);
        }
    }
}
