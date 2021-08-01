using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Manager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;
    [SerializeField] GameObject gameobject;

    void Awake()
    {
        //PhotonNetwork.SendRate = 60;
        //PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 50 }, null);
    }

    public override void OnJoinedRoom()
    {
        Spawn();
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update() 
    {
        text.text = PhotonNetwork.NetworkClientState.ToString();
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect(); 
    }
}
