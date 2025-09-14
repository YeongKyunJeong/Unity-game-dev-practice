using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TitleSceneSetterType
{
    Life
}
public abstract class Setter : MonoBehaviour
{
    [SerializeField] protected Button[] buttons;
    //[SerializeField] Slider slider;
    [SerializeField] protected TextMeshProUGUI valueTMP;
    [SerializeField] protected int value;
    // Start is called before the first frame update
    protected GameManager gameManager;
    protected TitleSceneSetterType setterType;

    public abstract void OnValueChangeButtonClick(bool isUp);

    public void SendValueToGameManager() 
    {
        gameManager.TakeSettingValue(value, setterType);
    }

}
