using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AspectRatioEnforcer : MonoBehaviour
{
    public float targetAspect = 16f / 9f;
    private RectTransform canvasRect;
    private Vector2 lastCanvasSize;
    private int screenWidth;
    private int screenHeight;
    private float newValue;
    private FullScreenMode windowed = FullScreenMode.Windowed;
    private Camera mainCamera;
    private CanvasScaler canvasScaler;

    private float fixedWindowAspect;
    private float changedWindiowAspect;
    private Vector2 canvasSizeDelta;

    public void Initialize(int screenWidth = 1920, int screenHeight = 1080)
    {
        if (canvasRect == null)
        {
            canvasRect = GetComponent<RectTransform>();
        }
        this.canvasScaler = canvasRect.transform.GetComponent<CanvasScaler>();
        this.screenHeight = screenHeight;
        this.screenWidth = screenWidth;
        fixedWindowAspect = 16f / 9f;
        lastCanvasSize = canvasRect.sizeDelta;
                
        ChangeSceneWithoutCamera();
    }

    public void ChangeSceneWithoutCamera()
    {
        mainCamera = Camera.main;
        Screen.SetResolution(screenWidth, screenHeight, windowed);
        lastCanvasSize = Vector2.zero;
    }

    private void LateUpdate()
    {
        canvasSizeDelta = canvasRect.sizeDelta;
        if (canvasSizeDelta != lastCanvasSize)
        {
            OnCanvasSizeChanged();
            lastCanvasSize = canvasSizeDelta;
        }
    }

    public void OnCanvasSizeChanged()
    {
        if (mainCamera == null)
        {
            ChangeSceneWithoutCamera();
        }

        changedWindiowAspect = (float)Screen.width / (float)Screen.height;
        if (changedWindiowAspect > fixedWindowAspect)
        {
            newValue = fixedWindowAspect / changedWindiowAspect;
            canvasScaler.matchWidthOrHeight = 1;
            mainCamera.rect = new Rect((1 - newValue) / 2f, 0, newValue, 1);
        }
        else
        {
            newValue = changedWindiowAspect / fixedWindowAspect;
            canvasScaler.matchWidthOrHeight = 0;

            mainCamera.rect = new Rect(0, (1 - newValue) / 2f, 1, newValue);

        }
    }

}
