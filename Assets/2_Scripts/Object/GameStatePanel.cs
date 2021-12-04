using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class GameStatePanel : MonoBehaviour
{
    [SerializeField] private Sprite resultPanel;
    [SerializeField] private Sprite turnPanel;
    
    private Image statePanel;
    
    void Start()
    {
        statePanel = gameObject.GetComponent<Image>();
    }

    public void ShowPanel(PanelState panelState)
    {
         StartCoroutine(WaitTime(panelState));
    }

    IEnumerator WaitTime(PanelState panelState)
    {
        if (panelState == PanelState.Result)
        {
            statePanel.sprite = resultPanel;
        }
        else
        {
            statePanel.sprite = turnPanel;
        }
        
        float panel = statePanel.color.a;
        Color panelColor = statePanel.color;

        while (statePanel.color.a < 0.99f)
        {
            panel += Time.deltaTime * 5;
            panelColor.a = panel;
            statePanel.color = panelColor;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (statePanel.color.a > 0)
        {
            panel -= Time.deltaTime * 5;
            panelColor.a = panel;
            statePanel.color = panelColor;
            yield return null;
        }
    }
}
