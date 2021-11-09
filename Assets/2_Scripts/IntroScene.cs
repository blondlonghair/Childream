using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private Sprite[] cutScene;
    [SerializeField] private SpriteRenderer cutSceneBG;
    private int curCutScene = 0;
    private void Start()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        print("JoinLobby");
        SceneManager.LoadScene("LobbyScene");
    }

    private void Update()
    {
        // if (Time.time >= 8)
        // {
        //     PhotonNetwork.ConnectUsingSettings();
        // }

        if (Input.GetMouseButtonDown(0))
        {
            curCutScene++;
            cutSceneBG.sprite = cutScene[curCutScene];

            if (curCutScene >= cutScene.Length - 1)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }
}
