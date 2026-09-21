using Skyloft.Core;
using Skyloft.Data;
using UnityEngine;

namespace Skyloft.UI
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        public void SelectDifficulty(DifficultyConfig difficulty)
        {
            if (difficulty == null || SceneTransition.Instance.IsTransitioning)
                return;
            RunSession.SelectedDifficulty = difficulty;
            SceneTransition.Instance.Load(SceneTransition.GameScene);
        }
    }
}
