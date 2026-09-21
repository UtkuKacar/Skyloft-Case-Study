using UnityEngine;

namespace Skyloft.Data
{
    [CreateAssetMenu(menuName = "Skyloft/Difficulty", fileName = "Difficulty")]
    public sealed class DifficultyConfig : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float spawnInterval = 4f;
        [SerializeField, Min(1)] private int enemiesPerBatch = 2;
        [SerializeField, Min(1)] private int maxActiveEnemies = 12;

        public float SpawnInterval => spawnInterval;
        public int EnemiesPerBatch => enemiesPerBatch;
        public int MaxActiveEnemies => maxActiveEnemies;

        private void OnValidate()
        {
            spawnInterval = float.IsNaN(spawnInterval) || float.IsInfinity(spawnInterval)
                ? 4f : Mathf.Max(0.1f, spawnInterval);
            enemiesPerBatch = Mathf.Max(1, enemiesPerBatch);
            maxActiveEnemies = Mathf.Max(1, maxActiveEnemies);
        }
    }
}
