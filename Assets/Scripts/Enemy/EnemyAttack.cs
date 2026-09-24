using System;
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
        [Tooltip("Maximum target distance at the animation impact; does not start an attack.")]
        [SerializeField, Min(0.1f)] private float impactRange = 1.5f;
        [Tooltip("Minimum distance inside attack range at which movement may stop.")]
        [SerializeField, Min(0.01f)] private float stoppingBuffer = 0.15f;
        [SerializeField, Min(0f)] private float damage = 10f;
        [SerializeField, Min(0.05f)] private float attackInterval = 0.5f;
        [SerializeField, Min(0f)] private float attackRecoveryDuration = 0.25f;

        [SerializeField, Min(0f)] private float lungeDistance = 0.6f;
        [SerializeField, Min(0.01f)] private float lungeDuration = 0.18f;
        [Tooltip("Strike cone around the direction captured when this attack begins.")]
        [SerializeField, Range(1f, 90f)] private float impactHalfAngle = 60f;

        private Vector3 attackDirection;
        private EnemyMovement movement;
        private EnemyHealth health;
        private EnemyAnimationController animationController;
        private float nextAttackTime;
        private bool impactConsumed;
        private float recoveryUntil;

        public Vector3 AttackDirection => attackDirection;
        public float AttackRange => attackRange;
        public float ImpactRange => impactRange;
        public bool InAttackRange { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsRecovering => IsAttacking && impactConsumed;
        public PlayerHealth Target => targetHealth;
        public event Action<bool> AttackStateChanged;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            health = GetComponent<EnemyHealth>();
            animationController = GetComponent<EnemyAnimationController>();
            if (targetHealth == null && movement.Target != null)
                targetHealth = movement.Target.GetComponent<PlayerHealth>();
        }

        public void SetTarget(PlayerHealth newTarget)
        {
            if (targetHealth != newTarget)
                CancelAttack();
            targetHealth = newTarget;
            movement.SetTarget(newTarget != null ? newTarget.transform : null);
        }

        private void Update()
        {
            InAttackRange = false;
            if (health.IsDead || !movement.isActiveAndEnabled || targetHealth == null ||
                !targetHealth.isActiveAndEnabled || targetHealth.IsDead)
            {
                CancelAttack();
                movement.SetMovementConstraint(true, null);
                return;
            }

            // This behaviour chooses the approach distance; EnemyMovement applies all translation/rotation.
            float approachDistance = Mathf.Min(movement.StoppingDistance,
                Mathf.Max(0.01f, attackRange - stoppingBuffer));
            movement.SetApproachDistance(approachDistance);
            Vector3 offset = targetHealth.transform.position - transform.position;
            offset.y = 0f;
            InAttackRange = offset.sqrMagnitude <= attackRange * attackRange;
            if (IsAttacking)
            {
                if (impactConsumed && Time.time >= recoveryUntil)
                    CancelAttack();
            }

            if (!IsAttacking && InAttackRange && Time.time >= nextAttackTime &&
                animationController != null && animationController.CanStartAttack)
            {
                IsAttacking = true;
                impactConsumed = false;
                nextAttackTime = Time.time + attackInterval;
                attackDirection = offset.sqrMagnitude > 0.0001f ? offset.normalized : transform.forward;
                AttackStateChanged?.Invoke(true);
                movement.BeginAttackLunge(attackDirection, lungeDistance, lungeDuration);
            }

            movement.SetMovementConstraint(IsAttacking,
                !IsAttacking && InAttackRange ? targetHealth.transform : null);
        }

        // Called directly by the Zombie Attack Animation Event on this GameObject's Animator.
        public void OnAttackImpact()
        {
            if (!IsAttacking || impactConsumed || !isActiveAndEnabled ||
                !health.isActiveAndEnabled || health.IsDead)
                return;

            BeginRecovery();
            if (targetHealth == null || !targetHealth.isActiveAndEnabled || targetHealth.IsDead)
                return;

            Vector3 offset = targetHealth.transform.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > impactRange * impactRange)
                return;

            // A sidestep out of the committed strike arc must remain a valid dodge.
            if (offset.sqrMagnitude > 0.0001f && Vector3.Dot(attackDirection, offset.normalized) <
                Mathf.Cos(impactHalfAngle * Mathf.Deg2Rad))
                return;

            targetHealth.TakeDamage(damage);
        }

        private void BeginRecovery()
        {
            impactConsumed = true;
            movement.StopAttackLunge();
            recoveryUntil = Time.time + attackRecoveryDuration;
            nextAttackTime = Mathf.Max(nextAttackTime, Time.time + attackInterval);
        }

        // A missed impact event can never leave an enemy permanently committed.
        public void OnAttackComplete()
        {
            if (IsAttacking && !impactConsumed)
                BeginRecovery();
        }

        private void CancelAttack()
        {
            bool wasAttacking = IsAttacking;
            if (movement != null)
                movement.ClearAttackCommitment();
            IsAttacking = false;
            impactConsumed = true;
            if (wasAttacking)
                AttackStateChanged?.Invoke(false);
        }

        private void OnDisable()
        {
            InAttackRange = false;
            CancelAttack();
            if (movement != null)
            {
                movement.SetMovementConstraint(false, null);
                movement.ClearApproachDistance();
            }
        }

        private void OnValidate()
        {
            attackRange = Mathf.Max(0.1f, attackRange);
            impactRange = Mathf.Max(attackRange, impactRange);
            stoppingBuffer = Mathf.Clamp(stoppingBuffer, 0.01f, attackRange - 0.01f);
            damage = Mathf.Max(0f, damage);
            attackInterval = Mathf.Max(0.05f, attackInterval);
            attackRecoveryDuration = Mathf.Max(0f, attackRecoveryDuration);
            lungeDistance = Mathf.Max(0f, lungeDistance);
            lungeDuration = Mathf.Max(0.01f, lungeDuration);
        }
    }
}
