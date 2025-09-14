using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDP
{
    public class GameOverPanel : MonoBehaviour
    {
        private GameManager gameManager;

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
        //private void OnEnable()
        //{
        //    roundsText.text = PlayerStats.Rounds.ToString();
        //}

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

        public void RetryCall()
        {
            OnEnableByManual(false);
            gameManager.RetryCall();
        }

        public void MenuCall()
        {
            OnEnableByManual(false);
            gameManager.MenuCall();
        }
    }
}
