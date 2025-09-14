using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject settingPanel;

    [SerializeField] private Setter[] setters;
    [SerializeField] private LifeSetter lifeSetter;

    [SerializeField] private Button[] buttons;

    public bool isReady { get; private set; }

    public void Initialize()
    {
        gameManager = GameManager.Instance;
        settingPanel.SetActive(false);
        lifeSetter.Initialize();
        isReady = true;
    }

    public void OnGameStartButtonClick()
    {
        foreach (Setter setter in setters)
        {
            setter.SendValueToGameManager();
        }

        gameManager.StartGameCall();
    }

    public void OnSettingButtonClick()
    {
        settingPanel.SetActive(true);
    }

    public void OnQuitButtonClick()
    {
        gameManager.QuitGameCall();
    }

}
