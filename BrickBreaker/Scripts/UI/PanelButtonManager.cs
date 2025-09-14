using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonManager : MonoBehaviour
{
    public GameOverPopUp gameOverPopUp;
    public PausePopUp pausePopUp;

    public void Initialize(SceneType initializedScene)
    {
        if (gameOverPopUp == null)
        {
            gameOverPopUp = FindFirstObjectByType<GameOverPopUp>();
        }
        if (pausePopUp == null)
        {
            pausePopUp = FindFirstObjectByType<PausePopUp>();
        }

        
            gameOverPopUp.Initialize(initializedScene);
            pausePopUp.Initialize(initializedScene);
        

        ResetPanel();
    }

    public void PauseGame()
    {
        SetActive(true);
        pausePopUp.SetActive(true);
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    public void ResetPanel()
    {
        SetActive(false);
        gameOverPopUp.SetActive(false);
        pausePopUp.SetActive(false);

    }

    public void GameOver()
    {
        SetActive(true);
        gameOverPopUp.SetActive(true);
    }
}
