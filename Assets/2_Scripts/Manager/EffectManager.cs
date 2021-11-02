using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [SerializeField] private GameObject Atk1_1;
    [SerializeField] private GameObject Atk1_2;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject FindEffect(string effect)
    {
        switch (effect)
        {
            case "Atk1_1":
                return Atk1_1;
            case "Atk1_2":
                return Atk1_2;
            default:
                return null;
        }
    }
}
