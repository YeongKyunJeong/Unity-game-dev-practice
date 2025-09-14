using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSetter : Setter
{
    private const int MAX_LIFE = 99;
    private const int MIN_LIFE = 1;

    public void Initialize()
    {
        gameManager = GameManager.Instance;
        setterType = TitleSceneSetterType.Life;
        value = gameManager.lives;

        valueTMP.text = value.ToString();
    }

    public override void OnValueChangeButtonClick(bool isUp)
    {
        if (isUp)
        {
            value++;
            if(value >= MAX_LIFE)
            {
                buttons[1].interactable = false;
            }
            
            if(value > MIN_LIFE)
            {
                buttons[0].interactable = true;
            }

            valueTMP.text = value.ToString();            
        }
        else
        {
            value--;
            if (value < MAX_LIFE)
            {
                buttons[1].interactable = true;
            }

            if (value <= MIN_LIFE)
            {
                buttons[0].interactable = false;
            }
            

            valueTMP.text = value.ToString();
        }
    }
}
