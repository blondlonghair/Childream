using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingDoor : MonoBehaviour
{
    public bool isAnimationOver = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void IsAnimationOver()
    {
        isAnimationOver = true;
    }

    public void OpenDoor()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("DoorOpen");
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("DoorClose");
    }
}
