using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PausePopUp : InGameButtonPopUp
{
    public override void SetActive(bool isOn)
    {
        go.SetActive(isOn);
        if (isInitializing)
        {
            isInitializing = false;
            buttons[0].interactable = true;
            buttons[1].interactable = true;
            buttons[2].interactable = !isTemporaryGameManager;
            if (isTemporaryGameManager)
            {
                TextMeshProUGUI disabledBtnTMP = buttons[2].GetComponentInChildren<TextMeshProUGUI>();
                Color disabledBtnTMPColor = disabledBtnTMP.color;
                disabledBtnTMPColor.a = 0.4f;
                disabledBtnTMP.color = disabledBtnTMPColor;
            }
            buttons[3].interactable = true;
        }
    }

    public void OnResetButtonClick(bool isFullReset)
    {
            gameManager.ResetCall(isFullReset);
    }

    public void OnResumeButtonClick()
    {
            gameManager.ResumeCall();
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
