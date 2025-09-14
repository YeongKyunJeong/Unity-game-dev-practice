using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopUp : InGameButtonPopUp
{
    public override void SetActive(bool isOn)
    {
        go.SetActive(isOn);
        if (isInitializing)
        {
            isInitializing = false;
            buttons[0].interactable = true;
            buttons[1].interactable = !isTemporaryGameManager;
            if (isTemporaryGameManager)
            {
                TextMeshProUGUI disabledBtnTMP = buttons[1].GetComponentInChildren<TextMeshProUGUI>();
                Color disabledBtnTMPColor = disabledBtnTMP.color;
                disabledBtnTMPColor.a = 0.4f;
                disabledBtnTMP.color = disabledBtnTMPColor;
            }
            buttons[2].interactable = true;
        }
    }
    public void OnResetButtonClick(bool isFullReset)
    {
        gameManager.ResetCall(isFullReset);
    }

    public void OnBackToTitleButtonClick()
    {
        if (isTemporaryGameManager)
        {
            gameManager.QuitGameCall();
        }
        else
        {
            gameManager.BackToTitleSceneCall();
        }
    }
}
