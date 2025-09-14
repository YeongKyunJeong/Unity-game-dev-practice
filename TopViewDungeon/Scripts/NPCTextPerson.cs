using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTextPerson : Collidable
{
    public string message;
    private float cooldown = 4.0f;
    private float lastShout;
    private bool isCoolDown = false;

    protected override void OnCollide(Collider2D coll)
    {
        if (!isCoolDown)
            StartCoroutine(SayWords());
        //lastShout = Time.time;
        //GameManager.instance.ShowText(message, 25, Color.white, transform.position, Vector3.zero, cooldown);
    }

    IEnumerator SayWords()
    {
        isCoolDown = true;
        GameManager.instance.ShowText(message, 25, Color.white, transform.position + 0.2f*Vector3.up, Vector3.zero, cooldown);
        yield return new WaitForSeconds(cooldown);
        isCoolDown = false;
        yield return null;
    }
}
