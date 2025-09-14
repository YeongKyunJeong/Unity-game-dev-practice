using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDP
{

    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance { get { return instance; } private set { instance = value; } }

        private static StageManager stageManager;
        public static StageManager StageManager { get { return stageManager; } private set { stageManager = value; } }

        private static LevelSelecter levelSelecter;
        public static LevelSelecter LevelSelecter { get { return levelSelecter; } private set { levelSelecter = value; } }

        private static MainMenu mainMenu;

        [SerializeField] private SceneFader sceneFader;
        public const string MAIN_MENU_SCENE_NAME_STR = "MainMenuScene";
        public const string LEVEL_SELECTER_SCENE_STR = "LevelSelecter";
        public const string STAGE_SCENE_NAME_STR = "StageScene";

        [SerializeField] private PlayerStats playerStats;
        //public PlayerStats GetPlayerStats { get => playerStats; }
        public static bool isGameOver = false;



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            playerStats = GetComponent<PlayerStats>();
            playerStats.Initialize();

            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case MAIN_MENU_SCENE_NAME_STR:
                    {
                        if (mainMenu == null)
                        {
                            mainMenu = FindFirstObjectByType<MainMenu>();
                            mainMenu.Initialize();
                        }
                        break;
                    }
                case LEVEL_SELECTER_SCENE_STR:
                    {
                        if (levelSelecter == null)
                        {
                            levelSelecter = FindObjectOfType<LevelSelecter>();
                        }
                        levelSelecter.Initialize();
                        break;
                    }
                case STAGE_SCENE_NAME_STR:
                    {
                        if (StageManager == null)
                        {
                            StageManager = FindObjectOfType<StageManager>();
                        }
                        StageManager.Initialize();
                        break;
                    }
                default:
                    {
                        if (sceneName.Contains(STAGE_SCENE_NAME_STR))
                        {
                            if (StageManager == null)
                            {
                                StageManager = FindObjectOfType<StageManager>();
                            }
                            StageManager.Initialize();
                        }

                        break;
                    }
            }

            isGameOver = false;
        }

        private void Update()
        {
            if (isGameOver)
            {
                return;
            }

            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    EndGame();
            //}

            if (PlayerStats.Life <= 0)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            isGameOver = true;

            stageManager.EndGame();
            //Debug.Log("Game Over!");

            //Time.timeScale = 0;
        }

        public void RetryCall()
        {
            CallSceneFader(SceneType.Retry);
        }

        public void MenuCall()
        {
            CallSceneFader(SceneType.MainMenu);
        }

        public void GoOtherLevelCall()
        {
            CallSceneFader(SceneType.Stage, stageManager.GetStageLevel + 1);
        }


        public void GoOtherLevelCall(int targetLevel)
        {
            CallSceneFader(SceneType.Stage, targetLevel);

        }

        public void MainMenuPlayCall()
        {
            PlayGame();
        }

        private void PlayGame()
        {
            CallSceneFader(SceneType.LevelSelecter);
            //SceneManager.LoadScene(STAGE_SCENE_NAME_STR);
        }

        private void CallSceneFader(SceneType targetSceneType, int targetLevel = 0)
        {
            if (sceneFader == null)
            {
                sceneFader = FindObjectOfType<SceneFader>();
            }
            sceneFader.FadeTo(targetSceneType, targetLevel);
        }

        public void MainMenuQuitCall()
        {
            QuitGame();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        //public void GoAnotherLevelCall(int targetLevel = 1)
        //{
        //    CallSceneFader(SceneType.Stage, targetLevel);
        //}
    }
}
