using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class EffectManager : SingletonMonoDestroy<EffectManager>
{
    [SerializeField] private GameObject attack1;
    [SerializeField] private GameObject attack2;
    [SerializeField] private GameObject attack3;
    [SerializeField] private GameObject defence;
    [SerializeField] private GameObject support1;
    [SerializeField] private GameObject support2;

    // [SerializeField] private GameObject Atk1_1;
    // [SerializeField] private GameObject Atk1_2;
    // [SerializeField] private GameObject Atk2;
    // [SerializeField] private GameObject Atk3;
    // [SerializeField] private GameObject Def1;
    // [SerializeField] private GameObject Def2;
    // [SerializeField] private GameObject Def3;
    // [SerializeField] private GameObject Sup1_1;
    // [SerializeField] private GameObject Sup1_2;
    // [SerializeField] private GameObject Sup2;
    // [SerializeField] private GameObject Sup3;

    private float rangePos = 2.7f;

    public void InitEffect(Player _caster, Player _target, int _index, int _effectId)
    {
        SpawnEffect(_caster, _target, _effectId);

        // Vector3 targetPos = new Vector3(PhotonNetwork.IsMasterClient ? 
        //     _index switch{1 => rangePos, 2 => 0, 3 => -rangePos, _ => 0} : 
        //     _index switch{1 => -rangePos, 2 => 0, 3 => rangePos, _ => 0}, 
        //     _target.transform.position.y, 0);
        //
        // switch (_effectId)
        // {
        //     case 1: StartCoroutine(EffectAtk1Update(_caster, _target, targetPos, Atk1_1, Atk1_2)); break;
        //     case 2: StartCoroutine(EffectAtk2Update(_caster, _target, targetPos, Atk2)); break;
        //     case 3: StartCoroutine(EffectAtk3Update(_caster, _target, targetPos, Atk3)); break;
        //     case 4: StartCoroutine(EffectDefUpdate(_caster, _target, targetPos, Def1)); break;
        //     case 5: StartCoroutine(EffectDefUpdate(_caster, _target, targetPos, Def2)); break;
        //     case 6: StartCoroutine(EffectDefUpdate(_caster, _target, targetPos, Def3)); break;
        //     case 7: StartCoroutine(EffectSup1Update(_caster, _target, targetPos, Sup1_1, Sup1_2)); break;
        //     case 8: StartCoroutine(EffectSup2Update(_caster, _target, targetPos, Sup2)); break;
        //     case 9: StartCoroutine(EffectSup3Update(_caster, _target, targetPos, Sup3)); break;
        // }
    }

    private void SpawnEffect(Player _caster, Player _target, int _effectId)
    {
        GameObject obj;
        Player player;
        
        switch (_effectId)
        {
            case 1 : obj = attack1; player = _target; break;
            case 2 : obj = attack2; player = _target; break;
            case 3 : obj = attack3; player = _target; break;
            case 4 : case 5: case 6 : obj = defence; player = _target; break;
            case 7 : obj = support1; player = _caster; break;
            case 8 : obj = support2; player = _caster; break;
            default : obj = null; player = _caster; break;
        }

        if (_effectId != 9)
        {
            Instantiate(obj, player.transform.position, Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0), player.transform);
            
        }
    }

    IEnumerator EffectAtk1Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj = Instantiate(_effect[0], _caster.transform.position, quaternion.identity);
        yield return null;

        while (effectObj.transform.position != _targetPos)
        {
            effectObj.transform.position = Vector3.Lerp(effectObj.transform.position, _targetPos, 0.1f);
            yield return null;
        }
        Destroy(effectObj);
        yield return null;
        
        GameObject effectobj = Instantiate(_effect[1], _targetPos, quaternion.identity);
        yield return new WaitForSeconds(1);
        
        Destroy(effectobj);
        yield return null;
    }

    IEnumerator EffectAtk2Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject[] effectObj = new GameObject[3];
        effectObj[0] = Instantiate(_effect[0], new Vector3(rangePos, _targetPos.y, 0), quaternion.identity);
        effectObj[1] = Instantiate(_effect[0], new Vector3(0, _targetPos.y, 0), quaternion.identity);
        effectObj[2] = Instantiate(_effect[0], new Vector3(-rangePos, _targetPos.y, 0), quaternion.identity);

        yield return new WaitForSeconds(2);
        for (int i = 0; i < 3; i++)
        {
            Destroy(effectObj[i]);
        }

        yield return null;
    }

    IEnumerator EffectAtk3Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj = Instantiate(_effect[0], _targetPos, Quaternion.Euler(90,0,0));
        yield return new WaitForSeconds(2);

        Destroy(effectObj);
        yield return null;
    }

    IEnumerator EffectDefUpdate(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj = Instantiate(_effect[0], _targetPos, quaternion.identity);
        yield return new WaitForSeconds(2);
        
        Destroy(effectObj);
        yield return null;
    }

    IEnumerator EffectSup1Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject[] effectObj = new GameObject[] {};
        
        effectObj[0] = Instantiate(_effect[0], new Vector3(rangePos, _caster.transform.position.y, 0), quaternion.identity);
        effectObj[1] = Instantiate(_effect[0], new Vector3(0, _caster.transform.position.y, 0), quaternion.identity);
        effectObj[2] = Instantiate(_effect[0], new Vector3(-rangePos, _caster.transform.position.y, 0), quaternion.identity);
        yield return null;

        while (effectObj[0].transform.position != _targetPos)
        {
            print(effectObj);
            effectObj[0].transform.position = Vector3.Lerp(effectObj[0].transform.position, _targetPos, 0.1f);
            effectObj[1].transform.position = Vector3.Lerp(effectObj[1].transform.position, _targetPos, 0.1f);
            effectObj[2].transform.position = Vector3.Lerp(effectObj[2].transform.position, _targetPos, 0.1f);
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            Destroy(effectObj[i]);
        }

        effectObj[3] = Instantiate(_effect[1], _targetPos, quaternion.identity);
        yield return null;

        yield return new WaitForSeconds(1);
        Destroy(effectObj[3]);
        yield return null;
    }

    IEnumerator EffectSup2Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj = Instantiate(_effect[0], _caster.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2);

        Destroy(effectObj);
        yield return null;
    }

    IEnumerator EffectSup3Update(Player _caster, Player _target, Vector3 _targetPos, params GameObject[] _effect)
    {
        GameObject effectObj = Instantiate(_effect[0], Vector3.zero, quaternion.identity);
        yield return new WaitForSeconds(2);

        Destroy(effectObj);
        yield return null;
    }
}
