using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameButtonPopUp : PopUp
{
    public override void Initialize(SceneType initializedScene)
    {
        isInitializing = true;
        go = this.gameObject;
        if (initializedScene == SceneType.Title)
            isTemporaryGameManager = false;
        else
            isTemporaryGameManager = true;
           
        gameManager = GameManager.Instance;
        SetActive(false);
    }

    public virtual void SetActive(bool isOn)
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
}
