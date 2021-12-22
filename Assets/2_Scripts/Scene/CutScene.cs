using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    [SerializeField] private Sprite[] cutScene;
    [SerializeField] private SpriteRenderer cutSceneBG;
    private int curCutScene = 0;

    private event Action _tutorialAction; 

    private void Start()
    {
        PlayerPrefs.SetInt("Tutorial", 0);
    }

    private void Update()
    {
        CutSceneScene();
    }

    void CutSceneScene()
    {
        if (Input.GetMouseButtonDown(0))
        {
            curCutScene++;
            cutSceneBG.sprite = cutScene[curCutScene];

            if (curCutScene >= cutScene.Length - 1)
            {
                SceneManager.LoadScene("TutorialScene");
            }
        }
    }
}