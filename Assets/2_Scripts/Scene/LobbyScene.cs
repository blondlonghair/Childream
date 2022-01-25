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
    private float matchDelay;

    private void Start()
    {
        // matchingDoor.OpenDoor();
    }
    
    public void Update()
    {
        if (matchingDoor.isCloseOver)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions : new RoomOptions {MaxPlayers = 2});
            cancelButton.SetActive(true);
            matchingDoor.isCloseOver = false;
        }

        if (AllPlayerIn())
        {
            matchDelay += Time.deltaTime;
        }

        if (loadOnce && matchDelay >= 2f)
        {
            PhotonNetwork.LoadLevel("IngameScene");
            loadOnce = false;
        }
    }

    public void MatchStartButton()
    {
        print(GameObject.Find("SoundManager"));
        // GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySFXSound("Button_matching");
        // SoundManager.Instance.PlaySFXSound("Button_matching");
        
        matchingDoor.gameObject.SetActive(true);
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

    IEnumerator AddTime()
    {
        while (true)
        {
            matchDelay += Time.deltaTime;
            yield return null;
        }
    }
}
