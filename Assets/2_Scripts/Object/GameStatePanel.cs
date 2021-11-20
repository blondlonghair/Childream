using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class GameStatePanel : MonoBehaviour
{
    private Text stateText;
    private Image statePanel;
    
    void Start()
    {
        stateText = transform.GetChild(0).GetComponent<Text>();
        statePanel = gameObject.GetComponent<Image>();
    }

    public void ShowPanel(string s)
    {
         StartCoroutine(WaitTime(s));
    }

    IEnumerator WaitTime(string s)
    {
        stateText.text = s;
        float panel = statePanel.color.a;
        float text = stateText.color.a;
        Color panelColor = statePanel.color;
        Color textColor = stateText.color;

        while (statePanel.color.a < 0.99f && statePanel.color.a < 0.99f)
        {
            print("turning On");
            panel += 0.05f;
            text += 0.05f;
            panelColor.a = panel;
            textColor.a = text;
            statePanel.color = panelColor;
            stateText.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        while (statePanel.color.a > 0 && stateText.color.a > 0)
        {
            print("turning Off");
            panel -= 0.05f;
            text -= 0.05f;
            panelColor.a = panel;
            textColor.a = text;
            statePanel.color = panelColor;
            stateText.color = textColor;
            yield return null;
        }
    }
}
