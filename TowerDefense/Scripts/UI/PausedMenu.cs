using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{
    public class PausedMenu : MonoBehaviour // Change component position to GameManager or StageManger
    {
        private GameManager gameManager;
        [SerializeField] private GameObject uiGO;

        public void Initialize()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
            if (uiGO.activeSelf)
            {
                uiGO.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            uiGO.SetActive(!uiGO.activeSelf);

            if (uiGO.activeSelf)
            {
                Time.timeScale = 0;
                Debug.Log("Time Stopped");
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("Time Continued");
            }
        }

        public void RetryCall()
        {
            Toggle();
            gameManager.RetryCall();
        }
        public void Menu()
        {
            Toggle();
            gameManager.MenuCall();
        }
    }
}
