using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpSlider : MonoBehaviour
{
    private Slider backSlider;
    private Slider frontSlider;
    
    void Start()
    {
        backSlider = transform.GetChild(0).GetComponent<Slider>();
        frontSlider = transform.GetChild(1).GetComponent<Slider>();
    }

    public void SliderValue(float value)
    {
        StartCoroutine(_SliderValue(value));
    }

    IEnumerator _SliderValue(float value)
    {
        frontSlider.value = value;
        while (value < backSlider.value)
        {
            yield return null;
            backSlider.value = Mathf.Lerp(backSlider.value, value, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
