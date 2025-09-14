using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class CompleteLevelPanel : MonoBehaviour
    {
        public GameManager gameManager;

        public void Initialize()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            if (gameObject.activeInHierarchy)
            {
                OnEnableByManual(false);
            }
        }

        public void OnEnableByManual(bool isOn)
        {
            gameObject.SetActive(isOn);
            if (isOn)
            {
                //roundsText.text = PlayerStats.Rounds.ToString();
                Time.timeScale = 0;
                Debug.Log("Game Stopped");
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("Game Continued");
            }
        }

        public void Continue()
        {
            OnEnableByManual(false);
            gameManager.GoOtherLevelCall();
        }

        public void MenuCall()
        {
            OnEnableByManual(false);
            gameManager.MenuCall();
        }

    }
}
