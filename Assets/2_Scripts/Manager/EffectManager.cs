using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class EffectManager : MonoBehaviourPunCallbacks
{
    public static EffectManager Instance;

    [SerializeField] private GameObject Atk1_1;
    [SerializeField] private GameObject Atk1_2;
    [SerializeField] private GameObject Atk2;
    [SerializeField] private GameObject Atk3;
    [SerializeField] private GameObject Def1_1;
    [SerializeField] private GameObject Def1_2;
    [SerializeField] private GameObject Def2_1;
    [SerializeField] private GameObject Def2_2;
    [SerializeField] private GameObject Def3_1;
    [SerializeField] private GameObject Def3_2;
    [SerializeField] private GameObject Sup1_1;
    [SerializeField] private GameObject Sup1_2;
    [SerializeField] private GameObject Sup2;
    [SerializeField] private GameObject Sup3;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        
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
    
    public void InitEffect(Player _caster, Player _target, int _index, string _effect)
    {
        print("InitEffect");
        Vector3 targetPos = Vector3.one;

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     GameObject temp = GameObject.FindWithTag($"Range{_index}");
        //     if (temp.transform.parent.name == "GuestRange")
        //         targetPos = temp.transform.position;
        // }
        // else
        // {
        //     GameObject temp = GameObject.FindWithTag($"Range{_index}");
        //     if (temp.transform.parent.name == "MasterRange")
        //         targetPos = temp.transform.position;
        // }

        targetPos = _target.transform.position;
        
        switch (_effect)
        {
            case "Atk1":
                StartCoroutine(EffectUpdate(_caster, _target, targetPos, Atk1_1, Atk1_2));
                break;
            case "Atk2":
                Instantiate(Atk1_2, transform.position, quaternion.identity);
                break;
            default:
                break;
        }
    }

    IEnumerator EffectUpdate(Player _caster, Player _target, Vector3 targetPos, params GameObject[] _effect)
    {
        print("EffectUpdate");

        GameObject effectObj;
        
        print(targetPos);
        
        effectObj = Instantiate(_effect[0], _caster.transform.position, quaternion.identity);
        yield return null;

        while (effectObj.transform.position != targetPos)
        {
            print(effectObj);
            effectObj.transform.position = Vector3.Lerp(effectObj.transform.position, targetPos, 0.1f);
            yield return null;
        }

        Instantiate(_effect[1], targetPos, quaternion.identity);
        yield return null;
    }
}
