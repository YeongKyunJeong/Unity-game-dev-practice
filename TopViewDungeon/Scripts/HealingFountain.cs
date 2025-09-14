using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : Collidable
{
    public int healingAmount = 1;

    private float healCooldown = 1.0f;
    private float lastHeal;

    [SerializeField]
    private bool isCoroutine = true;
    private bool isCooldown = false;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name != "Player")
            return;

        if (!isCoroutine)
        {
            if (Time.time - lastHeal > healCooldown)
            {
                lastHeal = Time.time;
                GameManager.instance.player.Heal(healingAmount);
            }
        }
        else if (!isCooldown)
        {
            StartCoroutine(HealCoroutine());
        }
    }

    IEnumerator HealCoroutine()
    {
        isCooldown = true;
        GameManager.instance.player.Heal(healingAmount);
        yield return new WaitForSeconds(healCooldown);
        isCooldown = false;
        yield return null;

    }
}
