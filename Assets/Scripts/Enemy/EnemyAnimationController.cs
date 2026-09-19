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

        [SerializeField] private Animator animator;
        private EnemyMovement movement;
        private EnemyAttack attack;

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
                bool isAttacking = attack != null && attack.isActiveAndEnabled && attack.InAttackRange;
                animator.SetBool(IsAttackingHash, isAttacking);
                animator.SetFloat(SpeedHash,
                    movementEnabled && movement.IsMoving && !isAttacking ? 1f : 0f);
            }
        }
    }
}
