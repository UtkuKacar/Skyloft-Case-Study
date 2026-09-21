using Skyloft.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Skyloft.UI
{
    public sealed class RunHUD : MonoBehaviour
    {
        [SerializeField] private GameFlow flow;
        [SerializeField] private Text timer;
        [SerializeField] private Text kills;

        private void OnEnable()
        {
            flow.SecondsChanged += ShowSeconds;
            flow.KillsChanged += ShowKills;
            ShowSeconds(Mathf.CeilToInt(flow.RemainingSeconds));
            ShowKills(flow.SessionKills);
        }

        private void OnDisable()
        {
            flow.SecondsChanged -= ShowSeconds;
            flow.KillsChanged -= ShowKills;
        }

        private void ShowSeconds(int seconds) => timer.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        private void ShowKills(int count) => kills.text = "Kills: " + count;
    }
}
