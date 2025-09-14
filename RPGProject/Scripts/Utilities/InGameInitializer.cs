using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RSP2
{
    public class InGameInitializer : SceneInitializer
    {
        [field: SerializeField] private GameObject gameManagerPrefab { get; set; }
        [field: SerializeField] private GameObject inGameManagerPrefab { get; set; }

        [field: SerializeField] private InGameManager inGameManager { get; set; }



        public override void Initialize()
        {
            if (isInitialize) return;

            isInitialize = true;

#if UNITY_EDITOR
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Instantiate(gameManagerPrefab);
            }

            if (inGameManager == null)
            {
                Debug.Log("In Game Manager Not Assigned");
                inGameManager = FindObjectOfType<InGameManager>();

                if (inGameManager == null)
                {
                    Debug.LogWarning("In Game Manager Not Exists");
                    inGameManager = Instantiate(inGameManagerPrefab).GetComponent<InGameManager>();
                }
            }
#endif
            inGameManager.Initialize();

        }
    }
}
