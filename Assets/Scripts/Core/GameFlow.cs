using System;
using System.Collections;
using Skyloft.Enemy;
using Skyloft.Player;
using Skyloft.Spawning;
using UnityEngine;

namespace Skyloft.Core
{
    public enum RunState { Preparing, Playing, Won, Lost }

    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class GameFlow : MonoBehaviour
    {
        [SerializeField] private PlayerHealth player;
        [SerializeField] private WaveSpawner spawner;
        [SerializeField, Min(1f)] private float survivalDuration = 180f;
        [SerializeField, Min(0f)] private float deathPoseDelay = 3.2f;
        private PlayerMovement movement;
        private PlayerCombat combat;
        private PlayerInput input;
        private int displayedSeconds = -1;

        public RunState State { get; private set; } = RunState.Preparing;
        public float RemainingSeconds { get; private set; }
        public int SessionKills { get; private set; }
        public event Action<int> SecondsChanged;
        public event Action<int> KillsChanged;

        private void Awake()
        {
            movement = player.GetComponent<PlayerMovement>();
            combat = player.GetComponent<PlayerCombat>();
            input = player.GetComponent<PlayerInput>();
            RemainingSeconds = survivalDuration;
            SetPlayerPlaying(false);
            spawner.SetPlayable(false);
        }

        private void OnEnable()
        {
            player.Died += Lose;
            EnemyRegistry.EnemyDied += CountKill;
        }

        private void OnDisable()
        {
            player.Died -= Lose;
            EnemyRegistry.EnemyDied -= CountKill;
        }

        private IEnumerator Start()
        {
            while (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning)
                yield return null;
            if (RunSession.SelectedDifficulty == null)
                RunSession.SelectedDifficulty = spawner.Difficulty; // Direct Game scene testing.
            spawner.SetDifficulty(RunSession.SelectedDifficulty);
            State = RunState.Playing;
            SessionKills = 0;
            SetPlayerPlaying(true);
            spawner.SetPlayable(true);
            PublishTime();
            KillsChanged?.Invoke(SessionKills);
        }

        private void Update()
        {
            if (State != RunState.Playing)
                return;
            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.deltaTime);
            PublishTime();
            if (RemainingSeconds <= 0f)
                Finish(player.IsDead ? RunState.Lost : RunState.Won);
        }

        private void PublishTime()
        {
            int seconds = Mathf.CeilToInt(RemainingSeconds);
            if (seconds == displayedSeconds)
                return;
            displayedSeconds = seconds;
            SecondsChanged?.Invoke(seconds);
        }

        private void CountKill(EnemyHealth enemy)
        {
            if (State != RunState.Playing)
                return;
            if (SessionKills < int.MaxValue)
                SessionKills++;
            RunSession.RecordKill();
            KillsChanged?.Invoke(SessionKills);
        }

        private void Lose() => Finish(RunState.Lost);

        private void Finish(RunState result)
        {
            if (State != RunState.Playing)
                return;
            State = result;
            spawner.SetPlayable(false);
            SetPlayerPlaying(false);
            for (int i = 0; i < EnemyRegistry.Count; i++)
            {
                var enemy = EnemyRegistry.GetAt(i);
                enemy.SetDamageEnabled(false);
                enemy.GetComponent<EnemyAttack>().enabled = false;
                enemy.GetComponent<EnemyMovement>().enabled = false;
                enemy.GetComponent<EnemyRenderLOD>()?.SetPlayer(null);
            }
            RunSession.StoreResult(result == RunState.Won, SessionKills);
            StartCoroutine(ShowResult(result == RunState.Lost ? deathPoseDelay : 0.25f));
        }

        private void SetPlayerPlaying(bool value)
        {
            movement.enabled = value;
            combat.enabled = value;
            input.enabled = value;
            player.SetDamageEnabled(value);
        }

        private IEnumerator ShowResult(float delay)
        {
            float remaining = delay;
            while (remaining > 0f)
            {
                remaining -= Time.unscaledDeltaTime;
                yield return null;
            }
            SceneTransition.Instance.Load(SceneTransition.ResultScene);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                PlayerPrefs.Save();
        }

        private void OnApplicationQuit() => PlayerPrefs.Save();
    }
}
