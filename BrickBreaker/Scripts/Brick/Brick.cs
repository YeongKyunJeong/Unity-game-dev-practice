using UnityEngine;

public class Brick : MonoBehaviour
{
    public int health { get; private set; }
    [SerializeField] int initialHealth = 3;
    private const int MAX_HEALTH = 5;
    private const int MIN_HEALTH = 1;

    public int points = 100;

    public SpriteRenderer spriteRenderer { get; private set; }

    private static GameManager gameManager;

    private static LayerMask BallLayer = 0;
    private static int brickDamage = 0;
    private static Vector3 zeroVector = Vector3.zero;
    private static BrickData brickData;

    public int BrickDamagerSetter { get { return brickDamage; } set { brickDamage = value; } }
    [SerializeField] private Item fixedDropItem = Item.None;
    [SerializeField] private Item resultDropItem;

    [SerializeField] private bool isBreakable = true;
    private bool isBroken = false;
    public bool isBrokenGetter { get => isBroken; }
    private Transform selfTransform;

    private SFXType brickHitType = SFXType.BrickHit;
    private SFXType brickBreakType = SFXType.BrickBreak;

    public void Initialize(LayerMask givenBallLayer, int givenBrickDamage, BrickData givenBrickData, Item givenItem = Item.None)
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        if (brickData == null)
        {
            brickData = givenBrickData;
        }
        if (BallLayer == 0)
        {
            BallLayer = givenBallLayer;
        }
        if (brickDamage == 0)
        {
            brickDamage = givenBrickDamage;
        }


        selfTransform = transform;
        if (givenItem == Item.None)
            resultDropItem = fixedDropItem;
        else
            resultDropItem = givenItem;

        ResetBrick();



        health = initialHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isBreakable)
            spriteRenderer.sprite = givenBrickData.brickSprites[health];
        else
            spriteRenderer.sprite = givenBrickData.brickSprites[0];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreakable && !isBroken)
            if (collision.gameObject.layer == BallLayer)
            {
                Hit();
            }
    }

    private void Hit()
    {
        if (health > 0)
        {
            health -= brickDamage;
        }

        if (isBroken)
        {
            Debug.Log("Repeated broken error");
        }

        if (health > 0)
        {
            PlayBrickSFX(brickHitType);
            this.spriteRenderer.sprite = brickData.brickSprites[health];
            gameManager.HitBrick(brickDamage * points, zeroVector);
        }
        else
        {
            health = 0;
            this.gameObject.SetActive(false);
            PlayBrickSFX(brickBreakType);
            isBroken = true;
            gameManager.HitBrick(brickDamage * points, selfTransform.position, isBroken, resultDropItem);
        }
    }

    public void PlayBrickSFX(SFXType inputSFX)
    {
        gameManager.PlaySFX(inputSFX);
    }

    public void ResetBrick()
    {
        this.gameObject.SetActive(true);
        this.health = initialHealth;

        isBroken = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brickData.brickSprites[health];
    }

    public void SetBrickParameter(int initHealth, bool isBreakable, BrickData givneBrickData)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (!isBreakable)
        {
            spriteRenderer.sprite = givneBrickData.brickSprites[0];
        }
        else
        {
            spriteRenderer.sprite = givneBrickData.brickSprites[initHealth];
        }
        this.initialHealth = initHealth;
    }
}
