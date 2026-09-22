using Skyloft.Data;
using Skyloft.Enemy;
using Skyloft.Player;
using UnityEngine;

namespace Skyloft.Spawning
{
    /// <summary>Timed batches for the current flat arena. Owns creation and retirement, not enemy behaviour.</summary>
    [DisallowMultipleComponent]
    public sealed class WaveSpawner : MonoBehaviour
    {
        private const int PositionAttempts = 12;
        [SerializeField] private DifficultyConfig difficulty;
        [SerializeField] private EnemyAttack enemyPrefab;
        [SerializeField] private PlayerHealth player;
        [SerializeField] private Collider arena;
        [SerializeField] private Transform spawnParent;
        [SerializeField, Min(0.5f)] private float boundaryInset = 1f;
        [SerializeField, Min(1f)] private float minimumPlayerDistance = 8f;
        [SerializeField, Min(0.5f)] private float minimumEnemySpacing = 1f;
        [SerializeField] private LayerMask environmentMask;

        private Bounds spawnBounds;
        private float nextBatchTime;
        private bool playable = true;
        private bool configured;
        private float enemyCapsuleRadius;
        private float enemyCapsuleHeight;

        public DifficultyConfig Difficulty => difficulty;
        public bool CanSpawn => configured && playable && difficulty != null &&
            player != null && player.isActiveAndEnabled && !player.IsDead &&
            spawnParent != null && spawnParent.gameObject.activeInHierarchy;

        private void Awake()
        {
            configured = difficulty != null && enemyPrefab != null && player != null &&
                arena != null && arena.enabled && spawnParent != null;
            if (configured)
            {
                var capsule = enemyPrefab.GetComponent<CharacterController>();
                configured = capsule != null && enemyPrefab.GetComponent<EnemyDeath>() != null;
                if (configured)
                {
                    enemyCapsuleRadius = capsule.radius + capsule.skinWidth;
                    enemyCapsuleHeight = capsule.height;
                    float inset = Mathf.Max(boundaryInset, capsule.radius + capsule.skinWidth);
                    spawnBounds = arena.bounds;
                    configured = spawnBounds.size.x > inset * 2f && spawnBounds.size.z > inset * 2f;
                    spawnBounds.SetMinMax(spawnBounds.min + new Vector3(inset, 0f, inset),
                        spawnBounds.max - new Vector3(inset, 0f, inset));
                }
            }
            if (!configured)
                Debug.LogError("[WaveSpawner] Assign a difficulty, valid Enemy prefab, Player, flat arena and spawn parent.", this);
        }

        private void OnEnable() => ResetSchedule();

        public void SetPlayable(bool value)
        {
            playable = value;
            ResetSchedule();
        }

        public void SetDifficulty(DifficultyConfig value)
        {
            difficulty = value;
            ResetSchedule();
        }

        private void ResetSchedule() => nextBatchTime = Time.time + (difficulty != null ? difficulty.SpawnInterval : 0f);

        private void Update()
        {
            if (!CanSpawn)
            {
                ResetSchedule();
                return;
            }
            if (Time.time < nextBatchTime)
                return;

            ResetSchedule(); // No deferred batches or catch-up burst after a pause/stall.
            for (int i = 0; i < difficulty.EnemiesPerBatch && EnemyRegistry.Count < difficulty.MaxActiveEnemies; i++)
                if (TryGetSpawnPosition(out Vector3 position))
                    CreateEnemy(position);
        }

        private bool TryGetSpawnPosition(out Vector3 position)
        {
            float playerDistanceSquared = minimumPlayerDistance * minimumPlayerDistance;
            float spacingSquared = minimumEnemySpacing * minimumEnemySpacing;
            for (int attempt = 0; attempt < PositionAttempts; attempt++)
            {
                int edge = Random.Range(0, 4);
                float x = edge < 2 ? (edge == 0 ? spawnBounds.min.x : spawnBounds.max.x)
                    : Random.Range(spawnBounds.min.x, spawnBounds.max.x);
                float z = edge >= 2 ? (edge == 2 ? spawnBounds.min.z : spawnBounds.max.z)
                    : Random.Range(spawnBounds.min.z, spawnBounds.max.z);
                position = new Vector3(x, spawnBounds.max.y + 0.02f, z);
                Vector3 offset = position - player.transform.position;
                offset.y = 0f;
                if (offset.sqrMagnitude < playerDistanceSquared)
                    continue;
                Vector3 capsuleBottom = position + Vector3.up * enemyCapsuleRadius;
                Vector3 capsuleTop = position + Vector3.up *
                    Mathf.Max(enemyCapsuleRadius, enemyCapsuleHeight - enemyCapsuleRadius);
                if (environmentMask.value != 0 && Physics.CheckCapsule(
                    capsuleBottom, capsuleTop, enemyCapsuleRadius, environmentMask,
                    QueryTriggerInteraction.Ignore))
                    continue;
                bool occupied = false;
                for (int i = 0; i < EnemyRegistry.Count; i++)
                {
                    var other = EnemyRegistry.GetAt(i);
                    if (other == null)
                        continue;
                    offset = position - other.CachedTransform.position;
                    offset.y = 0f;
                    if (offset.sqrMagnitude < spacingSquared)
                    {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied)
                    return true;
            }
            position = default;
            return false;
        }

        private void CreateEnemy(Vector3 position)
        {
            EnemyAttack instance = Instantiate(enemyPrefab, position, Quaternion.identity, spawnParent);
            instance.GetComponent<EnemyDeath>().Retired += ReleaseEnemy;
            instance.GetComponent<EnemyMovement>().SetMovementSpeed(difficulty.EnemyMovementSpeed);
            instance.SetTarget(player);
        }

        // Static callback stays valid if the spawner is disabled or removed before its enemies die.
        private static void ReleaseEnemy(EnemyDeath enemy)
        {
            enemy.Retired -= ReleaseEnemy;
            Destroy(enemy.gameObject);
        }
    }
}
