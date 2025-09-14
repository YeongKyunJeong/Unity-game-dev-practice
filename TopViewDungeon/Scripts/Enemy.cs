using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Experience
    public int xpValue = 1;

    // Logic
    public float triggerLength = 1;  // ���� ���� �ݰ� 1m
    public float chaseLength = 5;  // ���� �Ÿ� 5m

    [SerializeField]
    private bool chasing;
    [SerializeField]
    private bool collidingWithPlayer;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Vector3 startingPosition;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            //chasing = Vector3.Distance(playerTransform.position, startingPosition) < triggerLength;
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
                chasing = true;

            if (chasing) // ����
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else // �ǵ��� ��
            {
                UpdateMotor( (startingPosition - transform.position));
            }
        }
        else
        {
            UpdateMotor( (startingPosition - transform.position));
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits); // boxCollider�� �����ִ� Collider2D���� ã�� hits�� ��ȯ
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (/*hits[i].tag == "Fighter" &&*/ hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            //The array is not cleaned up, so we do it ourself;
            hits[i] = null;
        }
    }

    protected override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }
}
