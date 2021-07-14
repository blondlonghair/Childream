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
        print("���� ���� �Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
    }


    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause) => print("�������");


    public void JoinLobby() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => print("�κ� ���� �Ϸ�");


    public void CreateRoom() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    public void JoinRoom() => PhotonNetwork.JoinRoom(roomInput.text);
    public void JoinCreatRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnJoinedRoom() => print("�游��� �Ϸ�");
    public override void OnCreateRoomFailed(short returnCode, string message) => print("�� ����� ����");
    public override void OnJoinRoomFailed(short returnCode, string message) => print("�� ���� ����");
    public override void OnJoinRandomFailed(short returnCode, string message) => print("�� ���� ���� ����");

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ� �ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            foreach (var item in PhotonNetwork.PlayerList)
            {
                playerStr += item.NickName + ", ";
                print(playerStr);
            }
        }
        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("��������? : " + PhotonNetwork.IsConnected);
        }
    }
}