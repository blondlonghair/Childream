using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsAnimationOver : MonoBehaviour
{
    public bool isAnimationOver { get; set; }

    public void CheckAnimationOver() => isAnimationOver = true;
}
