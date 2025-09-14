using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Item
{
    None,
    PowerUp,
    LifeUp,
    MultiBall
}
//////////// #### To Do : Add more Item Logics
public class DroppingItem : MonoBehaviour, IPoolMemberObject
{
    private static GameManager gameManager;
    private static BrickData brickData;

    [SerializeField] private Transform selfTransform;
    private int collidedObjectLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Item itemType;
    [SerializeField] private Rigidbody2D rigid;

    private LayerMask paddleLayer;
    private LayerMask deadZoneLayer;
    [SerializeField] private float fallingSpeedField = 1;
    private Vector3 fallingSpeedVector;

    //public void GlobalInitialize(BrickData givenBrickData, LayerMask givenPaddleLayer, LayerMask givenDeadZoneLayer)
    //{
    //    if (GameManager.Instance == null)
    //    {
    //        Debug.LogError("Droping item : GameManager not detected");
    //    }
    //    else
    //    {
    //        gameManager = GameManager.Instance;
    //        brickData = givenBrickData;
    //        paddleLayer = givenPaddleLayer;
    //        deadZoneLayer = givenDeadZoneLayer;
    //        fallingSpeedVector = fallingSpeedField * Time.fixedDeltaTime * Vector3.down;
    //        itemTypeNumber = brickData.itemTypeNumber + 1;
    //        isGlobalReady = true;
    //    }
    //}

    public void Initialize()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (brickData == null)
        {
            brickData = gameManager.BrickDataGetter;
        }

        paddleLayer = gameManager.PaddleLayerGetter;
        deadZoneLayer = gameManager.DeadZoneLayerGetter;
        fallingSpeedVector = fallingSpeedField * Time.fixedDeltaTime * Vector3.down;

        gameManager.ResetDroppingItemAction += DisableByReset;

        RelInitialize();
    }

    public void SetPositionAndItemType(Vector3 startPosition, Item targetItem)
    {
        selfTransform.position = startPosition;
        itemType = targetItem/*(Item)Random.Range(1, itemTypeNumber)*/;
        spriteRenderer.color = brickData.itemColors[(int)itemType];

    }

    private void FixedUpdate()
    {
        selfTransform.localPosition += fallingSpeedVector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidedObjectLayer = collision.gameObject.layer;
        if (collidedObjectLayer == paddleLayer)
        {
            Debug.Log(itemType.ToString());
            gameObject.SetActive(false);
            gameManager.ItemGettodaze(itemType);
        }
        else if (collidedObjectLayer == deadZoneLayer)
        {
            gameObject.SetActive(false);
        }
    }

    public void DisableByReset()
    {
        gameObject.SetActive(false);
    }

    public void RelInitialize()
    {
        gameObject.SetActive(true);

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        if (selfTransform == null)
        {
            selfTransform = transform;
        }
    }

    private void OnDestroy()
    {
        gameManager.ResetDroppingItemAction -= DisableByReset;
    }
}