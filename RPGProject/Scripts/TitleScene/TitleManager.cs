using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class TitleManager : MonoBehaviour
    {
        private GameManager gameManager;

        private bool isLoading;

        public void Initialize()
        {
            isLoading = false;
            gameManager = GameManager.Instance;
            // TO DO 
        }

        public void ContinueCall()
        {
            if (isLoading) return;

            isLoading = true;
            gameManager.TitleSceneContinueCall();
        }

        public void StartCall(string sceneSubName)
        {
            if (isLoading) return;

            isLoading = true;
            gameManager.TitleSceneStartCall(sceneSubName);
        }
        public void OptionCall()
        {
            if (isLoading) return;

            // TO DO:: Add option window
        }

        public void QuitCall()
        {
            if (isLoading) return;

            isLoading = true;
            gameManager.TitleSceneQuitCall();
        }

    }
}
