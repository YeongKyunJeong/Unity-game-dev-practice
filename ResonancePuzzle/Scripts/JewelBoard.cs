using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelBoard : MonoBehaviour
{

    #region Arrange Before Runtime
    public bool DoArrangeJewel = false;
    public float gap = 1.2f;
    #endregion

    #region External Reference
    public Camera mainCamera;

    private GameManager gameManager;

    [SerializeField] private JewelData jewelData;
    [SerializeField] private List<Jewel> jewels;
    [SerializeField] private List<Transform> jewelTransforms;
    [SerializeField] private List<JewelRoom> jewelRooms;
    #endregion

    //#region Internal Reference
    //private JewelInputControler jewelInputControler;

    //#endregion

    #region Static Parameter
    private static Color[] selectableSignColors = new Color[3];
    private static int puzzlSize = 7;
    #endregion

    #region Parameter for Logic
    // Input
    private Vector3 mouseClickedPosition;
    private Collider2D clickedColl;

    // State
    public int level = 0;
    [SerializeField] private int itemCount = 0;
    [SerializeField] private int trapCount = 0;
    [SerializeField] private int chainDir = 5; // 5: 클릭 불가, 1: ↙, 2: ↓. 3: ↘, 4: ←, 6: →, 7: ↖, 8: ↑, 9: ↗
    public JewelRoom nowHeldJewel = null;

    // History
    public List<List<JewelRoom>> selectableRoomsSets = new List<List<JewelRoom>>() { };
    public List<JewelRoom> chainedRooms = new List<JewelRoom>();
    [SerializeField]
    private List<int> chainDirHistory = new List<int>();
    #endregion

    #region Temporary Parameter Caching
    public List<JewelRoom> tempJewelList;
    private int[] defaultNextJewelTypeIDs = new int[puzzlSize];
    private int[] tempIntArray = new int[puzzlSize];
    private int[] trapActivatedArray;
    private int[] trapDefaultActivatedArray;
    private int[] itemActivatedArray;
    private int[] itemDefaultActivatedArray;
    private int tempInd = 0;
    private int tempInd2 = 0;
    private int tempInd3 = 0;
    public JewelRoom tempJewelRoom;
    Vector2Int tempVector2Int = Vector2Int.zero;
    private int xDiff = 0;
    private int yDiff = 0;
    #endregion

    #region Trigger
    [SerializeField] private bool isActivating = false;
    [SerializeField] private bool isDeactivating = false;
    private bool updateJewel = false;
    #endregion

    #region Layer
    private int enabledRoomLayer;
    private int enabledRoomLayerMask;
    private int disabledRoomLayer;
    #endregion

    #region Strings Caching
    private string isSelected = "isSelected";
    public const string EnabledRoom = "EnabledRoom";
    public const string DisabledRoom = "DisabledRoom";
    #endregion

    #region Not Uesd Yet
    private Vector2Int nowHeldJewelInd = -Vector2Int.one;
    #endregion

    #region Parameter for Debugging
    public int testCase = -1;
    #endregion

    //private void OnValidate()
    //{
    //    if (DoArrangeJewel)
    //    {
    //    }
    //}
    public void Initialize(JewelData jewelData)
    {
        //jewelInputControler = GetComponent<JewelInputControler>();
        this.jewelData = jewelData;
        gameManager = GameManager.Instance;
        level = gameManager.gameLevel;

        trapActivatedArray = new int[jewelData.trapSprites.Length];
        trapDefaultActivatedArray = new int[jewelData.trapSprites.Length];

        itemActivatedArray = new int[jewelData.itemSprites.Length];
        itemDefaultActivatedArray = new int[jewelData.itemSprites.Length];

        enabledRoomLayerMask = LayerMask.GetMask(EnabledRoom);
        enabledRoomLayer = LayerMask.NameToLayer(EnabledRoom);
        disabledRoomLayer = LayerMask.NameToLayer(DisabledRoom);
        chainDir = 5;
        for (int i = 0; i < puzzlSize; i++)
        {
            defaultNextJewelTypeIDs[i] = -1;
        }
        selectableRoomsSets = new List<List<JewelRoom>>() { };

        chainedRooms = new List<JewelRoom>();
        chainDirHistory = new List<int>();

        if (testCase == -1)
        {
            testCase = 1;
        }
        for (int i = 0; i < puzzlSize; i++)
        {
            for (int j = 0; j < puzzlSize; j++)
            {
                jewelRooms[puzzlSize * i + j].Initialize(jewels[puzzlSize * i + j], new Vector2Int(j, i), jewelData);

                //jewels[puzzlSize * i + j].cord = new Vector2Int(j, i);
                //jewels[puzzlSize * i + j].Initialize(jewelData);
            }
        }
    }

    private void Start()
    {

        selectableSignColors[0] = jewelRooms[0].spriteRenderer.color;
        Color temp = new Color(selectableSignColors[0].r, selectableSignColors[0].g, selectableSignColors[0].b, 0);
        selectableSignColors[1] = temp;
        selectableSignColors[2] = 0.4f * Color.yellow;

        UpdateSelectable();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inputTrigger = true;
        }


        if (Input.GetMouseButtonUp(0))
        {
            inputTrigger = false;

            if (updateJewel)
            {
                ConfirmActivationJewel();

                if (chainedRooms.Count >= puzzlSize)
                {
                    Resonance(chainedRooms);
                }

            }

            EndClick();
        }
    }

    private void Resonance(List<JewelRoom> completedJewelRooms)
    {
        PopJewel(completedJewelRooms);
        EliminateNearbyTrap();
        DeactivateJewel(completedJewelRooms[0]);
        UpdateSelectable();
    }

    private void EliminateNearbyTrap()
    {
        // check four nearby jewels of poped jewels
        for (int i = 0; i < chainedRooms.Count; i++)
        {
            tempVector2Int = chainedRooms[i].cord;
            tempInd = -1;

            if (tempVector2Int.x > 0)   // Left jewel
            {
                tempJewelRoom = jewelRooms[tempVector2Int.x - 1 + tempVector2Int.y * puzzlSize];
                if (!chainedRooms.Contains(tempJewelRoom))
                    if (tempJewelRoom.jewelType >= 10 && tempJewelRoom.jewelType < 20)
                    {
                        tempJewelRoom.JewelUpdate(false, Random.Range(0, 7));
                    }
            }

            if (tempVector2Int.x < puzzlSize - 1)   // Right jewel
            {
                tempJewelRoom = jewelRooms[tempVector2Int.x + 1 + tempVector2Int.y * puzzlSize];
                if (!chainedRooms.Contains(tempJewelRoom))
                    if (tempJewelRoom.jewelType >= 10 && tempJewelRoom.jewelType < 20)
                    {
                        tempJewelRoom.JewelUpdate(false, Random.Range(0, 7));
                    }
            }
            if (tempVector2Int.y > 0)   // Above jewel
            {
                tempJewelRoom = jewelRooms[tempVector2Int.x + (tempVector2Int.y - 1) * puzzlSize];
                if (!chainedRooms.Contains(tempJewelRoom))
                    if (tempJewelRoom.jewelType >= 10 && tempJewelRoom.jewelType < 20)
                    {
                        tempJewelRoom.JewelUpdate(false, Random.Range(0, 7));
                    }
            }
            if (tempVector2Int.y < puzzlSize - 1)   // Bellow jewel
            {
                tempJewelRoom = jewelRooms[tempVector2Int.x + (tempVector2Int.y + 1) * puzzlSize];
                if (!chainedRooms.Contains(tempJewelRoom))
                    if (tempJewelRoom.jewelType >= 10 && tempJewelRoom.jewelType < 20)
                    {
                        tempJewelRoom.JewelUpdate(false, Random.Range(0, 7));
                    }
            }

        }

    }

    private void PopJewel(List<JewelRoom> targetJewels, int[] nextJewelTypeIDs = null)
    {
        tempInd = targetJewels.Count;
        tempIntArray = new int[tempInd];

        if (nextJewelTypeIDs == null)
        {
            nextJewelTypeIDs = new int[defaultNextJewelTypeIDs.Length];
            defaultNextJewelTypeIDs.CopyTo(nextJewelTypeIDs, 0);

            if (level >= 10)
            {
                tempInd2 = Random.Range(0, targetJewels.Count);
                nextJewelTypeIDs[tempInd2] = Random.Range(0, 1) + 10;
                do
                {
                    tempInd3 = Random.Range(0, targetJewels.Count);
                } while (tempInd3 != tempInd2);
                nextJewelTypeIDs[tempInd3] = Random.Range(0, 1) + 10;

            }
            else if (level >= 1)
            {
                tempInd2 = Random.Range(0, targetJewels.Count);
                nextJewelTypeIDs[tempInd2] = Random.Range(0, 1) + 10;
            }

        }
        itemDefaultActivatedArray.CopyTo(itemActivatedArray, 0);
        trapDefaultActivatedArray.CopyTo(trapActivatedArray, 0);

        for (int i = 0; i < tempInd; i++)
        {
            tempInd2 = targetJewels[i].jewelType;

            if (tempInd2 < 9)
            {
                tempIntArray[targetJewels[i].jewelType]++;
            }
            else if (tempInd2 > 19)
            {
                itemActivatedArray[tempInd2 - 20]++; // 터진 아이템 개수
            }
            else if (tempInd2 > 9)
            {
                trapActivatedArray[tempInd2 - 10]++; // 터진 함정 개수
            }

            targetJewels[i].JewelUpdate(true, nextJewelTypeIDs[i]);
        }

        PopingToScore(tempIntArray, trapActivatedArray, itemActivatedArray);
    }

    public void PopingToScore(int[] popingJewelTypeCounts, int[] popingTrapTypeCounts, int[] popingItemTypeCounts) ///////////////////
    {
        Debug.ClearDeveloperConsole();
        tempInd = 0;
        for (int i = 0; i < popingJewelTypeCounts.Length; i++)
        {
            if (popingJewelTypeCounts[i] > 1)
            {
                tempInd += 10 * (int)Mathf.Pow(2, popingJewelTypeCounts[i]);
                Debug.Log(i + " : " + popingJewelTypeCounts[i] + " : " + 10 * (int)Mathf.Pow(2, popingJewelTypeCounts[i]));

            }
        }
        Debug.Log("\n");
        for (int i = 0; i < popingTrapTypeCounts.Length; i++)
        {
            if (popingTrapTypeCounts[i] > 0)
            {
                tempInd -= 30/*임시 값*/;
                Debug.Log(i + " : " + popingTrapTypeCounts[i] + " : " + -30/*임시 값*/);
            }
        }
        gameManager.ScoreChangeCall(tempInd);

    }

    private bool inputTrigger = false;

    private void FixedUpdate()
    {
        if (inputTrigger)
        {
            if (!isDeactivating) // 이번 클릭으로 보석을 비활성화 하지 않음
            {
                if (TouchOrClick()) return;
            }
            else // 이번 클릭으로 이미 보석을 비활성화 한 경우 마우스 클릭을 떼기 전까지 입력을 받지 않음
            {
                return;
            }
        }
    }

    private bool TouchOrClick()
    {
        clickedColl = ShotRayAndDetectCollier();

        if (clickedColl != null) // 게임판을 클릭
        {
            updateJewel = true;
            JewelRoom clickedRoom = clickedColl.GetComponent<JewelRoom>();

            if (nowHeldJewel == clickedRoom) // 클릭이 이전 보석을 벗어나지 않았다면 아무 것도 하지 않음
                return true;

            nowHeldJewel = clickedRoom;


            if (clickedRoom.state == 0)
            {

                if (chainedRooms.Count > 1 && selectableRoomsSets[^2].Contains(clickedRoom))
                {
                    Rollback(chainedRooms[^2]);
                    UpdateSelectable();
                    nowHeldJewel = clickedRoom;
                }

                isActivating = true;
                ActivateJewel(clickedRoom);
            }
            else if (clickedRoom.state == 1)
            {
                Rollback(clickedRoom);// ToDo: 현재 마우스가 클릭 중인 보석 뒤 연결된, 이번 클릭에서 활성화된 보석을 취소
            }
            else
            {
                if (isActivating) // 이번 클릭에서 보석을 활성화 한 적이 있음
                {
                    Rollback(clickedRoom); // ToDo:이번 클릭에서 활성화된 보석을 모두 취소
                }
                else
                {
                    isDeactivating = true;
                    DeactivateJewel(clickedRoom);
                }
            }

            UpdateSelectable();
        }
        else // 게임판 바깥을 클릭
        {
            return true;
        }

        return false;
    }

    Collider2D ShotRayAndDetectCollier()
    {
        mouseClickedPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseClickedPosition.z = 0;
        return Physics2D.OverlapPoint(mouseClickedPosition, enabledRoomLayerMask);
    }

    void ActivateJewel(JewelRoom targetRoom)
    {
        targetRoom.state = 1;
        targetRoom.jewel.anim.SetBool(isSelected, true);

        chainDir = targetRoom.nextChainDir;

        chainDirHistory.Add(chainDir);
        chainedRooms.Add(targetRoom);
    }

    void ConfirmActivationJewel()
    {
        foreach (JewelRoom jewelRoom in chainedRooms)
        {
            jewelRoom.state = 2;
        }
    }

    void EndClick()
    {
        updateJewel = false;
        nowHeldJewel = null;
        isActivating = false;
        isDeactivating = false;
    }

    void DeactivateJewel(JewelRoom targetRoom, bool resetChainedRoom = true)
    {

        tempInd = chainedRooms.IndexOf(targetRoom);

        for (int i = tempInd; i < chainedRooms.Count; i++)
        {
            CancelActivationofJewel(chainedRooms[i]);
        }
        chainedRooms = chainedRooms.GetRange(0, tempInd);
        chainDirHistory = chainDirHistory.GetRange(0, tempInd);
        if (tempInd == 0)
        {
            nowHeldJewel = null;
            chainDir = 5;
            selectableRoomsSets = new List<List<JewelRoom>>();
        }
        else
        {
            nowHeldJewel = chainedRooms[^1];
            selectableRoomsSets = selectableRoomsSets.GetRange(0, tempInd - 1);
            chainDir = chainDirHistory[tempInd - 1];
        }
    }


    // 드래그 중 이전 보석으로 마우스 위치를 바꾼 경우 or 마지막으로 선택한 보석을 다른 보석으로 바꾸기 위해 돌아가는 경우
    void Rollback(JewelRoom targetRoom)
    {

        tempInd = chainedRooms.IndexOf(targetRoom);
        chainDir = chainDirHistory[tempInd];

        for (int i = tempInd + 1; i < chainedRooms.Count; i++)
        {
            CancelActivationofJewel(chainedRooms[i]);
        }

        nowHeldJewel = chainedRooms[tempInd];
        nowHeldJewel.state = 1;
        selectableRoomsSets = selectableRoomsSets.GetRange(0, tempInd);


        chainedRooms = chainedRooms.GetRange(0, tempInd + 1);
        chainDirHistory = chainDirHistory.GetRange(0, tempInd + 1);

    }

    void CancelActivationofJewel(JewelRoom jewelRoom) // 이번 클릭에 활성화된 보석을 되돌리는 경우
    {
        jewelRoom.state = 0;
        jewelRoom.jewel.anim.SetBool(isSelected, false);
    }

    public void UpdateSelectable() // 선택 후 각 Room의 상태, 선택 가능 여부, 선택 시 반응 갱신
    {
        tempInd = chainDirHistory.Count;
        // 게임 시작 시 or 모든 보석 선택을 취소해서 초기 상태로 되돌아 갔을 경우
        // (이전 보석의 이전 보석의 선택 여부가 없음)
        if (tempInd == 0)
        {
            FisrtTimeUpdate();
        }
        else
        {
            tempJewelList = new List<JewelRoom>();

            for (int i = 0; i < jewelRooms.Count; i++)
            {
                JewelRoom targetRoom = jewelRooms[i];

                if (chainedRooms.Contains(targetRoom))
                {
                    targetRoom.nextChainDir = chainDirHistory[chainedRooms.IndexOf(targetRoom)];
                }
                else if (tempInd > 1 && selectableRoomsSets[^1].Contains(targetRoom) && targetRoom != nowHeldJewel)
                {
                    MakeSelectable(targetRoom, true, true);
                }
                else
                {
                    xDiff = targetRoom.cord.x - nowHeldJewel.cord.x;
                    yDiff = targetRoom.cord.y - nowHeldJewel.cord.y;
                    switch (chainDir)
                    {
                        case 6:
                            {
                                if (xDiff != 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else if (Mathf.Abs(yDiff) > 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, true);
                                    targetRoom.nextChainDir = 6;
                                }
                                break;
                            }
                        case 4:
                            {
                                if (xDiff != -1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else if (Mathf.Abs(yDiff) > 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, true);
                                    targetRoom.nextChainDir = 4;
                                }
                                break;
                            }
                        case 2:
                            {
                                if (yDiff != 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else if (Mathf.Abs(xDiff) > 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, true);
                                    targetRoom.nextChainDir = 2;
                                }
                                break;
                            }
                        case 8:
                            {
                                if (yDiff != -1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else if (Mathf.Abs(xDiff) > 1)
                                {
                                    MakeSelectable(targetRoom, false);
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, true);
                                    targetRoom.nextChainDir = 8;
                                }
                                break;
                            }
                        case 3:
                            {
                                if (xDiff == 1)
                                {
                                    if (yDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 3;
                                    }
                                    else if (yDiff == 0 || yDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 6;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else if (yDiff == 1)
                                {
                                    if (xDiff == 0 || xDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 2;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, false);
                                }

                                break;
                            }
                        case 1:
                            {
                                if (xDiff == -1)
                                {
                                    if (yDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 1;
                                    }
                                    else if (yDiff == 0 || yDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 4;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else if (yDiff == 1)
                                {
                                    if (xDiff == 0 || xDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 2;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, false);
                                }

                                break;
                            }
                        case 9:
                            {
                                if (xDiff == 1)
                                {
                                    if (yDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 9;
                                    }
                                    else if (yDiff == 0 || yDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 6;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else if (yDiff == -1)
                                {
                                    if (xDiff == 0 || xDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 8;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, false);
                                }

                                break;
                            }
                        case 7:
                            {
                                if (xDiff == -1)
                                {
                                    if (yDiff == -1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 7;
                                    }
                                    else if (yDiff == 0 || yDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 4;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else if (yDiff == -1)
                                {
                                    if (xDiff == 0 || xDiff == 1)
                                    {
                                        MakeSelectable(targetRoom, true);
                                        targetRoom.nextChainDir = 8;
                                    }
                                    else
                                    {
                                        MakeSelectable(targetRoom, false);
                                    }
                                }
                                else
                                {
                                    MakeSelectable(targetRoom, false);
                                }

                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            // 각 단계에서 선택 가능했던 보석 모음
            // 마지막 보석 선택을 단순 드래그로 바꾸는 데 사용
            selectableRoomsSets.Add(tempJewelList);
        }
    }

    private void FisrtTimeUpdate()
    {
        for (int i = 0; i < jewelRooms.Count; i++)
        {
            JewelRoom targetRoom = jewelRooms[i];
            if (targetRoom.jewel.cord.x == 0)
            {

                MakeSelectable(targetRoom, true);

                if (targetRoom.jewel.cord.y == 0)
                {
                    targetRoom.nextChainDir = 3;
                }
                else if (targetRoom.jewel.cord.y == 6)
                {
                    targetRoom.nextChainDir = 9;
                }
                else
                {
                    targetRoom.nextChainDir = 6;
                }
            }
            else if (targetRoom.jewel.cord.x == puzzlSize - 1)
            {
                MakeSelectable(targetRoom, true);

                if (targetRoom.jewel.cord.y == 0)
                {
                    targetRoom.nextChainDir = 1;
                }
                else if (targetRoom.jewel.cord.y == 6)
                {
                    targetRoom.nextChainDir = 7;
                }
                else
                {
                    targetRoom.nextChainDir = 4;
                }
            }
            else if (targetRoom.jewel.cord.y == 0)
            {
                MakeSelectable(targetRoom, true);
                targetRoom.nextChainDir = 2;
            }
            else if (targetRoom.jewel.cord.y == puzzlSize - 1)
            {
                MakeSelectable(targetRoom, true);
                targetRoom.nextChainDir = 8;
            }
            else
            {
                MakeSelectable(targetRoom, false);
                targetRoom.nextChainDir = 5;
            }
        }
    }

    private void MakeSelectable(JewelRoom targetRoom, bool toSelectable = true, bool beforeRoom = false)
    {

        if (toSelectable)
        {
            if (beforeRoom)
            {
                targetRoom.spriteRenderer.color = selectableSignColors[2];
            }
            else
            {
                targetRoom.spriteRenderer.color = selectableSignColors[0];
                tempJewelList.Add(targetRoom);
            }
            targetRoom.gameObject.layer = enabledRoomLayer;
        }
        else
        {
            targetRoom.spriteRenderer.color = selectableSignColors[1];
            targetRoom.gameObject.layer = disabledRoomLayer;
        }
    }
}
