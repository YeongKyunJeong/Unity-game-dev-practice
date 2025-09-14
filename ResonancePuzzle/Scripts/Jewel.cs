using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    private JewelData jewelData;
    public Vector2Int cord /*{ get; set; }*/;
    public Animator anim /*{ get; set; }*/;
    public SpriteRenderer spriteRenderer /*{ get; set; }*/;
    private Coroutine animationCoroutine = null;
    private string pop = "pop";
    public WaitForSeconds popAnimationTime;

    public void Initialize(Vector2Int cord, JewelData jewelData)
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        this.jewelData = jewelData;
        this.cord = cord;
        popAnimationTime = jewelData.popAnimationWaitforSecond;
    }

    public void ChangeJewelSprite(bool isPop = true, int targetID = -1)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(SpriteChangeCoroutine(isPop, targetID));
    }

    IEnumerator SpriteChangeCoroutine(bool isPop, int targetID)
    {
        if (isPop) // 보석이 터지는 이펙트
        {
            anim.SetTrigger(pop);
            //Debug.Log(this.name + " poped!");
            // 임시 pop animation
            transform.localScale = 1.4f * Vector2.one;


            yield return popAnimationTime;

            // 임시 pop animation
            transform.localScale = 1f * Vector2.one;
            //Debug.Log(this.name + "'s pop animation Ends");
        }
        if (targetID > 19)
        {
            anim.runtimeAnimatorController = jewelData.itemAOCs[targetID - 20];
            spriteRenderer.sprite = jewelData.itemSprites[targetID - 20];
        }
        else if (targetID > 9)
        {
            anim.runtimeAnimatorController = jewelData.trapAOCs[targetID - 10];
            spriteRenderer.sprite = jewelData.trapSprites[targetID - 10];
        }
        else
        {
            anim.runtimeAnimatorController = jewelData.jewelAOCs[targetID];
            spriteRenderer.sprite = jewelData.jewelSprites[targetID];
        }

        //Debug.Log(this.name + "'s Type changes to " + targetID.ToString());


        yield return null;
    }



}
