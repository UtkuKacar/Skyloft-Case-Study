using UnityEngine;

namespace Skyloft.Player
{
    /// <summary>
    /// Synchronizes player movement state with the Animator.
    /// Strictly adheres to Single Responsibility Principle (SRP): only manages animation parameter updates.
    /// Employs cached parameter hashes and zero-allocation updates to ensure peak mobile performance.
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        [Header("Dependencies")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerInput playerInput;

        [Header("Settings")]
        [Tooltip("Smoothing speed for the Speed parameter to prevent abrupt transitions.")]
        [SerializeField] private float speedDampTime = 12f;

        [Tooltip("Input threshold below which movement is treated as strict zero (idle).")]
        [SerializeField] private float inputDeadZoneThreshold = 0.05f;

        private float _currentSpeed;
        private IMovementInput _movementInput;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (playerInput == null)
            {
                playerInput = GetComponentInParent<PlayerInput>();
                if (playerInput == null)
                {
                    playerInput = GetComponent<PlayerInput>();
                }
            }

            _movementInput = playerInput;
        }

        private void Update()
        {
            if (animator == null)
            {
                return;
            }

            float targetSpeed = 0f;

            if (_movementInput != null && _movementInput.HasInput)
            {
                float mag = _movementInput.MoveInput.magnitude;
                if (mag >= inputDeadZoneThreshold)
                {
                    targetSpeed = Mathf.Clamp01(mag);
                }
            }

            // Smoothly interpolate current speed to eliminate flickering around joystick threshold
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, speedDampTime * Time.deltaTime);

            animator.SetFloat(SpeedHash, _currentSpeed);
        }

        public void SetMovementInput(IMovementInput movementInput)
        {
            _movementInput = movementInput;
        }
    }
}
