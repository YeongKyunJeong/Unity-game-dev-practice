using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region InternalRef
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    AudioSource audioSource;
    #endregion

    #region ExternalRef
    GameManager gameManager = GameManager.Instance;
    #endregion

    #region Parameters
    public float maxSpeed;
    public float jumpPower;
    float immuneTime;
    RaycastHit2D rayHit;
    public Vector2 landingDetectionBoxSize = new Vector2(0.25f, 1f)/*new Vector2(1f, 1f)*/;
    public float dist = 0;
    #endregion

    #region Strings
    // Animator
    string isWalking = "isWalking";
    string isJumping = "isJumping";
    string isUp = "isUp";
    string isDamaged = "isDamaged";
    string isImmuned = "isImmuned";
    // Button
    string jumpBtn = "Jump";
    string horizontalBtn = "Horizontal";
    ////Tag
    //string enemy = "Enemy";
    //string item = "Item";
    //string finish = "Finish";
    #endregion

    #region Layers
    int platformLayerMask;
    int playerDamagedLayerNum;
    int playerBaseLayerNum;
    #endregion

    #region Coroutine
    Coroutine damageCoroutine;
    #endregion

    #region AudioClip
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (maxSpeed < 1)
            maxSpeed = 4.5f;
        if (jumpPower < 1)
            jumpPower = 20;
        platformLayerMask = LayerMask.GetMask("Platform");
        if (immuneTime < 1)
        {
            immuneTime = 3.0f;
        }
        playerDamagedLayerNum = LayerMask.NameToLayer("PlayerDamaged");
        playerBaseLayerNum = gameObject.layer;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update() // 단발적인 키 입력
    {
        // Jump
        if (Input.GetButtonDown(jumpBtn) && !anim.GetBool(isJumping))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool(isJumping, true);
            anim.SetBool(isUp, true);
            PlaySound(audioJump);
        }

        // Stop Speed
        if (Input.GetButtonUp(horizontalBtn))    //*** 정지 조작감 수정 필요, a와 d가 동시에 눌렸을 때 상황 고려 필요
        {
            //rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            rigid.velocity = new Vector2(0.1f * rigid.velocity.x, rigid.velocity.y);
        }

        // Direction Sprtie
        if (Input.GetAxisRaw(horizontalBtn) > 0.1f)
        {
            anim.SetBool(isWalking, true);    // 입력 기준 애니메이션 변경
            spriteRenderer.flipX = false;
        }
        else if (Input.GetAxisRaw(horizontalBtn) < -0.1f)
        {
            anim.SetBool(isWalking, true);
            spriteRenderer.flipX = true;
        }
        else
        {
            anim.SetBool(isWalking, false);
        }

    }

    private void FixedUpdate() // 지속적인 키 입력
    {
        // Move Speed
        float h = Input.GetAxisRaw(horizontalBtn);
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed) // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed) // Right Max Speed
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        // Landing Platform
        Debug.DrawRay(rigid.position + 0.125f * Vector2.right, 0.75f * Vector3.down, Color.green);
        Debug.DrawRay(rigid.position - 0.125f * Vector2.right, 0.75f * Vector3.down, Color.green);


        if (rigid.velocity.y <= 0)
        {

            //rayHit = Physics2D.BoxCast(rigid.position, landingDetectionBoxSize, 0, Vector2.down, 0.7f, platformLayerMask);
            rayHit = Physics2D.BoxCast(rigid.position + dist * Vector2.down, landingDetectionBoxSize, 0, Vector2.down, 0, platformLayerMask);
            //Collider2D[] temp = Physics2D.OverlapBoxAll(rigid.position + dist * Vector2.down, landingDetectionBoxSize,0, platformLayerMask);
            //hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
            //rayHit = Physics2D.BoxCast(rigid.position)
            //RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.7f, platformLayerMask);
            if (rayHit.collider != null)
            //if (temp.Length != 0)
            {
                anim.SetBool(isJumping, false);
            }
            else if (!anim.GetBool(isJumping))
            {
                anim.SetBool(isJumping, true);
                anim.SetBool(isUp, false);
            }
            else if (anim.GetBool(isUp))
            {
                anim.SetBool(isJumping, true);
                anim.SetBool(isUp, false);
            }
        }
        else
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.transform.GetComponent<Enemy>();
        if (enemy is EnemyMove enemyMove)
        {
            if (rigid.velocity.y < 0 && (transform.position.y > collision.transform.position.y))
            {
                OnAttack(collision.transform);
                gameManager.stagePoint += 100;
                gameManager.UpdatePoints();
            }
            else
                damageCoroutine = StartCoroutine(DamagedCoroutine(collision.transform.position));
        }
        else if (enemy != null)
            damageCoroutine = StartCoroutine(DamagedCoroutine(collision.transform.position));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item is Coin coin)
        {
            PlaySound(audioItem);
            gameManager.stagePoint += coin.point;
            gameManager.UpdatePoints();
            coin.gameObject.SetActive(false);
        }
        else if (item is Finish finish )
        {
            PlaySound(audioItem);
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point

        // Reaction Force;
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        PlaySound(audioAttack);
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    public void OnFalling()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(FallingDamagedCoroutine());
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    IEnumerator DamagedCoroutine(Vector2 enemyPos)
    {
        gameManager.Health--;
        if (gameManager.Health == 0)
        {
            OnDie();
            yield break;
        }

        gameObject.layer = playerDamagedLayerNum;
        int dirc = transform.position.x - enemyPos.x > 0 ? 1 : -1;
        rigid.velocity = new Vector2(rigid.velocity.x, 5);
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        anim.SetBool(isImmuned, true);
        anim.SetTrigger(isDamaged);
        PlaySound(audioDamaged);

        yield return new WaitForSeconds(immuneTime);
        gameObject.layer = playerBaseLayerNum;
        anim.SetBool(isImmuned, false);
        spriteRenderer.color = Color.white;

        yield return null;
    }

    IEnumerator FallingDamagedCoroutine()
    {
        gameManager.Health--;
        if (gameManager.Health == 0)
        {
            OnDie();
            yield break;
        }

        gameObject.layer = playerDamagedLayerNum;
        rigid.velocity = Vector3.zero;
        anim.SetBool(isImmuned, true);
        anim.SetTrigger(isDamaged);
        PlaySound(audioDamaged);

        yield return new WaitForSeconds(immuneTime);
        gameObject.layer = playerBaseLayerNum;
        anim.SetBool(isImmuned, false);
        spriteRenderer.color = Color.white;

        yield return null;
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        anim.SetTrigger("isDead");
        PlaySound(audioDie);
        gameObject.layer = playerDamagedLayerNum;
    }
}
