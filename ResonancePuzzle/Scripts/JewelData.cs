using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JewelData", menuName = "Game/JewelData", order = 1)]
public class JewelData : ScriptableObject
{
    public Sprite[] jewelSprites;
    public AnimatorOverrideController[] jewelAOCs;

    public Sprite[] trapSprites;
    public AnimatorOverrideController[] trapAOCs;

    public Sprite[] itemSprites;
    public AnimatorOverrideController[] itemAOCs;

    //public AnimationClipSet[] jewelAnimationClipSet;

    public float popAnimationTime = 1f;

    public WaitForSeconds popAnimationWaitforSecond { get; private set; }

    public void Initialize()
    {
        popAnimationWaitforSecond = new WaitForSeconds(popAnimationTime);
    }
}

//[System.Serializable]
//public class AnimationClipSet
//{
//    public AnimationClip[] animationClips;
//}
