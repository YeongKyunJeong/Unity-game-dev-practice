using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    PaddleHit,
    BrickHit,
    BrickBreak,
    Fall,
}

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private AudioSource audioSource_BGM;
    [SerializeField]
    private AudioSource audioSource_SFX;
    private BrickData brickData;

    private AudioClip[] sFxaudioClips;
    private int clipIndex = -1;
    private const int NUMBER_PADDLE_HIT = 0;
    private const int NUMBER_BRICK_HIT = 1;
    private const int NUMBER_BRICK_BREAK = 2;
    private const int NUMBER_FALL = 3;

    public void Initialize(BrickData brickData)
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        if (audioSource_BGM == null)
        {
            Debug.LogError("BGM audio source not detected");
        }
        if (audioSource_SFX == null)
        {
            Debug.LogError("SFX audio source not detected");
        }
        this.brickData = brickData;
        SetAudioClips();

        clipIndex = -1;
        audioSource_BGM.Play();
    }

    private void SetAudioClips()
    {
        sFxaudioClips = brickData.sFXaudioClips;
    }

    public void PlaySFX(SFXType inputSFXType)
    {
        switch (inputSFXType)
        {
            case SFXType.PaddleHit:
                {
                    if(clipIndex != NUMBER_PADDLE_HIT)
                    {
                        audioSource_SFX.clip = sFxaudioClips[NUMBER_PADDLE_HIT];
                        clipIndex = NUMBER_PADDLE_HIT;
                    }
                    break;
                }
            case SFXType.BrickHit:
                {
                    if (clipIndex != NUMBER_BRICK_HIT)
                    {
                        audioSource_SFX.clip = sFxaudioClips[NUMBER_BRICK_HIT];
                        clipIndex = NUMBER_BRICK_HIT;
                    }
                    break;
                }
            case SFXType.BrickBreak:
                {
                    if (clipIndex != NUMBER_BRICK_BREAK)
                    {
                        audioSource_SFX.clip = sFxaudioClips[NUMBER_BRICK_BREAK];
                        clipIndex = NUMBER_BRICK_BREAK;
                    }
                    break;
                }
            case SFXType.Fall:
                {
                    if (clipIndex != NUMBER_FALL)
                    {
                        audioSource_SFX.clip = sFxaudioClips[NUMBER_FALL];
                        clipIndex = NUMBER_FALL;
                    }
                    break;
                }
            default:
                break;
        }
        audioSource_SFX.Play();
    }
}
