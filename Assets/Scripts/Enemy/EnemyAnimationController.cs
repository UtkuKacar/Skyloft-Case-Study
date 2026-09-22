using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>Drives locomotion and the visual attack pose; does not apply damage.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMovement))]
    public sealed class EnemyAnimationController : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int AttackStateHash = Animator.StringToHash("Base Layer.Zombie Attack");

        [SerializeField] private Animator animator;
        private EnemyMovement movement;
        private EnemyAttack attack;

        // Wait for the previous state's exit blend; outgoing events must never belong to a new attack.
        public bool CanStartAttack => animator != null && animator.isActiveAndEnabled &&
            !animator.IsInTransition(0) &&
            animator.GetCurrentAnimatorStateInfo(0).fullPathHash != AttackStateHash;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            attack = GetComponent<EnemyAttack>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        private void Update()
        {
            if (animator != null)
            {
                bool movementEnabled = movement.isActiveAndEnabled;
                bool isAttacking = attack != null && attack.isActiveAndEnabled && attack.IsAttacking;
                animator.SetBool(IsAttackingHash, isAttacking);
                animator.SetFloat(SpeedHash,
                    movementEnabled && movement.IsMoving && !isAttacking ? 1f : 0f);
            }
        }
    }
}
