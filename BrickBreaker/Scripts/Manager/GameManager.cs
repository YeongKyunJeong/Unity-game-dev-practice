using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Collections;
using System.Text;

public enum SceneType
{
    Title,
    Stage
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    #region Magic Number
    private const string STAGE_SCENE_STRING = "1StageScene";
    private const string TITLE_SCENE_STRING = "0TitleScene";
    private const int TITLE_SCENE_INT = 0;
    private const int MAX_LIFE = 99;
    private const int MAX_SCORE = 99999999;
    private const int LIFE_TO_SCORE = 10000;

    private SFXType fallSFXTye = SFXType.Fall;
    private int totalSceneCount = 0;
    private LayerMask ballLayer;
    public LayerMask BallLayerGetter { get => ballLayer; }
    private LayerMask paddleLayer;
    public LayerMask PaddleLayerGetter { get => paddleLayer; }
    private LayerMask deadZoneLayer;
    public LayerMask DeadZoneLayerGetter { get => deadZoneLayer; }
    private LayerMask bricksLayer;
    public LayerMask BricksLayerGetter { get => bricksLayer; }

    [SerializeField] private float powerUpContinuanceTime = 10f;
    private WaitForSeconds powerUpWaitForSec;
    #endregion

    #region Player Status
    public SceneType nowScene;
    public int myScoreAt_StageStart = 0;
    private int myScoreAt_GameStart = 0;
    public int livesAt_StageStart = 3;
    private int livesAt_GameStart = 3;

    public int level = 0;
    public int myScore = 0;
    public int lives = 3;

    private int brickDamageAtGameStart = 1;
    public int brickDamage = 1;

    [SerializeField] private int leftBrickCount = -1;
    [SerializeField] private int leftBallCount = -1;
    private bool stageCleared = false;
    #endregion

    #region Boolean
    [SerializeField] private bool isNewGame = true;
    [SerializeField] private bool isMainMenuOn = false;
    #endregion

    #region Logic Parameter
    private Item[] itemSetting;
    private string tempString = null;
    private int tempInt = 0;
    private int tempInt2 = 0;
    private int totalWeight = 0;

    private int multiBallNumber = 5;

    //private Coroutine[] PowerUpCoroutines;
    private Coroutine PowerUpCoroutine;
    private List<Coroutine> CoroutineLists = new List<Coroutine>();
    public Action ResetBallAction;
    public Action ResetDroppingItemAction;
    public Action<int> BallPowerChangeAction;

    #endregion

    #region External Reference
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private DroppingItem droppingItemPrefab;
    private DroppingItem droppingItemInitializer;
    [SerializeField] private Ball ballPrefab;
    private Ball ballInitializer;
    [SerializeField] private Paddle paddlePrefab;
    private Paddle paddleInitializer;

    [SerializeField] private BrickData brickData;
    public BrickData BrickDataGetter { get => brickData; }

    public TitleScene titleScene;

    private static int itemTypeNumber;
    private int[] defaultItemProbability;

    [SerializeField] private ObjectPool objectPool;

    #region Stage Object
    private StageMaker stageMaker;
    [SerializeField] private Paddle paddle;
    [SerializeField] private Ball[] balls;
    [SerializeField] private Brick[] bricks;
    [SerializeField] private DeadZone[] walls;
    [SerializeField] private StageManagerNeo stageManagerNeo;
    #endregion

    #endregion

    public static GameManager Instance { get { return instance; } private set { instance = value; } }
    private void Awake()
    {
        if (instance == null)
        {
            Instance = this;
        }
        else
        {
            instance.titleScene = this.titleScene;
            titleScene.Initialize();
            Destroy(this.uiManager.gameObject);
            Destroy(this.gameObject);
            return;
        }

        nowScene = SceneManager.GetActiveScene().name == TITLE_SCENE_STRING ? SceneType.Title : SceneType.Stage;

        switch (nowScene)
        {
            case SceneType.Title:
                {
                    DontDestroyOnLoad(gameObject);
                    totalSceneCount = SceneManager.sceneCountInBuildSettings;
                    Debug.Log(totalSceneCount);
                    break;
                }
            case SceneType.Stage: { break; }
            default: break;
        }

        ballLayer = LayerMask.NameToLayer("Ball");
        paddleLayer = LayerMask.NameToLayer("Paddle");
        deadZoneLayer = LayerMask.NameToLayer("DeadZone");
        bricksLayer = LayerMask.NameToLayer("Bricks");
        powerUpWaitForSec = new WaitForSeconds(powerUpContinuanceTime);
        isMainMenuOn = false;

        PowerUpCoroutine = null;
        //PowerUpCoroutines = null
        //;
        //objectPool.Initialize();
        Initialize();
    }

    private void Initialize()
    {
        //brickData = Resources.Load<BrickData>("Address");
        if (brickData == null)
        {
            Debug.LogError("BrickData asset not detected");
        }
        brickData.Initialize();
        itemTypeNumber = brickData.totalItemTypeCount;
        //defaultItemProbability = brickData.defaultItemProbability; // Build version 
        defaultItemProbability = new int[4] { 0, 1, 1, 3 }; // For test
        totalWeight = 0;
        foreach (int weight in defaultItemProbability)
        {
            totalWeight += weight;
        }

        if (uiManager == null)
        {
            uiManager = Instantiate(uiManagerPrefab).GetComponent<UIManager>();
        }
        uiManager.Initialize(nowScene);

        if (soundManager == null)
        {
            soundManager = FindFirstObjectByType<SoundManager>();
        }
        soundManager.Initialize(brickData);

        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
        }
        inputManager.Initialize();

        //if (droppingItemPrefab == null)
        //{
        //    Debug.LogError("DropingItemPrefab not detected");
        //}
        //droppingItemInitializer = droppingItemPrefab;
        //droppingItemInitializer.GlobalInitialize(brickData, paddleLayer, deadZoneLayer);
        //droppingItemInitializer = null;

        //if (ballPrefab == null)
        //{
        //    Debug.LogError("BallPrefab not detected");
        //}
        //ballInitializer = ballPrefab;
        //ballInitializer.GlobalInitialize(brickData);
        //ballInitializer = null;

        //if (paddlePrefab == null)
        //{
        //    Debug.LogError("PaddlePrefab not detected");
        //}
        //paddleInitializer = paddlePrefab;
        //droppingItemInitializer.GlobalInitialize(brickData, paddleLayer, deadZoneLayer);
        //droppingItemInitializer = null;

        InputManager.OnESCInput += ESCCall;
        //SceneManager.sceneLoaded += OnSceneLoaded;

        if (nowScene == SceneType.Title)
        {
            if (titleScene == null)
            {
                titleScene = FindFirstObjectByType<TitleScene>();
            }
            titleScene.gameObject.SetActive(true);
            titleScene.Initialize();
        }
        else
        {

        }
    }

    public void StartGameCall()
    {
        StartGame();
    }

    private void StartGame()
    {
        //// To do : Title scene -> Stage scene
        uiManager.gameObject.SetActive(true);
        if (isNewGame)
        {
            StartNewGame();
        }
        else
        { }// # To do : Add other game starting options;
    }

    public void OpenSetting()
    {

    }

    public void QuitGameCall()
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

    public void BackToTitleSceneCall()
    {
        BackToTitleScene();
    }

    private void BackToTitleScene()
    {
        uiManager.gameObject.SetActive(false);
        LoadSceneOrStage(SceneType.Title, -1);
        //LoadLevel(TITLE_SCENE_INT);
    }

    public void SendStageManager(StageManagerNeo stageManagerNeo)
    {
        this.stageManagerNeo = stageManagerNeo;
        GetDataFromStageManager(true);
    }

    public void GetDataFromStageManager(bool isFirstSettingAfterSceneChange = false)
    {
        bricks = stageManagerNeo.bricks;
        brickDamage = brickDamageAtGameStart;

        leftBrickCount = bricks.Length;

        MakeItemSetting(stageManagerNeo.isRandomItemSet, stageManagerNeo.itemSettingWeight);
        for (int i = 0; i < leftBrickCount; i++)
        {
            bricks[i].Initialize(ballLayer, brickDamage, brickData, itemSetting[i]);
        }


        if (isFirstSettingAfterSceneChange)
        {
            stageMaker = stageManagerNeo.StageMakerSetter;

            walls = stageManagerNeo.walls;
            foreach (DeadZone wall in walls)
            {
                wall.Initialize(ballLayer);
            }

            balls = stageManagerNeo.balls;
            for (int i = 0; i < balls.Length; i++)
            {
                ballInitializer = balls[i];
                ballInitializer.Initialize();
                ballInitializer.SetBallPower(brickDamage, i == 0);
            }

            paddle = stageManagerNeo.paddle;
            paddle.Initialize();
        }


        //DoUIManagerSetting(level, lives, myScore);
        objectPool.Initialize();
    }

    private void MakeItemSetting(bool isRandomSetting, int[] givenItemProbability)
    {
        if (givenItemProbability.Length != itemTypeNumber)
        {
            givenItemProbability = defaultItemProbability;
        }

        itemSetting = new Item[leftBrickCount];
        if (isRandomSetting)
            for (int i = 0; i < leftBrickCount; i++)
            {
                itemSetting[i] = (Item)WeightedRandom(givenItemProbability);
            }
    }

    private int WeightedRandom(int[] weights)
    {
        tempInt2 = Random.Range(0, totalWeight);
        tempInt = 0;
        for (int i = 0; i < itemTypeNumber; i++)
        {
            tempInt += weights[i];
            if (tempInt2 < tempInt)
            {
                return i;
            }
        }
        Debug.LogError("Item setting probability error");
        return -1; // errror
    }

    private void StartNewGame()
    {
        myScore = myScoreAt_GameStart;
        lives = livesAt_GameStart;

        LoadSceneOrStage(SceneType.Stage, 1);
        //LoadLevel(1);
    }

    private void LoadSceneOrStage(SceneType targetSceneType, int targetLevel)
    {
        bool isSceneChange = true;
        bool isScoreUIOn = true;
        bool isStageDataLoad = true;


        bool playerStateSave = false;
        bool playerStateTo_GameStart = false;
        bool playerStateTo_StageStart = false;

        switch (targetSceneType)
        {
            case SceneType.Title:   // Only Stage - to - Title yet
                {
                    tempString = TITLE_SCENE_STRING;
                    isScoreUIOn = false;
                    playerStateTo_GameStart = true;
                    isStageDataLoad = false;

                    //if (nowScene == SceneType.Stage)    // Stage - to - Title
                    //{

                    //}
                    //else if (nowScene == SceneType.Title)    // Title - to - Title
                    //{
                    //    isSceneChange = false;
                    //    // To do : Add game play data saving logic
                    //}

                    break;
                }
            case SceneType.Stage:
                {
                    tempString = STAGE_SCENE_STRING;
                    isScoreUIOn = true;

                    if (nowScene == SceneType.Stage)    // Stage - to - Stage
                    {
                        isSceneChange = false;

                        if (this.level == targetLevel) // Only retry now stage // not used
                        {
                            playerStateTo_StageStart = true;
                            isStageDataLoad = false;
                        }
                        else // Change stage
                        {
                            playerStateSave = true;
                        }
                    }
                    else if (nowScene == SceneType.Title)    // Title - to - Stage
                    {
                        if (isNewGame)  // New game only yet
                        {
                            playerStateTo_GameStart = true;
                        }
                        else
                        {
                            // To do : Add game play data loading logic
                        }
                    }
                    break;
                }
            default:
                {
                    try
                    {
                        throw new Exception("Not implemented scene loaded");
                    }
                    catch
                    {
                        break;
                    }
                }
        }

        level = targetLevel;
        if (playerStateSave)
        {
            myScoreAt_StageStart = myScore;
            livesAt_StageStart = lives;
        }
        else if (playerStateTo_GameStart)
        {
            myScore = myScoreAt_GameStart;
            myScoreAt_StageStart = myScoreAt_GameStart;
            lives = livesAt_GameStart;
            livesAt_StageStart = livesAt_GameStart;

        }
        else if (playerStateTo_StageStart)
        {
            myScore = myScoreAt_StageStart;
            lives = livesAt_StageStart;
        }

        if (isSceneChange)
        {
            SceneManager.LoadScene(tempString);
        }
        else if (isStageDataLoad)
        {
            tempString = null;
            if (targetLevel < 10)
                tempString = $"{Application.dataPath}/StageParameter/Level0{targetLevel}.json";
            else
                tempString = $"{Application.dataPath}/StageParameter/Level{targetLevel}.json";
            if (!string.IsNullOrEmpty(tempString))
            {
                stageMaker.LoadStageParameter(tempString);
                GetDataFromStageManager();
            }
            else
            {
                Debug.Log("Stage data not detected");
            }
        }


        uiManager.gameObject.SetActive(isScoreUIOn);
        AllStageChangeCommonInitialize();
        nowScene = targetSceneType;

        if (nowScene == SceneType.Title)
        {
            OnlyToTitle();
        }
        else
        {
            OnlyToStage();
        }

    }

    private void AllStageChangeCommonInitialize()
    {
        ResetPowerUpCoroutine();
        stageCleared = false;
        isMainMenuOn = false;
        TimeControler.TimeScaler(1);
    }

    private void OnlyToTitle()
    {
        ResetBallAction = null;
        BallPowerChangeAction = null;
        ResetDroppingItemAction = null;
        balls = null;
        paddle = null;
        bricks = null;
        walls = null;
    }

    private void OnlyToStage()
    {
        uiManager.DoUIManagerSetting(level, lives, myScore);
        ResetDroppingItemAction?.Invoke();
        ResetPaddleCall();
        ResetBallCall();
        ResetBricks();
    }

    //private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    //{
    //    if (loadedScene == SceneManager.GetSceneByBuildIndex(0))
    //    {
    //        //titleScene = FindFirstObjectByType<TitleScene>();
    //        //titleScene.Initialize();
    //    }
    //}

    public void HitBrick(int score, Vector3 brokenBrickPosition, bool isBroken = false, Item targetItem = Item.None)
    {
        myScore += score;
        uiManager.ChangeNumber(myScore);

        if (isBroken)
        {
            leftBrickCount--;
            if (leftBrickCount == 0)
            {
                stageCleared = true;
            }
            else if (targetItem != Item.None)
            {
                CreateDroppingItem(brokenBrickPosition, targetItem);
            }
        }
        if (stageCleared)
        {
            //if (level < totalSceneCount)
            //{
            //    level++;
            //    uiManager.ChangeNumber(level, UINumberCategory.Level);
            //}
            LoadSceneOrStage(SceneType.Stage, level + 1);
            //LoadLevel(level);
        }
    }

    public void PlaySFX(SFXType inputSFXType)
    {
        soundManager.PlaySFX(inputSFXType);
    }

    public void ESCCall()
    {
        if (isMainMenuOn)
        {
            isMainMenuOn = false;
            ResumeCall();
        }
        else
        {
            isMainMenuOn = true;
            PauseCall();
        }
    }

    public void ResumeCall()
    {
        ResumeGame();
    }

    private void ResumeGame()
    {
        TimeControler.TimeScaler(1);
        isMainMenuOn = false;
        uiManager.ResumeGame();
    }

    public void PauseCall()
    {
        PauseGame();
    }

    private void PauseGame()
    {
        TimeControler.TimeScaler(0);
        uiManager.PauseGame();
    }

    public void ResetCall(bool isFullReset)
    {
        if (isFullReset)
        {
            LoadSceneOrStage(SceneType.Stage, 1);
            //StartNewGame();
        }
        else
        {
            LoadSceneOrStage(SceneType.Stage, level);
        }
    }

    //private void ResetStage()
    //{
    //    ResetPowerUpCoroutine();

    //    myScore = myScoreAt_StageStart;
    //    lives = livesAt_StageStart;
    //    isMainMenuOn = false;

    //    TimeControler.TimeScaler(1);
    //    ResetDroppingItemAction?.Invoke();
    //    uiManager.DoUIManagerSetting(level, myScore, lives);
    //    ResetPaddleCall();
    //    ResetBallCall();
    //    ResetBricks();

    //}

    //private void ResetItemDropBoxes()
    //{
    //    for (int i = 0; i < enabledDroppingItems.Count; i++)
    //    {
    //        enabledDroppingItems[i].DisableByReset();
    //    }
    //}

    private void ResetBricks()
    {
        if (bricks != null)
        {
            leftBrickCount = bricks.Length;
            foreach (Brick brick in bricks)
            {
                brick.ResetBrick();
            }

        }
    }

    public void ResetBallCall()
    {
        leftBallCount = 1;
        ResetBallAction?.Invoke();
    }

    public void ResetPaddleCall()
    {
        if (paddle != null)
            paddle.ResetPaddle();
    }

    public void DeadZoneOut(GameObject maybeBall)
    {
        if (leftBallCount > 1)
        {
            leftBallCount--;
            maybeBall.gameObject.SetActive(false);
        }
        else
        {
            lives--;
            leftBallCount = 1;
            PlaySFX(fallSFXTye);
            uiManager.ChangeNumber(lives, UINumberCategory.Life);

            if (lives > 0)
            {
                ResetBallCall();
            }
            else
            {
                TimeControler.TimeScaler(0);
                uiManager.GameOver();
                Debug.Log("Game Over");
            }
        }
    }

    public void ItemGettodaze(Item item)
    {
        switch (item)
        {
            case Item.PowerUp:
                {
                    ResetPowerUpCoroutine();
                    PowerUpCoroutine = StartCoroutine(BrickDamagerUp());

                    break;
                }
            case Item.LifeUp:
                {
                    LifeUp();
                    break;
                }
            case Item.MultiBall:
                {
                    MultiBallStackUp();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void MultiBallStackUp()
    {
        Ball.multiBallStack++;
        //highestHeight = -16;
        //highestBallIndex = 0;
        //leftBallCount = balls.Length;
        //for (int i = 0; i < balls.Length; i++)
        //{
        //    if (balls[i].gameObject.activeInHierarchy)
        //        if (balls[i].transform.position.y > highestHeight)
        //        {
        //            highestHeight = balls[i].transform.position.y;
        //            highestBallIndex = i;
        //        }
        //}

        //balls[highestBallIndex].ReadyMultiBall();
        //for (int i = 0; i < balls.Length; i++)
        //{
        //    balls[i].MakeMultiBall();
        //}





        //for (int i = 0; i < balls.Length; i++)
        //{
        //    balls[i].MakeMultiBall(balls[highestBallIndex]);
        //}
    }

    public void MakeMultiballCall()
    {
        for (int i = 0; i < multiBallNumber - 1; i++)
        {
            ballInitializer = objectPool.GetObject<Ball>(PoolObjectType.Ball);
            ballInitializer.SetBallPower(brickDamage);
            ballInitializer.BeMultiBall();
            leftBallCount++;
        }
    }

    private void LifeUp(int deltaLife = 1)
    {
        if (lives < MAX_LIFE)
        {
            lives += deltaLife;
            if (lives > MAX_LIFE)
            {
                lives = MAX_LIFE;
            }
            uiManager.ChangeNumber(lives, UINumberCategory.Life);
        }
        else if (myScore < MAX_SCORE)
        {
            myScore += LIFE_TO_SCORE;
            uiManager.ChangeNumber(myScore);
        }
    }

    IEnumerator BrickDamagerUp(int upedPower = 2)
    {
        if (upedPower > brickDamage)
        {
            brickDamage = upedPower;
        }
        bricks[0].BrickDamagerSetter = brickDamage;
        BallPowerChangeAction?.Invoke(brickDamage);
        //for (int i = 0; i < balls.Length; i++)
        //{
        //    if (balls[i].gameObject.activeInHierarchy)
        //    {
        //        balls[i].ChangeColor(brickDamage);
        //    }
        //}

        yield return powerUpWaitForSec;

        brickDamage = 1;
        bricks[0].BrickDamagerSetter = brickDamage;
        BallPowerChangeAction?.Invoke(brickDamage);
        //for (int i = 0; i < balls.Length; i++)
        //{
        //    if (balls[i].gameObject.activeInHierarchy)
        //    {
        //        balls[i].ChangeColor(brickDamage);
        //    }
        //}

        yield return null;
    }

    private void ResetPowerUpCoroutine()
    {
        if (PowerUpCoroutine != null)
        {
            StopCoroutine(PowerUpCoroutine);
            PowerUpCoroutine = null;
        }
    }

    public void CreateDroppingItem(Vector3 creationPosition, Item targetItem)
    {
        droppingItemInitializer = objectPool.GetObject<DroppingItem>(PoolObjectType.DroppingItemBox);
        droppingItemInitializer.SetPositionAndItemType(creationPosition, targetItem);
    }

    public void TakeSettingValue(int value, TitleSceneSetterType setterType)
    {
        switch (setterType)
        {
            case TitleSceneSetterType.Life:
                {
                    livesAt_GameStart = value;
                    break;
                }
            default:
                break;
        }
    }

}

public static class TimeControler
{
    public static void TimeScaler(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}

