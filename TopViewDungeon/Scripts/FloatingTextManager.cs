using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;

    private List<FloatingText> floatingTexts = new List<FloatingText>();

    //private void Start()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}

    private void Update()
    {
       // MonoBehavior�� ���̱� ������ �� FloatingText���� MonoBehavior�� �޾� Update ���� �ʰ� FloatingText
       
        foreach (FloatingText txt in floatingTexts)
            txt.UpdateFloatingText(); 
        
    }

    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        FloatingText floatingText = GetFloatingText();

        floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;
        floatingText.txt.color = color;
        
        floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position); // Transfer world space to screen sapce so we can use it in the UI
        floatingText.motion = motion;
        floatingText.duration = duration;

        floatingText.Show();
    }

    private FloatingText GetFloatingText() // Disable ������ FloatingText ������Ʈ�� ã�� ��ȯ 
    {
        FloatingText txt = floatingTexts.Find(t => !t.active);

        if(txt == null) // Disable ������ FloatingText�� ���� �ÿ��� ���� ���� ��ȯ
        {
            txt = new FloatingText();
            txt.go = Instantiate(textPrefab);
            txt.go.transform.SetParent(textContainer.transform);
            txt.txt = txt.go.GetComponent<Text>();

            floatingTexts.Add(txt);
        }

        return txt;
    }

}