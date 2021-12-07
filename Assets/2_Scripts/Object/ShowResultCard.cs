using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowResultCard : MonoBehaviour
{
    public int cardId;
    [SerializeField] private Sprite[] sprite;
    
    private SpriteRenderer sr;
    
    private void Start()
    {
        if (TryGetComponent(out sr))
        {
            sr.sprite = sprite[cardId - 1];
        }
    }

    public void DestroyCard()
    {
        Destroy(gameObject);
    }
}
