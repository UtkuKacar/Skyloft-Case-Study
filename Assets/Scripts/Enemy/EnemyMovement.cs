using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>Direct arena steering toward an explicitly assigned target.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class EnemyMovement : MonoBehaviour
    {
        private const float Gravity = 20f;
        private const float GroundingSpeed = 0.5f;
        private const float TargetDistanceTolerance = 0.01f;

        [SerializeField] private Transform target;
        [SerializeField, Min(0f)] private float movementSpeed = 2f;
        [SerializeField, Min(0f)] private float rotationSpeed = 360f;
        [SerializeField, Min(0.01f)] private float stoppingDistance = 0.9f;

        private CharacterController characterController;
        private float verticalVelocity;
        private bool movementHeld;
        private Transform facingTarget;
        private float approachDistance = -1f;

        public Transform Target => target;
        public float MovementSpeed => movementSpeed;
        public bool IsMoving { get; private set; }

        public bool HasReachedTarget
        {
            get
            {
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    return false;
                }

                Vector3 offset = target.position - transform.position;
                offset.y = 0f;
                float arrivalDistance = stoppingDistance + TargetDistanceTolerance;
                return offset.sqrMagnitude <= arrivalDistance * arrivalDistance;
            }
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        // An external behaviour may request a stop/facing direction; movement remains the sole transform owner.
        public void SetMovementConstraint(bool holdPosition, Transform lookTarget)
        {
            movementHeld = holdPosition;
            facingTarget = lookTarget;
        }

        public void SetApproachDistance(float distance) => approachDistance = Mathf.Max(0f, distance);
        public void ClearApproachDistance() => approachDistance = -1f;

        private void Update()
        {
            IsMoving = false;
            if (!characterController.enabled)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            Vector3 displacement = Vector3.zero;
            Vector3 moveDirection = Vector3.zero;
            if (!movementHeld && target != null && target.gameObject.activeInHierarchy && movementSpeed > 0f)
            {
                Vector3 direction = target.position - transform.position;
                direction.y = 0f;
                float stopDistance = approachDistance >= 0f ? approachDistance : stoppingDistance;
                if (direction.sqrMagnitude > stopDistance * stopDistance)
                {
                    float distance = direction.magnitude;
                    direction /= distance;
                    float step = Mathf.Min(movementSpeed * deltaTime, distance - stopDistance);
                    displacement = direction * step;
                    IsMoving = step > 0f;
                    moveDirection = direction;
                }
            }

            Vector3 lookDirection = facingTarget != null
                ? facingTarget.position - transform.position : moveDirection;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(lookDirection), rotationSpeed * deltaTime);

            verticalVelocity = characterController.isGrounded
                ? -GroundingSpeed
                : verticalVelocity - Gravity * deltaTime;
            displacement.y = verticalVelocity * deltaTime;
            characterController.Move(displacement);
        }

        private void OnDisable()
        {
            IsMoving = false;
            verticalVelocity = 0f;
        }

        private void OnValidate()
        {
            movementSpeed = Mathf.Max(0f, movementSpeed);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            stoppingDistance = Mathf.Max(0.01f, stoppingDistance);
        }
    }
}
