using UnityEngine;

namespace Skyloft.Player
{
    /// <summary>
    /// Handles player movement and rotation on the XZ plane using CharacterController.
    /// Strictly separated from input gathering and UI presentation.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [Tooltip("Horizontal movement speed in meters per second.")]
        [SerializeField] private float movementSpeed = 6f;

        [Tooltip("Rotation speed in degrees per second.")]
        [SerializeField] private float rotationSpeed = 720f;

        [Tooltip("Downwards gravity acceleration to ensure stable grounding on uneven surfaces.")]
        [SerializeField] private float gravity = 20f;

        [Header("Input Dependency")]
        [SerializeField] private PlayerInput playerInput;

        private CharacterController characterController;
        private float verticalVelocity = 0f;

        public float MovementSpeed => movementSpeed;
        public bool IsMoving { get; private set; }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }

        private void Update()
        {
            MoveAndRotate();
        }

        private void MoveAndRotate()
        {
            Vector2 input = playerInput != null ? playerInput.MoveInput : Vector2.zero;

            // Map 2D joystick (X, Y) to 3D world ground plane (X, Z)
            Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
            float inputMagnitude = moveDirection.magnitude;

            if (inputMagnitude > 1f)
            {
                moveDirection.Normalize();
                inputMagnitude = 1f;
            }

            IsMoving = inputMagnitude > 0.001f;

            // Rotate towards movement direction when input is active
            if (IsMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            // Downward grounding force
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.5f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            Vector3 horizontalVelocity = moveDirection * (movementSpeed * inputMagnitude);
            Vector3 velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

            characterController.Move(velocity * Time.deltaTime);
        }

        public void SetMovementSpeed(float newSpeed)
        {
            movementSpeed = Mathf.Max(0f, newSpeed);
        }

        private void OnValidate()
        {
            movementSpeed = Mathf.Max(0f, movementSpeed);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            gravity = Mathf.Max(0f, gravity);
        }
    }
}
