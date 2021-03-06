using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public bool isEnd;
    
    private Coroutine _coroutine;

    private void Start()
    {
        Open();
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.enabled = true;
    }

    public void Open()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(Co_OpenPanel());
    }

    public void Close(string sceneName)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(Co_ClosePanel(sceneName));
    }

    IEnumerator Co_OpenPanel()
    {
        while (transform.position.y < 39.9f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 40, 0), 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Co_ClosePanel(string sceneName)
    {
        while (transform.position.y > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, 0), 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene(sceneName);
    }
}