using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{
    public class MainMenu : MonoBehaviour
    {
        private GameManager gameManager;

        public void Initialize()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
        }

        public void PlayCall()
        {
            gameManager.MainMenuPlayCall();
        }

        public void QuitCall()
        {
            gameManager.MainMenuQuitCall();
        }
    }
}