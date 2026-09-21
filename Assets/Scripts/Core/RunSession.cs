using Skyloft.Data;
using UnityEngine;

namespace Skyloft.Core
{
    /// <summary>Small cross-scene payload; gameplay state remains owned by GameFlow.</summary>
    public static class RunSession
    {
        public const string TotalKillsKey = "Skyloft.TotalKills";
        public static DifficultyConfig SelectedDifficulty { get; set; }
        public static bool Won { get; private set; }
        public static int LastSessionKills { get; private set; }
        public static int TotalKills => Mathf.Max(0, PlayerPrefs.GetInt(TotalKillsKey, 0));

        public static void RecordKill()
        {
            int total = TotalKills;
            if (total < int.MaxValue)
                PlayerPrefs.SetInt(TotalKillsKey, total + 1);
        }

        public static void StoreResult(bool won, int kills)
        {
            Won = won;
            LastSessionKills = kills;
            PlayerPrefs.Save();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            SelectedDifficulty = null;
            Won = false;
            LastSessionKills = 0;
        }
    }
}
