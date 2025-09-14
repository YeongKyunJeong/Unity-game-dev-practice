using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManagerNeo : MonoBehaviour
{ 
    #region Stage Information
    [SerializeField]
    private int level;
    public int LevelSetter { set => value = level; }
    public Paddle paddle;
    public Ball[] balls;
    public Brick[] bricks;
    public DeadZone[] walls;
    public bool isRandomItemSet = true;
    public int[] itemSettingWeight = new int[0];
    #endregion

    #region Fixed Parameter
    private static SFXType fallSFXType;
    private static LayerMask ballLayer;
    #endregion

    #region External Reference
    [SerializeField] private BrickData brickData;
    [SerializeField] private StageMaker stageMaker;
    public StageMaker StageMakerSetter { get => stageMaker; }

    private GameManager gameManager;
    public GameObject GameManagerPrefab;
    #endregion

    private void Awake()
    {
        CheckGameManager();
        CheckGameElemets();
        SendStageManagerToGameManager();
    }

    private void CheckGameElemets()
    {
        if (paddle == null)
        {
            paddle = FindFirstObjectByType<Paddle>();
        }
        if ((balls.Length == 0) || (balls[0] == null))
        {
            balls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        }
        if ((bricks.Length == 0) || (bricks[0]) == null)
        {
            bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
        }
        if(stageMaker == null)
        {
            stageMaker = FindFirstObjectByType<StageMaker>();
        }
    }

    private void CheckGameManager()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("No GameManager Detected");
            Instantiate(GameManagerPrefab);
        }
        else
        {
            Debug.Log("GameManager Detected");
        }
        gameManager = GameManager.Instance;

        // To do : Awake with data whose level is not 1;
    }

    private void SendStageManagerToGameManager()
    {
        gameManager.SendStageManager(this);
    }


    //#region StageEditing
    //[SerializeField] private GameObject brickPrefab;
    //[SerializeField] private GameObject brickRowPrefab;
    //[SerializeField] private Transform[] brickRows;
    //[SerializeField] private Transform brickRowParents;
    //public Transform GetBrickRowParent { get { return brickRowParents; } }

    //[HideInInspector]
    //[SerializeField] private string[] rowBrickHealths = new string[0];


    //[HideInInspector]
    //[SerializeField] private int brickRowCount = 5;
    //[HideInInspector]
    //[SerializeField] private int brickPerRow = 7;
    //[HideInInspector]
    //[SerializeField] private float brickSpaceX = 0.25f;
    //[HideInInspector]
    //[SerializeField] private float brickSpaceY = 0.25f;

    //private float defaultBrickHeight = 5.5f;
    //private float brickSizeX = 4;
    //private float brickSizeY = 1;

    //[HideInInspector]
    //public string savePath = "Assets/StageParameter";

    //public void GenerateBricks()
    //{
    //    MakeBricks(brickRowCount, brickPerRow, brickSpaceX, brickSpaceY);
    //}

    //public void MakeBricks(int brickRowCount, int brickPerRow, float brickSpaceX, float brickSpaceY)
    //{
    //    brickData.Initialize();
    //    if (brickRowParents == null)
    //    {
    //        brickRowParents = GameObject.Find("Bricks").transform;
    //    }

    //    ClearBricks(brickRowCount, brickPerRow);


    //    for (int i = 0; i < brickRowCount; i++)
    //    {

    //        brickRows[i] = Instantiate(brickRowPrefab, brickRowParents).transform;
    //        brickRows[i].localPosition = new Vector3(0, defaultBrickHeight + ((float)(brickRowCount - 1) / 2 - i) * (brickSpaceY + brickSizeY), 0);


    //        Brick[] tempBrick = brickRows[i].GetComponentsInChildren<Brick>();
    //        for (int j = 6; j > -1; j--)
    //        {
    //            int health = int.Parse(rowBrickHealths[i][j].ToString());
    //            if (j < brickPerRow)
    //            {
    //                bricks[brickPerRow * i + j] = tempBrick[j];
    //                tempBrick[j].transform.localPosition = new Vector3((-(float)(brickPerRow - 1) / 2 + j) * (brickSpaceX + brickSizeX), 0, 0);
    //                tempBrick[j].SetBrickParameter(health, true, brickData);
    //            }
    //            else
    //            {
    //                DestroyImmediate(tempBrick[j].gameObject);
    //            }
    //        }
    //    }
    //}

    //private void ClearBricks(int brickRowCount, int brickPerRow)
    //{
    //    if (brickRows.Length != 0)
    //    {
    //        for (int q = brickRows.Length - 1; q > -1; q--)
    //        {
    //            if (brickRows[q] != null)
    //                DestroyImmediate(brickRows[q].gameObject);
    //        }
    //    }

    //    brickRows = new Transform[brickRowCount];
    //    bricks = new Brick[brickRowCount * brickPerRow];
    //}

    //public void SaveStageParameter(string path)
    //{
    //    if (string.IsNullOrEmpty(path))
    //    {
    //        string folderPath = Path.Combine(Application.dataPath, savePath.TrimStart("Assets/".ToCharArray()));
    //        if (!Directory.Exists(folderPath))
    //        {
    //            Directory.CreateDirectory(folderPath);
    //            Debug.Log($"Created folder : {folderPath}");
    //        }

    //        if (level < 10)
    //        {
    //            path = Path.Combine(folderPath, $"Level0{level}.json");
    //        }
    //        else
    //        {
    //            path = Path.Combine(folderPath, $"Level{level}.json");
    //        }
    //    }
    //    SaveStageParameterToFile(path);
    //}

    //private void SaveStageParameterToFile(string path)
    //{
    //    StageParameter stageParameterData = new StageParameter
    //    {
    //        level = this.level,
    //        brickRowCount = this.brickRowCount,
    //        brickPerRow = this.brickPerRow,
    //        brickSpaceX = this.brickSpaceX,
    //        brickSpaceY = this.brickSpaceY,
    //        rowBrickHealths = this.rowBrickHealths
    //    };
    //    string json = JsonUtility.ToJson(stageParameterData, true);
    //    File.WriteAllText(path, json);
    //    Debug.Log($"Stage paramter are saved at {path}");
    //}


    //public void LoadStageParameter(string path)
    //{
    //    if (File.Exists(path))
    //    {
    //        LoadStageParameterFromFile(path);
    //    }
    //    else
    //    {
    //        Debug.LogError($"File not found : {path}");
    //    }
    //}

    //private void LoadStageParameterFromFile(string path)
    //{
    //    string json = File.ReadAllText(path);
    //    StageParameter loadedStageParameter = JsonUtility.FromJson<StageParameter>(json);

    //    ClearBricks(loadedStageParameter.brickRowCount, loadedStageParameter.brickPerRow);

    //    level = loadedStageParameter.level;
    //    brickRowCount = loadedStageParameter.brickRowCount;
    //    brickPerRow = loadedStageParameter.brickPerRow;
    //    brickSpaceX = loadedStageParameter.brickSpaceX;
    //    brickSpaceY = loadedStageParameter.brickSpaceY;
    //    rowBrickHealths = loadedStageParameter.rowBrickHealths;

    //    GenerateBricks();
    //}
    //#endregion
}


