using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroScene : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            SceneManager.LoadScene("CutScene");
        }

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}