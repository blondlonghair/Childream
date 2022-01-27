using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroScene : MonoBehaviourPunCallbacks
{
    [SerializeField] LoadingPanel _loadingPanel;
    
    private void Start()
    {
        // PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            SceneManager.LoadScene("CutScene");
        }
        
        SoundManager.Instance.PlayBGMSound();

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
        _loadingPanel.Close("LobbyScene");
    }
}