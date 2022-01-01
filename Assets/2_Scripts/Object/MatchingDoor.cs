using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingDoor : MonoBehaviour
{
    public bool isCloseOver { get; set; }
    public bool isOpenOver { get; set; }
    [SerializeField] private Animator animator;

    public void IsCloseOver()
    {
        isCloseOver = true;
    }

    public void IsOpenOver()
    {
        isOpenOver = true;
    }

    public void OpenDoor()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("DoorOpen");
        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
        animator = GetComponent<Animator>();
        animator.SetTrigger("DoorClose");
    }
}
