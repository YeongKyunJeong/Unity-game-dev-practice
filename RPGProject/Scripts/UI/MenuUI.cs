using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace RSP2
{
    public class MenuUI : MonoBehaviour
    {
        private GameManager gameManager;
        private InGameManager inGameManager;

        private bool isLoading;
        private Coroutine loadingCoroutine;


        public bool IsActive { get => GetIsOpened(); }


        public void Initialize(InGameManager _inGameManager)
        {
            inGameManager = _inGameManager;
            gameManager = GameManager.Instance;
            isLoading = false;

            gameObject.SetActive(false);
        }

        private bool GetIsOpened()
        {
            return gameObject.activeSelf;
        }

        public void Open()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void SaveCall()
        {
            if (isLoading) return;

            gameManager.InGameSceneSaveCall();
            loadingCoroutine = StartCoroutine(LoadingCoroutine());

        }

        public void LoadCall()
        {
            // TO DO :: Open Save List Windows
        }

        public void TitleCall()
        {
            if (isLoading) return;

            isLoading = true;
            gameManager.InGameSceneTitleCall();
        }

        private IEnumerator LoadingCoroutine()
        {
            isLoading = true;
            yield return new WaitForSeconds(2);

            isLoading = false;
            loadingCoroutine = null;
            yield return null;
        }

    }
}
