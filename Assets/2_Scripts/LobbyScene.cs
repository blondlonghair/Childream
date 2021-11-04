using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject MatchingDoor;
    
    public void MatchStartButton()
    {
        StartCoroutine(MatchStart());
        
        MatchingDoor.SetActive(true);
        MatchingDoor.GetComponent<Animator>().SetTrigger("DoorClose");
    }

    IEnumerator MatchStart()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions : new RoomOptions {MaxPlayers = 2});
        yield return null;
    }

    public void GoShopScene()
    {
        SceneManager.LoadScene("ShopScene");
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
        print(PhotonNetwork.NetworkClientState);
    }

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;
}
