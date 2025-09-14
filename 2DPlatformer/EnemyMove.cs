using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : Enemy
{
    #region InternalRef
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D coll;
    Coroutine thinkCoroutine;
    #endregion
    #region ExternalRef
    #endregion
    #region Parameters
    public int nextMove;
    public float moveSpeed;
    public float thinkMaxTime;
    float immuneTime;
    int platformLayerMask;
    bool isMoving;
    float resultmoveSpeed;
    #endregion
    #region Strings
    string isWalking = "isWalking";
    #endregion

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        isMoving = false;

        coll = GetComponent<CapsuleCollider2D>();
        //Invoke("Think", 5);
        platformLayerMask = LayerMask.GetMask("Platform");
        if (anim == null)
            anim = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (thinkMaxTime < 2f)
        {
            thinkMaxTime = 5f;
        }
        if (immuneTime < 1)
        {
            immuneTime = 3.0f;
        }

        thinkCoroutine = StartCoroutine(ThinkCoroutine());
    }

    private void FixedUpdate()
    {
        // Move
        //rigid.velocity = new Vector2(resultmoveSpeed, rigid.velocity.y);
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        if (resultmoveSpeed != 0)
        {
            //Debug.Log(" ");
        }

        // Platform Check
        Debug.DrawRay(rigid.position + 0.3f * nextMove * Vector2.right, 0.6f * Vector3.down, Color.red);
        if (isMoving)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + 0.3f * nextMove * Vector2.right, Vector3.down, 0.6f, platformLayerMask);
            if (rayHit.collider == null)
            {
                StopCoroutine(thinkCoroutine);
                thinkCoroutine = StartCoroutine(ThinkCoroutine(-nextMove));
            }

        }


    }

    public void OnDamaged()
    {
        //anim.SetBool("isImmuned", true);
        StartCoroutine(DamagedCoroutine());
    }


    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DamagedCoroutine()
    {
        //anim.SetBool("isImmuned", true);
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);
        spriteRenderer.flipY = true;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse);
        coll.enabled = false;
        yield return new WaitForSeconds(immuneTime);
        Deactivate();
        //anim.SetBool("isImmuned", false);
        yield return null;
    }

    IEnumerator ThinkCoroutine(int firstMoveDir = 0)
    {
        float thinkTime = 0;
        if (firstMoveDir != 0)
        {
            nextMove = firstMoveDir;
            isMoving = true;
            resultmoveSpeed = nextMove * moveSpeed;
            if (nextMove == 1)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
            thinkTime = Random.Range(2f, thinkMaxTime + 1);
            yield return new WaitForSeconds(thinkTime);
        }

        while (true)
        {
            nextMove = Random.Range(-1, 2);

            if (nextMove == 0)
            {
                isMoving = false;
                anim.SetBool(isWalking, false);
            }
            else
            {
                if (nextMove == 1)
                    spriteRenderer.flipX = true;
                else
                    spriteRenderer.flipX = false;

                isMoving = true;
                anim.SetBool(isWalking, true);
            }

            resultmoveSpeed = nextMove * moveSpeed;
            thinkTime = Random.Range(2f, thinkMaxTime + 1);
            yield return new WaitForSeconds(thinkTime);

        }
    }
}
