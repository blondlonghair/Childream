using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        print("doorOpen");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name == "IngameScene")
        {
            if (TryGetComponent(out Image image))
            {
                image.enabled = true;
            }
            animator.SetTrigger("DoorOpen");
        }
    }

    private void Update()
    {
        if (isOpenOver)
        {
            gameObject.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        animator.SetTrigger("DoorOpen");
        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("DoorClose");
    }
}
