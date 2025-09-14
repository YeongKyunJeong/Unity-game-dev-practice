using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RSP2
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [field: SerializeField] private DataManager dataManager { get; set; }
        [field: SerializeField] private SceneFader sceneFader { get; set; }
        public SceneFader SceneFader { get => sceneFader; }

        [field: SerializeField] public List<int> GameProgress { get; private set; }
        //[field: SerializeField] private InGameInitializer inGameInitializer { get; set; }

        //[field: SerializeField] private SceneInitializer sceneInitializer;


        [field: SerializeField] private SceneType currentSceneType { get; set; }
        public UserData CurrentUserData { get; private set; }

        public Action<int> GameProgressChangeEvent;

        public const string TITLE_SCENE_NAME_STR = "TitleScene";
        public const string IN_GAME_SCENE_NAME_STR = "InGameScene_";


        protected override void Awake()
        {
            base.Awake();

            if (Instance == null) { }// Always false by MonoSingleton

            if (dataManager == null)
            {
                Debug.Log("Data Manager Not Assigned");
                dataManager = FindObjectOfType<DataManager>();
            }

            if (sceneFader == null)
            {
                Debug.Log("Scene Fader Not Assigned");
                sceneFader = FindObjectOfType<SceneFader>();
            }

            dataManager.Initialize();
            sceneFader.Initialize(this);

            CurrentUserData = dataManager.UserDataLoader.LoadUserData(0);

            currentSceneType = GetCurrentSceneType();

            DontDestroyOnLoad(gameObject);
        }

        private SceneType GetCurrentSceneType(string sceneName = "")
        {
            if (sceneName.Length == 0)
            {
                sceneName = SceneManager.GetActiveScene().name;
            }

            if (sceneName.Contains(TITLE_SCENE_NAME_STR))
            {
                return SceneType.TitleScene;
            }
            else if (sceneName.Contains(IN_GAME_SCENE_NAME_STR))
            {
                return SceneType.InGameScene;
            }
            // TO DO :: Add if Other sceneType is Add

            return SceneType.InGameScene;
        }


        /// <param name="sceneName">로드할 씬 이름</param>
        public void LoadScene(string sceneName)
        {
            sceneFader.CallFade(FadingType.SlowFadeOut, true, () => StartCoroutine(LoadSceneAsync(sceneName)));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // TO DO :: Add Loading Screen
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            currentSceneType = GetCurrentSceneType(sceneName);
            InitializeScene();

            yield return null;
        }

        private void InitializeScene()
        {
            SceneInitializer sceneInitializer = FindObjectOfType<SceneInitializer>();
            if (sceneInitializer != null)
            {
                sceneInitializer.Initialize(); // Just to Ensure SceneInitializer Assigned
            }
            else
            {
                Debug.LogWarning("No Scene Initializer Found in This Scene");
            }

            sceneFader.CallFade(FadingType.SlowFadeIn, false, null);
        }

        #region Title Scene
        public void TitleSceneContinueCall(/*string sceneSubName*/)
        {
            PlayerSaveData lastSaveData = dataManager.UserDataLoader.CurrentSaveDataList[dataManager.UserDataLoader.CurrentSaveDataList.Count - 1];

            GameProgress = lastSaveData.GameProgress;

            if (lastSaveData.SceneNumber <= 0)
            {
                // TO DO :: Add Each Scene Name Finding by SceneNumber Logic
                LoadScene(string.Concat(IN_GAME_SCENE_NAME_STR, "01"));
                return;
            }

            LoadScene(string.Concat(IN_GAME_SCENE_NAME_STR, lastSaveData.SceneNumber.ToString("D2")));
            return;
        }

        public void TitleSceneStartCall(string sceneSubName)
        {
            LoadScene(string.Concat(IN_GAME_SCENE_NAME_STR, sceneSubName));
        }

        public void TitleSceneQuitCall()
        {
            QuitGame();
        }

        public PlayerSaveData CallSaveDataLoading(int userID = 0, int saveNumber = -1)
        {
            PlayerSaveData playerSaveData = dataManager.LoadSaveData(userID, saveNumber);
            GameProgress = playerSaveData.GameProgress;
            return playerSaveData;
        }

        public void QuitGame()
        {
            // TO DO:: Add Game Save Logic

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region In Game Scene
        public void InGameSceneSaveCall()
        {
            dataManager.UserDataWriter.SaveCurrentData();
        }

        public void InGameSceneLoadCall(int saveNumber)
        {
            PlayerSaveData targetSaveData = dataManager.LoadSaveData(CurrentUserData.UserID, saveNumber);

            if (targetSaveData == null) return;

            LoadScene(string.Concat(IN_GAME_SCENE_NAME_STR, targetSaveData.SceneNumber.ToString("D2")));
            return;
        }

        public void InGameSceneTitleCall()
        {
            LoadScene(string.Concat(TITLE_SCENE_NAME_STR));
        }


        #region Game Progress
        public void SetGameProgress(int sceneNumber, List<int> gameProgress)
        {
            GameProgress = gameProgress;

            GameProgressChangeEvent?.Invoke(gameProgress[sceneNumber - 1]);
        }

        public void SetGameProgress(int sceneNumber, int gameProgressInThisScene)
        {
            if (GameProgress.Count + 1 == sceneNumber)
            {
                GameProgress.Add(gameProgressInThisScene);
            }
            else if (GameProgress.Count <= sceneNumber)
            {
                GameProgress[sceneNumber - 1] = gameProgressInThisScene;
            }
            else
            {
                Debug.Log("Game Progress is Not Matched With Game Scene");
                return;
            }

            GameProgressChangeEvent?.Invoke(gameProgressInThisScene);
        }
        #endregion

        #endregion
    }
}
