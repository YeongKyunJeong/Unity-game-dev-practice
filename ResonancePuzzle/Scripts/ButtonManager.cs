using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private GameManager gameManager;

    public Button menuButton;
    public GameObject MenuUIParent;

    public GameObject upgradeButtonParent;
    public Button[] upgradeButtons;

    public Button continueButton;
    public Button restartButton;



    public void Initializie()
    {
        gameManager = GameManager.Instance;
        Debug.Log("ScreenButtonManager");
        MenuUIParent.SetActive(false);
        upgradeButtonParent.SetActive(false);
    }

    public void RestartBtnClick()
    {
        gameManager.Restart();
    }

    public void MenuBtnClick()
    {
        Time.timeScale = 0;
        MenuUIParent.gameObject.SetActive(true);
    }

    public void ContinueBtnClick()
    {
        Time.timeScale = 1;
        MenuUIParent.gameObject.SetActive(false);
    }

}
