using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDP
{
    public class LevelSelecter : MonoBehaviour
    {
        public SceneFader sceneFader;

        public Button[] levelButtons;

        public void Initialize()
        {
            // To do : save progress and load it by making manager class
            int levelProgressed = PlayerPrefs.GetInt("levelProgressed", 1);

            for (int i = levelProgressed ; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }
        }

        public void Select(int targetLevel)
        {
            sceneFader.FadeTo(SceneType.Stage, targetLevel);
        }

    }
}
