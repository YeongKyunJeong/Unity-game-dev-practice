using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class TitleInitializer : SceneInitializer
    {
        [field: SerializeField] private static GameObject gameManagerPrefab { get; set; }
        [field: SerializeField] private GameObject TitleSceneManagerPrefab { get; set; }

        [field: SerializeField] private TitleManager titleSceneManager;

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

            if (titleSceneManager == null)
            {
                Debug.Log("TitleScene Manager Not Assigned");
                titleSceneManager = FindObjectOfType<TitleManager>();

                if (titleSceneManager == null)
                {
                    Debug.LogWarning("In Game Manager Not Exists");
                    titleSceneManager = Instantiate(TitleSceneManagerPrefab).GetComponent<TitleManager>();
                }
            }
#endif
            titleSceneManager.Initialize();

        }
    }
}
