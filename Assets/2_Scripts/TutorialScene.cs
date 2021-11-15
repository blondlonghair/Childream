using System;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private Sprite[] cutScene;
    [SerializeField] private SpriteRenderer cutSceneBG;
    private int curCutScene = 0;
    
    private void Update()
    {
        CutScene();
    }

    void CutScene()
    {
        if (Input.GetMouseButtonDown(0))
        {
            curCutScene++;
            cutSceneBG.sprite = cutScene[curCutScene];

            if (curCutScene >= cutScene.Length - 1)
            {
                //튜토리얼 시작 재생
            }
        }
    }
}