using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnEndButton : MonoBehaviour
{
    private Animator anim;
    private Button button;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TurnEnd()
    {
        anim.SetTrigger("TurnEnd");
    }

    public void TurnStart()
    {
        anim.SetTrigger("TurnStart");
    }
}
