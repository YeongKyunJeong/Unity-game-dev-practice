using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RSP2
{
    public enum SceneType
    {
        None,
        TitleScene,
        InGameScene
    }

    public class InGameManager : MonoSingleton<InGameManager>
    {
        private GameManager gameManager;

        [field: SerializeField] private int sceneNumber { get; set; }
        [field: SerializeField] public bool IsInitialized { get; private set; }

        [field: SerializeField] private PlayerInput PlayerInput { get; set; }

        [field: SerializeField] private CameraManager CameraManager { get; set; }
        [field: SerializeField] private ProjectileManager ProjectileManager { get; set; }
        [field: SerializeField] private InteractionManager InteractionManager { get; set; }
        [field: SerializeField] private VFXManager VFXManager { get; set; }
        [field: SerializeField] private SFXManager SFXManager { get; set; }
        [field: SerializeField] private DayNightManager DayNightManager { get; set; }
        [field: SerializeField] private QuestManager QuestManager { get; set; }
        [field: SerializeField] private CutsceneManager CutsceneManager { get; set; }

        [field: SerializeField] private CanvasUIManager CanvasUIManager { get; set; }

        public Player Player { get; set; }
        public CinemachineInputProvider CinemachineInputProvider { get; set; }

        public PlayerSaveData CurrentSaveData { get; private set; }
        public event Action<Enemy> EnemyDieEvent;


        //private void Awake()
        public void Initialize()
        {
            gameManager = GameManager.Instance;

            CanvasUIManager = FindObjectOfType<CanvasUIManager>();
            Player = FindObjectOfType<Player>();
            PlayerInput = Player.GetComponent<PlayerInput>();
            CinemachineInputProvider = FindObjectOfType<CinemachineInputProvider>();

            if (CameraManager == null)
            {
                Debug.Log("Camera Manager Not Assigned");
                CameraManager = FindObjectOfType<CameraManager>();
            }

            if (ProjectileManager == null)
            {
                Debug.Log("Projectile Manager Not Assigned");
                ProjectileManager = FindObjectOfType<ProjectileManager>();
            }
            if (InteractionManager == null)
            {
                Debug.Log("Interaction Manager Not Assigned");
                InteractionManager = FindObjectOfType<InteractionManager>();
            }
            if (VFXManager == null)
            {
                Debug.Log("VFX Manager Not Assigned");
                VFXManager = FindObjectOfType<VFXManager>();
            }
            if (SFXManager == null)
            {
                Debug.Log("SFX Manager Not Assigned");
                SFXManager = FindObjectOfType<SFXManager>();
            }
            if (DayNightManager == null)
            {
                Debug.Log("Day Night Manager Not Assigned");
                DayNightManager = FindObjectOfType<DayNightManager>();
            }
            if (QuestManager == null)
            {
                Debug.Log("Quest Manager Not Assigned");
                QuestManager = FindObjectOfType<QuestManager>();
            }
            if (CutsceneManager == null)
            {
                Debug.Log("Cut Scene Manager Not Assigned");
                CutsceneManager = FindObjectOfType<CutsceneManager>();
            }

            CameraManager.Initialize(this);

            ProjectileManager.Initialize(this);
            InteractionManager.Initialize(this, CameraManager, CanvasUIManager);
            VFXManager.Initialize(this);
            SFXManager.Initialize(this);
            DayNightManager.Initialize();
            CutsceneManager.Initialize(this);

            CanvasUIManager.Initialize(this);

            gameManager.GameProgressChangeEvent += OnGameProgressChange;

            CurrentSaveData = gameManager.CallSaveDataLoading(gameManager.CurrentUserData.UserID);
        }

        private void Start()
        {
            LockCursor(true);
        }

        private void Update()
        {
            ProjectileManager.CallUpdate();
        }

        private void FixedUpdate()
        {
            //if (!IsInitialized) return;

            DayNightManager.CallPhysicsUpdate();
        }

        public void OnPanelUIOpen(bool isOn)
        {
            if (isOn)
            {
                EnableInputActionMap(ActionMap.UI);
            }
            else
            {
                EnableInputActionMap(ActionMap.Field);
            }
            EnableCinemachineInput(!isOn);
            LockCursor(!isOn);
        }

        public void OnInteractionUIOpen(bool isOn)
        {
            if (isOn)
            {
                EnableInputActionMap(ActionMap.Interaction);
            }
            else
            {
                EnableInputActionMap(ActionMap.Field);
            }
            EnableCinemachineInput(!isOn);
        }

        // TO DO :: Standardize by making using Action

        //public bool CheckNowInteractable()
        //{
        //    if (Player.ActionStateMachine.isDead) return false;

        //    if (!Player.ActionStateMachine.isOnLand) return false;

        //    return true;
        //}

        private void EnableInputActionMap(ActionMap targetMap, bool isOnly = true)
        {
            switch (targetMap)
            {
                case ActionMap.Field:
                    {
                        Player?.InputReader.EnableFieldInput(isOnly); break;
                    }
                case ActionMap.UI:
                    {
                        Player?.InputReader.EnableUIInput(isOnly); break;
                    }
                case ActionMap.Interaction:
                    {
                        Player?.InputReader.EnableInteractionInput(isOnly); break;
                    }
            }
        }

        private void EnableCinemachineInput(bool isOn)
        {
            CinemachineInputProvider.enabled = isOn;
        }


        public void LockCursor(bool isLock)
        {
            if (isLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        //public void EnemyDie(Enemy diedEnemy)
        //{
        //    EnemyDieEvent?.Invoke(diedEnemy);
        //}

        //private void CheckScene()
        //{
        //    string sceneName = SceneManager.GetActiveScene().name;
        //    switch (sceneName)
        //    {
        //        case TITLE_SCENE_NAME_STR:
        //            {
        //                IsInitialized = false;
        //                break;
        //            }
        //        default:
        //            {
        //                if (sceneName.Contains(GAME_SCENE_NAME_STR))
        //                {
        //                    IsInitialized = true;
        //                }
        //                else
        //                {
        //                    Debug.LogError("Scene Name Is Not Correct");
        //                    IsInitialized = false;
        //                }
        //                break;
        //            }
        //    }
        //}

        public PlayerSaveData GetCurrentDataToSave()
        {
            PlayerSaveData newSaveData = new PlayerSaveData();

            newSaveData.SceneNumber = sceneNumber;

            newSaveData.GameProgress = gameManager.GameProgress;

            Player.GetPlayerDataForSave(newSaveData);
            QuestManager.GetQuestDataForSave(newSaveData);
            // TO DO:: NPCs Data

            return newSaveData;
        }

        public void GetDataFromSave(int userID = 0, int saveNumber = -1)
        {
            PlayerSaveData loadedSaveData = gameManager.CallSaveDataLoading(userID, saveNumber);

            Player.SetPlayerDataFromSave(loadedSaveData);
            QuestManager.SetQuestDataFromSave(loadedSaveData);

            // TO DO:: NPCs Data
            gameManager.SetGameProgress(sceneNumber, loadedSaveData.GameProgress);
        }

        public void CallGameProgressChange(int newGameProgress)
        {
            gameManager.SetGameProgress(sceneNumber, newGameProgress);
        }

        private void OnGameProgressChange(int newGameProgress)
        {
            // TO DO:: Add 
            CutsceneManager.PlayerCutscene(newGameProgress);
        }
    }
}
