using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // Damage structure
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7, 8 };
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 2.8f, 3.5f, 3.8f, 4.1f, 4.5f };

    // Upgrade
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    // Swing
    private Animator anim;
    private bool isPlayerAlive = true;
    private float cooldown = 0.5f;
    private float lastSwing;
    // Coroutine 활용
    public bool isCoroutine = false;
    private bool isCooldown = false;


    protected override void Start()
    {
        base.Start();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCoroutine)
            {
                if (Time.time - lastSwing > cooldown)
                {
                    lastSwing = Time.time;
                    Swing();
                }
            }
            else if (!isCooldown)
            {
                StartCoroutine(SwingCoroutine());
            }
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            // Create a new damage object, then we'll send it to fighter we've hit
            Damage dmg = new Damage
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position, //*** 무기가 아니라 플레이어 기준으로 밀려나도록 수정
                pushForce = pushForce[weaponLevel]
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

    private void Swing()
    {
        if (isPlayerAlive)
            anim.SetTrigger("Swing");
    }

    public void PlayerDead()
    {
        isPlayerAlive = false;
    }

    public void PlayerRespawn()
    {
        isPlayerAlive = true;
    }

    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprties[weaponLevel];

    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprties[weaponLevel];
    }

    IEnumerator SwingCoroutine()
    {
        if (isPlayerAlive)
        {
            Swing();
            isCooldown = true;
            yield return new WaitForSeconds(cooldown);
        }
        isCooldown = false;
        yield return null;
    }
}
