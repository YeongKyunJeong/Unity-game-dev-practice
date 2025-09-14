using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
public class Player : Mover
{
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //DontDestroyOnLoad(gameObject);
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;
        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChage();
    }

    protected override void Death()
    {
        isAlive = false;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
        GameManager.instance.weapon.PlayerDead();
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive)
            UpdateMotor(new Vector3(x, y, 0));
    }

    public void SwapSprite(int skinId)
    {
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
    }
    public void OnLevelUp()
    {
        maxHitPoint++;
        hitPoint = maxHitPoint;
    }
    public void SetLevel(int level)
    {
        for (int i = 1; i < level; i++)
            OnLevelUp();
    }
    public void Heal(int healingAmount)
    {
        if (hitPoint >= maxHitPoint)
        {
            hitPoint = maxHitPoint;
            return;
        }

        hitPoint += healingAmount;
        GameManager.instance.ShowText("+" + healingAmount.ToString() + " hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.OnHitpointChage();
    }
    
    public void Respawn()
    {
        //Heal(maxHitPoint);
        hitPoint = maxHitPoint;
        GameManager.instance.OnHitpointChage();
        GameManager.instance.weapon.PlayerRespawn();
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }

}

