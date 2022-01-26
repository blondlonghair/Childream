using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    [SerializeField] private Sprite[] cutScene;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private int curCutScene = 0;
    private bool isCutSceneLerp;

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
        if (Input.GetMouseButtonDown(0) && !isCutSceneLerp)
        {
            isCutSceneLerp = true;
            StartCoroutine(nextSprite());
        }
    }

    IEnumerator nextSprite()
    {
        Color color = _spriteRenderer.color;
        while (color.a >= 0)
        {
            color.a -= 0.01f;
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        
        curCutScene++;
        
        if (curCutScene >= cutScene.Length)
        {
            SceneManager.LoadScene("TutorialScene");
        }

        _spriteRenderer.sprite = cutScene[curCutScene];
        
        while (color.a <= 1)
        {
            color.a += 0.01f;
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(0.01f);
        }

        isCutSceneLerp = false;
        yield return null;
    }
}