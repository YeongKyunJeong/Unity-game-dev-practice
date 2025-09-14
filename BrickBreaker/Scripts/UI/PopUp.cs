using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopUp : MonoBehaviour
{
    protected GameObject go;
    protected bool isTemporaryGameManager;
    protected GameManager gameManager;
    protected bool isInitializing = true;
    public Button[] buttons;

    public abstract void Initialize(SceneType initializedScene);

}
