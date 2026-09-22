using UnityEngine;

namespace Skyloft.Data
{
    [CreateAssetMenu(menuName = "Skyloft/Difficulty", fileName = "Difficulty")]
    public sealed class DifficultyConfig : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float spawnInterval = 4f;
        [SerializeField, Min(1)] private int enemiesPerBatch = 2;
        [SerializeField, Min(1)] private int maxActiveEnemies = 12;
        [SerializeField, Min(0f)] private float enemyMovementSpeed = 5f;

        public float SpawnInterval => spawnInterval;
        public int EnemiesPerBatch => enemiesPerBatch;
        public int MaxActiveEnemies => maxActiveEnemies;
        public float EnemyMovementSpeed => enemyMovementSpeed;

        private void OnValidate()
        {
            spawnInterval = float.IsNaN(spawnInterval) || float.IsInfinity(spawnInterval)
                ? 4f : Mathf.Max(0.1f, spawnInterval);
            enemiesPerBatch = Mathf.Max(1, enemiesPerBatch);
            maxActiveEnemies = Mathf.Max(1, maxActiveEnemies);
            enemyMovementSpeed = float.IsNaN(enemyMovementSpeed) || float.IsInfinity(enemyMovementSpeed)
                ? 5f : Mathf.Max(0f, enemyMovementSpeed);
        }
    }
}
