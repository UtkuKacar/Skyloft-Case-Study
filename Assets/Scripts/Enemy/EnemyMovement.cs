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
        [SerializeField, Min(0f)] private float movementSpeed = 6f;
        [SerializeField, Min(0f)] private float rotationSpeed = 360f;
        [SerializeField, Min(0.01f)] private float stoppingDistance = 0.9f;
        [Header("Lightweight Obstacle Avoidance")]
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField, Min(0.1f)] private float obstacleProbeDistance = 1.4f;
        [SerializeField, Min(0.1f)] private float avoidanceCommitDuration = 0.75f;

        private CharacterController characterController;
        private float verticalVelocity;
        private bool movementHeld;
        private Transform facingTarget;
        private float approachDistance = -1f;
        private Vector3 committedDirection;
        private float lungeDistanceRemaining;
        private float lungeTimeRemaining;
        private float lungeSpeed;
        private Vector3 avoidanceDirection;
        private float avoidanceUntil;

        public Transform Target => target;
        public float MovementSpeed => movementSpeed;
        public float StoppingDistance => stoppingDistance;
        private float EffectiveStoppingDistance => approachDistance >= 0f ? approachDistance : stoppingDistance;
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
                float arrivalDistance = EffectiveStoppingDistance + TargetDistanceTolerance;
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

        public void SetMovementSpeed(float value)
        {
            movementSpeed = float.IsNaN(value) || float.IsInfinity(value) ? 0f : Mathf.Max(0f, value);
        }

        // An external behaviour may request a stop/facing direction; movement remains the sole transform owner.
        public void SetMovementConstraint(bool holdPosition, Transform lookTarget)
        {
            movementHeld = holdPosition;
            facingTarget = lookTarget;
        }

        // One fixed-direction request; CharacterController movement stays owned here.
        public void BeginAttackLunge(Vector3 direction, float distance, float duration)
        {
            direction.y = 0f;
            committedDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
            movementHeld = true;
            facingTarget = null;
            lungeDistanceRemaining = Mathf.Max(0f, distance);
            lungeTimeRemaining = Mathf.Max(0.01f, duration);
            lungeSpeed = lungeDistanceRemaining / lungeTimeRemaining;
            transform.rotation = Quaternion.LookRotation(committedDirection);
        }

        public void StopAttackLunge()
        {
            lungeDistanceRemaining = 0f;
            lungeTimeRemaining = 0f;
        }

        public void ClearAttackCommitment()
        {
            StopAttackLunge();
            committedDirection = Vector3.zero;
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
            if (movementHeld && lungeTimeRemaining > 0f)
            {
                float stepTime = Mathf.Min(deltaTime, lungeTimeRemaining);
                float step = Mathf.Min(lungeDistanceRemaining, lungeSpeed * stepTime);
                displacement = committedDirection * step;
                lungeDistanceRemaining -= step;
                lungeTimeRemaining -= stepTime;
            }
            else if (!movementHeld && target != null && target.gameObject.activeInHierarchy && movementSpeed > 0f)
            {
                Vector3 direction = target.position - transform.position;
                direction.y = 0f;
                float stopDistance = EffectiveStoppingDistance;
                if (direction.sqrMagnitude > stopDistance * stopDistance)
                {
                    float distance = direction.magnitude;
                    direction /= distance;
                    direction = GetSteeringDirection(direction);
                    float step = Mathf.Min(movementSpeed * deltaTime, distance - stopDistance);
                    displacement = direction * step;
                    IsMoving = step > 0f;
                    moveDirection = direction;
                }
            }

            Vector3 lookDirection = committedDirection.sqrMagnitude > 0f ? committedDirection : facingTarget != null
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

        private Vector3 GetSteeringDirection(Vector3 directDirection)
        {
            if (obstacleMask.value == 0)
                return directDirection;

            Vector3 origin = transform.position +
                Vector3.up * Mathf.Min(characterController.height * 0.5f, 0.75f);
            float probeRadius = Mathf.Max(0.05f, characterController.radius * 0.65f);
            bool directBlocked = Physics.SphereCast(origin, probeRadius, directDirection,
                out RaycastHit directHit, obstacleProbeDistance, obstacleMask,
                QueryTriggerInteraction.Ignore);

            if (avoidanceDirection.sqrMagnitude > 0.0001f)
            {
                if (!directBlocked && Time.time >= avoidanceUntil)
                {
                    avoidanceDirection = Vector3.zero;
                    return directDirection;
                }
                if (Physics.SphereCast(origin, probeRadius, avoidanceDirection,
                    out RaycastHit followHit, obstacleProbeDistance, obstacleMask,
                    QueryTriggerInteraction.Ignore))
                {
                    avoidanceDirection = ChooseWallDirection(
                        origin, probeRadius, followHit.normal, directDirection);
                    avoidanceUntil = Time.time + avoidanceCommitDuration;
                }
                return avoidanceDirection;
            }

            if (!directBlocked)
                return directDirection;

            avoidanceDirection = ChooseWallDirection(
                origin, probeRadius, directHit.normal, directDirection);
            avoidanceUntil = Time.time + avoidanceCommitDuration;
            return avoidanceDirection;
        }

        private Vector3 ChooseWallDirection(
            Vector3 origin, float radius, Vector3 wallNormal, Vector3 directDirection)
        {
            Vector3 first = Vector3.Cross(Vector3.up, wallNormal).normalized;
            Vector3 second = -first;
            float firstScore = GetClearance(origin, radius, first) +
                Mathf.Max(0f, Vector3.Dot(first, directDirection)) * obstacleProbeDistance;
            float secondScore = GetClearance(origin, radius, second) +
                Mathf.Max(0f, Vector3.Dot(second, directDirection)) * obstacleProbeDistance;
            return firstScore >= secondScore ? first : second;
        }

        private float GetClearance(Vector3 origin, float radius, Vector3 direction)
        {
            float distance = obstacleProbeDistance * 6f;
            return Physics.SphereCast(origin, radius, direction, out RaycastHit hit,
                distance, obstacleMask, QueryTriggerInteraction.Ignore)
                ? hit.distance : distance;
        }

        private void OnDisable()
        {
            ClearAttackCommitment();
            avoidanceDirection = Vector3.zero;
            IsMoving = false;
            verticalVelocity = 0f;
        }

        private void OnValidate()
        {
            movementSpeed = Mathf.Max(0f, movementSpeed);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            stoppingDistance = Mathf.Max(0.01f, stoppingDistance);
            obstacleProbeDistance = Mathf.Max(0.1f, obstacleProbeDistance);
            avoidanceCommitDuration = Mathf.Max(0.1f, avoidanceCommitDuration);
        }
    }
}
