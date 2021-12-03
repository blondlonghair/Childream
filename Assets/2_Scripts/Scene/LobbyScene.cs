using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private MatchingDoor matchingDoor;
    [SerializeField] private GameObject cancelButton;
    
    private bool loadOnce = true;

    private void Start()
    {
        matchingDoor.OpenDoor();
    }
    
    public void Update()
    {
        if (matchingDoor.isAnimationOver)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions : new RoomOptions {MaxPlayers = 2});
            cancelButton.SetActive(true);
            matchingDoor.isAnimationOver = false;
        }

        if (AllPlayerIn() && loadOnce)
        {
            // SceneManager.LoadScene("IngameScene");
            PhotonNetwork.LoadLevel("IngameScene");
            loadOnce = false;
        }
    }

    public void MatchStartButton()
    {
        matchingDoor.CloseDoor();
    }

    public void MatchCancelButton()
    {
        if (!AllPlayerIn())
        {
            PhotonNetwork.LeaveRoom();
            cancelButton.SetActive(false);
            matchingDoor.OpenDoor();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
    }

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;
}