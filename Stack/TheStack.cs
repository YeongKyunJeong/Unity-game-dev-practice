using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    public Text scoreText;
    public Color32[] gameColors = new Color32[4];
    public Material stackMat;
    public GameObject endPanel;
    public Transform rubblesParent;
    

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3;
    private const int STACK_COUNT = 12;

    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int stackIndex;
    private int scoreCount = 0;
    private int combo = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

    // Start is called before the first frame update
    void Start()
    {
        endPanel.SetActive(false);
        theStack = new GameObject[STACK_COUNT];
        for (int i = 0; i < STACK_COUNT; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }
        stackIndex = STACK_COUNT - 1;

    }
    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Transform tr = go.transform;
        tr.localPosition = pos;
        tr.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponent<MeshRenderer>().material = stackMat;
        tr.SetParent(rubblesParent); 

        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
                scoreText.text = scoreCount.ToString();

            }
            else
            {
                EndGame();
            }
        }

        MoveTile();
        // 스택 전체가 움직임
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);

    }

    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
        {
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
        }
    }

    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = STACK_COUNT - 1;
        }
        Transform transformNow = theStack[stackIndex].transform;

        desiredPosition = Vector3.down * scoreCount;
        transformNow.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
        transformNow.transform.localPosition = new Vector3(0, scoreCount, 0);
        ColorMesh(transformNow.GetComponent<MeshFilter>().mesh, -1);

    }

    private bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

        if (isMovingOnX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                // 타일 잘림
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                {
                    return false; // 게임 오버
                }

                float middle = (lastTilePosition.x + t.localPosition.x) / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                if (deltaX > 0)
                    CreateRubble(new Vector3(t.position.x - stackBounds.x / 2, t.position.y, lastTilePosition.z),
                        new Vector3(Mathf.Abs(deltaX), 1, stackBounds.y));

                else

                    CreateRubble(new Vector3(t.position.x + stackBounds.x / 2, t.position.y, lastTilePosition.z),
                        new Vector3(Mathf.Abs(deltaX), 1, stackBounds.y));

                t.localPosition = new Vector3(middle, scoreCount, lastTilePosition.z);
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUNDS_SIZE)
                    {
                        stackBounds.x = BOUNDS_SIZE;
                    }
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                // 타일 잘림
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                Debug.Log(stackBounds.y);
                if (stackBounds.y <= 0)
                {
                    return false; // 게임 오버
                }

                float middle = (lastTilePosition.z + t.localPosition.z) / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                if (deltaZ >= 0)
                    CreateRubble(new Vector3(lastTilePosition.x, t.position.y, t.position.z - stackBounds.y / 2),
                        new Vector3(stackBounds.x, 1, Mathf.Abs(deltaZ)));
                else
                    CreateRubble(new Vector3(lastTilePosition.x, t.position.y, t.position.z + stackBounds.y / 2),
                        new Vector3(stackBounds.x, 1, Mathf.Abs(deltaZ)));

                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle);
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUNDS_SIZE)
                    {
                        stackBounds.y = BOUNDS_SIZE;
                    }
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }

        secondaryPosition = isMovingOnX
            ? t.localPosition.x
            : t.localPosition.z;

        isMovingOnX = !isMovingOnX;

        return true;
    }

    private void ColorMesh(Mesh mesh, int  correction = 0)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin((scoreCount - correction) * 0.25f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }

        mesh.colors32 = colors;
    }

    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
        {
            return Color.Lerp(a, b, t / 0.33f);
        }
        else if (t < 0.66f)
        {
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        }
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.33f);
    }

    private void EndGame()
    {
        if(PlayerPrefs.GetInt("score") < scoreCount)
        {
            PlayerPrefs.SetInt("score", scoreCount);
        }
        gameOver = true;
        endPanel.SetActive(true);
        theStack[stackIndex].AddComponent<Rigidbody>();
    }

    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
