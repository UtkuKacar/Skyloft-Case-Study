using UnityEngine;

namespace Skyloft.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHealth), typeof(PlayerMovement), typeof(PlayerCombat))]
    public sealed class PlayerDeath : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private PlayerHealth health;
        private PlayerMovement movement;
        private PlayerCombat combat;
        private PlayerAnimationController animationController;
        private Animator animator;

        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
            animationController = GetComponent<PlayerAnimationController>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            health.Died += StopPlayer;
            if (health.IsDead)
                StopPlayer();
        }

        private void OnDisable() => health.Died -= StopPlayer;

        private void StopPlayer()
        {
            combat.enabled = false;
            movement.enabled = false;
            if (animationController != null)
                animationController.enabled = false;
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0f);
                animator.SetBool(IsDeadHash, true);
            }
        }
    }
}
