using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreTMPSetter;
    [SerializeField] private GameButtonManager gsButtonManager;
    [SerializeField] private PanelButtonManager panelButtonManager;
    [SerializeField] private AspectRatioEnforcer aspectRatioEnforcer;

    public void Initialize(SceneType initializedScene)
    {
        if (scoreTMPSetter != null)
        {
            scoreTMPSetter.Initialize();
        }
        else
        {
            Debug.LogError("ScoreManager Not Detected");
        }

        if (gsButtonManager != null)
        {
            gsButtonManager.Initialize(initializedScene); ;
        }
        else
        {
            Debug.LogError("GameButtonManager Not Detected");
        }

        if (panelButtonManager != null)
        {
            panelButtonManager.Initialize(initializedScene); ;
        }
        else
        {
            Debug.LogError("PanelButtonManager Not Detected");
        }

        if (aspectRatioEnforcer != null)
        {
            aspectRatioEnforcer.Initialize(GetComponent<RectTransform>());
        }

        if (initializedScene == SceneType.Title)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

    }

    public void DoUIManagerSetting(int targetLevel, int targetLives, int targetScore)
    {
        ChangeNumber(targetLevel, UINumberCategory.Level);
        ChangeNumber(targetLives, UINumberCategory.Life);
        ChangeNumber(targetScore, UINumberCategory.Score);
        panelButtonManager.ResetPanel();
        SetAspectRatioEnforcer();
    }

    public void SetAspectRatioEnforcer()
    {
        aspectRatioEnforcer.ChangeSceneWithoutCamera();
    }

    public void ChangeNumber(int inputInt, UINumberCategory changedNumber = UINumberCategory.Score)
    {
        scoreTMPSetter.ChangeNumber(inputInt, changedNumber);
    }

    public void ResumeGame()
    {
        panelButtonManager.ResetPanel();
    }

    public void PauseGame()
    {
        panelButtonManager.PauseGame();
    }

    public void GameOver()
    {
        panelButtonManager.GameOver();
    }


}
