using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBrickData", menuName = "ScriptableObjects/BrickData", order = 1)]
public class BrickData : ScriptableObject
{
    [SerializeField]
    private Sprite[] brickSpritesField;
    [SerializeField]
    private AudioClip audioClip_PaddleHit;
    [SerializeField]
    private AudioClip audioClip_BrickHit;
    [SerializeField]
    private AudioClip audioClip_BrickBreak;
    [SerializeField]
    private AudioClip audioClip_Fall;

    [SerializeField]
    private int[] defaultItemProbabilityField;

    [SerializeField]
    private Color[] itemColorsField; // Temporary instead of sprite

    [SerializeField]
    private Color[] ballColorsField; // Temporary instead of sprite

    [HideInInspector]
    public int totalItemTypeCount;
    [HideInInspector]
    public int ballSpriteNumber;
    public Sprite[] brickSprites { get; private set; }
    public int[] defaultItemProbability { get; private set; }
    public Color[] itemColors { get; private set; }
    public Color[] ballColors { get; private set; }

    public AudioClip[] sFXaudioClips;

    public void Initialize()
    {
        brickSprites = new Sprite[brickSpritesField.Length];
        brickSpritesField.CopyTo(brickSprites, 0);

        defaultItemProbability = new int[defaultItemProbabilityField.Length];
        defaultItemProbabilityField.CopyTo(defaultItemProbability, 0);

        totalItemTypeCount = itemColorsField.Length;
        itemColors = new Color[totalItemTypeCount];
        itemColorsField.CopyTo(itemColors, 0);

        ballSpriteNumber = ballColorsField.Length;
        ballColors = new Color[ballSpriteNumber];
        ballColorsField.CopyTo(ballColors, 0);

        sFXaudioClips = new AudioClip[Enum.GetValues(typeof(SFXType)).Length];
        sFXaudioClips[0] = audioClip_PaddleHit;
        sFXaudioClips[1] = audioClip_BrickHit;
        sFXaudioClips[2] = audioClip_BrickBreak;
        sFXaudioClips[3] = audioClip_Fall;
    }


}
