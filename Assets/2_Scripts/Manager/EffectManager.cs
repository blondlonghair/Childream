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
    
    public void InitEffect(Player _caster, Player _target, int _index, int _effectId)
    {
        print(_effectId);
        Vector3 targetPos = new Vector3(PhotonNetwork.IsMasterClient ? 
            (float)(_index switch{1 => 3.5, 2 => 0, 3 => -3.5, _ => 0}) : 
            (float)(_index switch{1 => -3.5, 2 => 0, 3 => 3.5, _ => 0}), 
            _target.transform.position.y, 0);
        
        switch (_effectId)
        {
            case 1:
                StartCoroutine(EffectAtk1Update(_caster, _target, targetPos, Atk1_1, Atk1_2));
                break;
            case 2:
                Instantiate(Atk2, new Vector3(3.5f, targetPos.y, 0), quaternion.identity);
                Instantiate(Atk2, new Vector3(0, targetPos.y, 0), quaternion.identity);
                Instantiate(Atk2, new Vector3(-3.5f, targetPos.y, 0), quaternion.identity);
                break;
            case 3:
                Instantiate(Atk3, targetPos, Quaternion.Euler(90, 0, 0));
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                StartCoroutine(EffectSup1Update(_caster, _target, targetPos, Sup1_1, Sup1_2));
                StartCoroutine(EffectSup1Update(_caster, _target, targetPos, Sup1_1, Sup1_2));
                StartCoroutine(EffectSup1Update(_caster, _target, targetPos, Sup1_1, Sup1_2));
                break;
            case 8:
                
                break;
            case 9:

                break;
        }
    }

    IEnumerator EffectAtk1Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj;
        
        effectObj = Instantiate(_effect[0], _caster.transform.position, quaternion.identity);
        yield return null;

        while (effectObj.transform.position != _targetPos)
        {
            print(effectObj);
            effectObj.transform.position = Vector3.Lerp(effectObj.transform.position, _targetPos, 0.1f);
            yield return null;
        }

        Instantiate(_effect[1], _targetPos, quaternion.identity);
        yield return null;
    }

    IEnumerator EffectDefUpdate(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        yield return null;
    }

    IEnumerator EffectSup1Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj;
        
        effectObj = Instantiate(_effect[0], _caster.transform.position, quaternion.identity);
        yield return null;

        while (effectObj.transform.position != _targetPos)
        {
            print(effectObj);
            effectObj.transform.position = Vector3.Lerp(effectObj.transform.position, _targetPos, 0.1f);
            yield return null;
        }

        Instantiate(_effect[1], _targetPos, quaternion.identity);
        yield return null;
    }
}
