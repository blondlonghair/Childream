using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    [SerializeField] private Slider myHpBar;
    [SerializeField] private Slider myMpBar;
    [SerializeField] private Slider EnemyHpBar;
    [SerializeField] private Slider EnemyMpBar;

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myHpBar.value = GameManager.Instance.HostPlayer.CurHp / GameManager.Instance.HostPlayer.MaxHp;
            myMpBar.value = GameManager.Instance.HostPlayer.CurMp / GameManager.Instance.HostPlayer.MaxMp;
            EnemyHpBar.value = GameManager.Instance.GuestPlayer.CurHp / GameManager.Instance.GuestPlayer.MaxHp;
            EnemyMpBar.value = GameManager.Instance.GuestPlayer.CurMp / GameManager.Instance.GuestPlayer.MaxMp;
        }
        else
        {
            myHpBar.value = GameManager.Instance.GuestPlayer.CurHp / GameManager.Instance.GuestPlayer.MaxHp;
            myMpBar.value = GameManager.Instance.GuestPlayer.CurMp / GameManager.Instance.GuestPlayer.MaxMp;
            EnemyHpBar.value = GameManager.Instance.HostPlayer.CurHp / GameManager.Instance.HostPlayer.MaxHp;
            EnemyMpBar.value = GameManager.Instance.HostPlayer.CurMp / GameManager.Instance.HostPlayer.MaxMp;
        }
    }
}