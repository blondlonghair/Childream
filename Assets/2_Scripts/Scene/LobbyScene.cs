using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private MatchingDoor matchingDoor;

    private void Start()
    {
        matchingDoor.OpenDoor();
    }

    public void MatchStartButton()
    {
        matchingDoor.CloseDoor();

        if (matchingDoor.isAnimationOver)
        {
            StartCoroutine(time(2));
        }
    }

    IEnumerator time(float t)
    {
        yield return new WaitForSeconds(t);
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions : new RoomOptions {MaxPlayers = 2});
        yield return null;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("IngameScene");
        print("JoinRoom");
    }
    
    public void Update()
    {
        // print(PhotonNetwork.NetworkClientState);
    }

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;
}
