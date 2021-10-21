using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Utils;

public class ServerManager : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    public static float netTime = 0;

    void Start()
    {
        // PhotonPeer.RegisterType(typeof(ThisCard), (byte)'W', SerializeCard, DeserializeCard);

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.ConnectUsingSettings();

        StartCoroutine(WebCheck());

        PV = this.PV();
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

    public override void OnJoinedRoom()
    {
        print("JoinRoom");
    }

    void Update()
    {
        if (AllPlayerIn() && SceneManager.GetActiveScene().name != "IngameScene")
        {
            SceneManager.LoadScene("IngameScene");
        }
    }

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;

    public void MatchStartButton()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, null, null, null,
            new RoomOptions {MaxPlayers = 2});
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
    }

    public void SurrenderButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }

    IEnumerator WebCheck()
    {
        DateTime tTime = DateTime.Now.ToUniversalTime();

        while (true)
        {
            UnityWebRequest request;
            using (request = UnityWebRequest.Get("www.naver.com"))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError)
                {
                    print(request.error);
                }
                else
                {
                    string date = request.GetResponseHeader("date");
                    DateTime dateTime = DateTime.Parse(date).ToUniversalTime();

                    if (tTime != dateTime)
                    {
                        netTime++;
                        tTime = dateTime;
                    }

                    //print(netTime);
                }
            }
        }
    }

    public static float GetTime(out float time)
    {
        //netTime = 0;
        float fdsa;
        fdsa = netTime;
        time = netTime - fdsa;
        return time;
    }

    // public static readonly byte[] memCard = new byte[3 * 4];
    // private static short SerializeCard(StreamBuffer outStream, object customobject)
    // {
    //     ThisCard thisCard = (ThisCard) customobject;
    //     lock (memCard)
    //     {
    //         byte[] bytes = memCard;
    //         int index = 0;
    //         Protocol.Serialize(thisCard.cost, bytes, ref index);
    //         Protocol.Serialize(thisCard.id, bytes, ref index);
    //         Protocol.Serialize(thisCard.power, bytes, ref index);
    //         outStream.Write(bytes, 0, 2 * 4);
    //     }
    //
    //     return 2 * 4;
    // }
    //
    // private static object DeserializeCard(StreamBuffer inStream, short length)
    // {
    //     ThisCard thisCard = new ThisCard();
    //
    //     lock (memCard)
    //     {
    //         inStream.Read(memCard, 0, 2 * 4);
    //         int index = 0;
    //         Protocol.Serialize(thisCard.cost, memCard, ref index);
    //         Protocol.Serialize(thisCard.id, memCard, ref index);
    //         Protocol.Serialize(thisCard.power, memCard, ref index);
    //     }
    //
    //     return memCard;
    // }
}