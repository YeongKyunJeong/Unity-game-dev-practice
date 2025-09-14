using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI levelTMP;
    public int levelSetter
    {
        get { return levelInt; }
        set
        {
            levelInt = value;
            levelTMP.text = value.ToString();
            if (gameManager != null)
            {
                gameManager.LevelChanged(value);
            }
        }
    }
    private int levelInt;

    public TextMeshProUGUI goalScore;
    public int goalScoreSetter
    {
        get { return goalScoreInt; }
        set
        {
            goalScoreInt = value <= 99999999 ? value : 99999999;
            goalScore.text = ScoreTextSet(goalScoreInt);
        }
    }
    private int goalScoreInt;

    public TextMeshProUGUI myScore;
    public int myScoreSetter
    {
        get { return myScoreInt; }
        set
        {
            myScoreInt = value <= 99999999 ? value : 99999999;
            gameManager.myScore = myScoreInt;
            myScore.text = ScoreTextSet(myScoreInt);
        }
    }
    private int myScoreInt;


    public TextMeshProUGUI chance;
    public int chanceSetter
    {
        get { return chanceInt; }
        set
        {
            chance.text = ChanceTextSet(value);
        }
    }
    private int chanceInt;

    private string stringForWork = "";
    private int intForWork = 0;

    private GameManager gameManager;

    public void Initialize(int startLevel, int initGoalScore, int initChance = 6, int initMyScore = 0)
    {
        gameManager = GameManager.Instance;
        levelSetter = startLevel;
        goalScoreSetter = initGoalScore;
        myScoreSetter = initMyScore;
        chanceSetter = initChance;
    }

    private string ChanceTextSet(int rawInt, int totalChance = 6)
    {
        if (rawInt == 0)
        {
            if (CheckGameEnd())
            {
                chanceInt = totalChance;
            }
            else
            {
                chanceInt = totalChance;
                return chanceInt.ToString();
            }
        }
        else
        {
            chanceInt = rawInt;
        }
        stringForWork = "";
        stringForWork = chanceInt.ToString() + " / " + totalChance.ToString();
        return stringForWork;
    }

    private string ScoreTextSet(int rawInt, int maxLength = 8)
    {
        stringForWork = rawInt.ToString();
        intForWork = stringForWork.Length;
        for (int i = 0; i < maxLength - intForWork; i++)
        {
            stringForWork = "0" + stringForWork;
        }
        return stringForWork;
    }

    private bool CheckGameEnd()
    {
        if (myScoreInt > goalScoreInt)
        {
            levelSetter++;
            // юс╫ц
            if (goalScoreInt < 99999999)
                goalScoreSetter *= 2;
            else
            {
                goalScoreSetter = 99999999;
                myScoreSetter = 0;
            }
            return true;
        }
        else
        {
            gameManager.GameOver();
            return false;
        }
    }
}
