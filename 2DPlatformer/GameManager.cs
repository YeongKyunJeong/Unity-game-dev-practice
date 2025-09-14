using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get { return instance; }

        private set { instance = value; }
    }
    private static GameManager instance;

    #region Stage
    public GameObject[] stages;
    public Vector3[] spawnPosition;
    #endregion

    #region UI
    public Image[] healthImagesUI;
    public Text pointsUI;
    public Text stageUI;
    public Button retruBtn;
    #endregion

    #region ExternalRef
    // ExternalRef
    public PlayerMove player;
    #endregion

    #region Parameter
    public int totalPoint = 0;
    public int stagePoint = 0;
    public int stageIndex = 0;
    private int health = 3;
    private Color enabledColor = new Color(1, 1, 1, 0.2f);
    public int Health
    {
        get { return health; }
        set
        {
            if (value < 3)
            {
                healthImagesUI[value ].color = enabledColor;
            }
            if(value == 0)
            {
                retruBtn.gameObject.SetActive(true);
            }
            health = value;
        }
    }
    #endregion
    private void Awake()
    {
        Instance = this;
        totalPoint = 0;
        stagePoint = 0;
        stageIndex = 0;
        pointsUI.text = "0";
        stageUI.text = "Stage 1";
        foreach (var item in healthImagesUI)
        {
            item.gameObject.SetActive(true);
        }
        Health = 3;
        if (spawnPosition.Length < stages.Length)
        {
            Vector3[] temp = new Vector3[stages.Length];
            for (int i = 0; i < spawnPosition.Length; i++)
            {
                temp[i] = spawnPosition[i];
            }
            for (int i = spawnPosition.Length; i < stages.Length; i++)
            {
                temp[i] = spawnPosition[0];
            }
            spawnPosition = temp;
        }
        for (int i = 0; i < stages.Length; i++)
        {
            if (i == 0)
            {
                stages[i].SetActive(true);
            }
            else
                stages[i].SetActive(false);
        }
        retruBtn.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindFirstObjectByType<PlayerMove>();
        }
    }

    //private void Update()
    //{
    //    pointsUI.text = (totalPoint + stagePoint).ToString();
    //}

    public void UpdatePoints()
    {
        pointsUI.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex++].SetActive(false);

            stages[stageIndex].SetActive(true);
            stageUI.text = "Stage " + (stageIndex + 1).ToString();

            player.transform.position = spawnPosition[stageIndex];

            totalPoint += stagePoint;
            stagePoint = 0;
        }
        else
        {
            Time.timeScale = 0;
            retruBtn.gameObject.SetActive(true);
            player.PlaySound(player.audioFinish);
            Debug.Log("Game Clear");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            player.transform.position = new Vector3(-6.5f, -1.5f, 0);
            player.OnFalling();
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
