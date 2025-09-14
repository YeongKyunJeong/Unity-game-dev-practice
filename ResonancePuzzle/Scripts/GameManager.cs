using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } private set { instance = value; } }

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private int screenWidth;
    [SerializeField]
    private int screenHeight;

    [SerializeField]
    private AspectRatioEnforcer aspectRatioEnforcer;

    [SerializeField]
    private JewelBoard jewelBoard;
    [SerializeField]
    private JewelData jewelData;
    [SerializeField]
    private UIManager uIManager;

    public int cycleCount = 10;

    public int myScore = 0;

    public int gameLevel;

    private void Awake()
    {
        GameManager.Instance = this;
        myScore = 0;
        gameLevel = 0;
        uIManager.Initialize(gameLevel);
        aspectRatioEnforcer.Initialize(uIManager.GetComponent<RectTransform>(), screenWidth, screenHeight);
        jewelData.Initialize();
        jewelBoard.Initialize(jewelData);
    }

    public void ScoreChangeCall(int resultScore)
    {
        myScore += resultScore;
        if(myScore < 0)
        {
            myScore = 0;
        }
        uIManager.ScoreChangeCall(myScore);
    }

    public void Restart()
    {
        if (sceneName == null)
        {
            Debug.Log("Scene Name Error");
        }
        else
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

    }

    public void GameOver()
    {
        uIManager.ButtonManager.continueButton.enabled = false;
        uIManager.ButtonManager.MenuBtnClick();
        Debug.Log("Game Over");
    }

    public void LevelChanged(int level)
    {
        gameLevel = level;
        jewelBoard.level = level;
    }
}
