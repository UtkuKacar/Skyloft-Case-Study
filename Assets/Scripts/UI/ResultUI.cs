using Skyloft.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Skyloft.UI
{
    public sealed class ResultUI : MonoBehaviour
    {
        [SerializeField] private Text outcome;
        [SerializeField] private Text sessionKills;
        [SerializeField] private Text totalKills;

        private void Start()
        {
            outcome.text = RunSession.Won ? "YOU WIN" : "YOU LOSE";
            sessionKills.text = "Kills: " + RunSession.LastSessionKills;
            totalKills.text = "Total Kills: " + RunSession.TotalKills;
        }

        public void Replay() => SceneTransition.Instance.Load(SceneTransition.GameScene);
        public void MainMenu() => SceneTransition.Instance.Load(SceneTransition.MenuScene);
    }
}
