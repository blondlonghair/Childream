using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

public class DBManager : MonoBehaviourPunCallbacks
{
    public Text statusText;
    public InputField nickNameInput, roomInput;

    void Awake() => Screen.SetResolution(450, 1000, false);
    void Update() => statusText.text = PhotonNetwork.NetworkClientState.ToString();


    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        print("서버 접속 완료");
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
    }


    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");


    public void JoinLobby() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => print("로비 접속 완료");


    public void CreateRoom() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    public void JoinRoom() => PhotonNetwork.JoinRoom(roomInput.text);
    public void JoinCreatRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnJoinedRoom() => print("방만들기 완료");
    public override void OnCreateRoomFailed(short returnCode, string message) => print("방 만들기 실패");
    public override void OnJoinRoomFailed(short returnCode, string message) => print("방 참가 실패");
    public override void OnJoinRandomFailed(short returnCode, string message) => print("방 랜덤 참가 실패");

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대 인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            foreach (var item in PhotonNetwork.PlayerList)
            {
                playerStr += item.NickName + ", ";
                print(playerStr);
            }
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됬는지? : " + PhotonNetwork.IsConnected);
        }
    }
}