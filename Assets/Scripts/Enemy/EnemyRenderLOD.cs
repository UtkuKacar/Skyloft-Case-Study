using Skyloft.Player;
using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>Renders ordinary crowd movement with shared baked poses; combat keeps the original rig.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyAttack), typeof(EnemyHealth), typeof(EnemyMovement))]
    public sealed class EnemyRenderLOD : MonoBehaviour
    {
        private static readonly int AttackStateHash = Animator.StringToHash("Base Layer.Zombie Attack");
        private const float PostAttackBlendGrace = 0.1f;
        private const float MaxPostAttackBlend = 0.35f;

        [SerializeField] private SkinnedMeshRenderer animatedRenderer;
        [SerializeField] private MeshRenderer distantRenderer;
        [SerializeField] private MeshFilter distantMeshFilter;
        [SerializeField] private Mesh[] distantRunPoses;
        [SerializeField, Min(0.05f)] private float poseInterval = 0.09166667f;

        private EnemyAttack attack;
        private EnemyHealth health;
        private EnemyMovement movement;
        private Animator animator;
        private float nextPoseTime;
        private float returnToBakedAt;
        private int poseIndex;
        private bool hasPlayer;
        private bool isBaked;
        private bool returnToBakedPending;

        public bool IsBaked => isBaked;

        private void Awake()
        {
            attack = GetComponent<EnemyAttack>();
            health = GetComponent<EnemyHealth>();
            movement = GetComponent<EnemyMovement>();
            animator = GetComponentInChildren<Animator>(true);
            if (distantRunPoses != null && distantRunPoses.Length > 0)
                poseIndex = (GetInstanceID() & int.MaxValue) % distantRunPoses.Length;
        }

        private void OnEnable()
        {
            attack.AttackStateChanged += OnAttackStateChanged;
            health.Died += OnDied;
            returnToBakedPending = false;
            nextPoseTime = Time.time + poseInterval;
            SetBaked(hasPlayer && health.IsTargetable && !attack.IsAttacking);
        }

        // WaveSpawner assigns the existing Player reference once when it creates the Enemy.
        public void SetPlayer(PlayerHealth player)
        {
            hasPlayer = player != null;
            if (!hasPlayer)
                returnToBakedPending = false;
            SetBaked(hasPlayer && health.IsTargetable && !attack.IsAttacking);
        }

        private void OnAttackStateChanged(bool attacking)
        {
            if (attacking)
            {
                returnToBakedPending = false;
                SetBaked(false);
            }
            else if (hasPlayer && health.IsTargetable)
            {
                // Keep only the outgoing attacker skinned until its attack blend has ended.
                returnToBakedPending = true;
                returnToBakedAt = Time.time + PostAttackBlendGrace;
            }
        }

        private void OnDied()
        {
            returnToBakedPending = false;
            SetBaked(false);
        }

        private void LateUpdate()
        {
            if (returnToBakedPending && Time.time >= returnToBakedAt &&
                (Time.time >= returnToBakedAt + MaxPostAttackBlend || animator == null ||
                    (!animator.IsInTransition(0) &&
                     animator.GetCurrentAnimatorStateInfo(0).fullPathHash != AttackStateHash)))
            {
                returnToBakedPending = false;
                SetBaked(true);
            }

            if (!isBaked || Time.time < nextPoseTime)
                return;

            nextPoseTime = Time.time + poseInterval;
            if (movement.IsMoving)
            {
                poseIndex++;
                if (poseIndex == distantRunPoses.Length)
                    poseIndex = 0;
            }
            else
            {
                poseIndex = 0;
            }
            ApplyPose();
        }

        private void SetBaked(bool value)
        {
            if (animatedRenderer == null || distantRenderer == null || distantMeshFilter == null ||
                distantRunPoses == null || distantRunPoses.Length == 0)
                return;

            if (value && distantRunPoses[poseIndex] == null)
                value = false;
            if (isBaked == value && animatedRenderer.enabled == !value &&
                distantRenderer.enabled == value)
                return;

            if (value)
                ApplyPose();
            animatedRenderer.enabled = !value;
            distantRenderer.enabled = value;
            isBaked = value;
        }

        private void ApplyPose()
        {
            Mesh pose = distantRunPoses[poseIndex];
            if (pose != null && distantMeshFilter.sharedMesh != pose)
                distantMeshFilter.sharedMesh = pose;
        }

        private void OnDisable()
        {
            returnToBakedPending = false;
            attack.AttackStateChanged -= OnAttackStateChanged;
            health.Died -= OnDied;
            SetBaked(false);
        }

        private void OnValidate() => poseInterval = Mathf.Max(0.05f, poseInterval);
    }
}
