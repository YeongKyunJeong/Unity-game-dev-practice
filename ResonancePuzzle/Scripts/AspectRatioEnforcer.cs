using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioEnforcer : MonoBehaviour
{
    public float targetAspect = 9f / 16f;
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

    public void Initialize(RectTransform canvasRect, int screenWidth = 1080, int screenHeight = 1920)
    {
        this.canvasRect = canvasRect;
        this.canvasScaler = canvasRect.transform.GetComponent<CanvasScaler>();
        this.screenHeight = screenHeight;
        this.screenWidth = screenWidth;
        mainCamera = Camera.main;
        fixedWindowAspect = 9f / 16f;
        lastCanvasSize = canvasRect.sizeDelta;
        Screen.SetResolution(screenWidth, screenHeight, windowed);
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
            mainCamera.rect = new Rect(0, (1 - newValue) / 2f,1, newValue);
        }
    }

}
