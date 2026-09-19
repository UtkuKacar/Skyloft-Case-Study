using System.Collections.Generic;
using UnityEngine;

namespace Skyloft.Enemy
{
    /// <summary>Active, living enemies register through their health component's lifecycle.</summary>
    public static class EnemyRegistry
    {
        private static readonly List<EnemyHealth> Enemies = new List<EnemyHealth>(64);

        public static int Count => Enemies.Count;
        public static EnemyHealth GetAt(int index) => Enemies[index];

        internal static void Register(EnemyHealth enemy)
        {
            if (enemy.IsTargetable && !Enemies.Contains(enemy))
                Enemies.Add(enemy);
        }

        internal static void Unregister(EnemyHealth enemy) => Enemies.Remove(enemy);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => Enemies.Clear();
    }
}
