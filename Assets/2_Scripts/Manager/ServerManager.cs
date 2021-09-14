using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;
using System;
using ExitGames.Client.Photon;
using Utils;

public class ServerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;
    PhotonView PV;

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(ThisCard), (byte)'W', SerializeCard, DeserializeCard);
        
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.ConnectUsingSettings();

        StartCoroutine(WebCheck());

        PV = this.PV();
        
        // PhotonPeer.RegisterType(Player, )
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Spawn();
    }

    public void Spawn()
    {
    }

    void Update()
    {
        text.text = PhotonNetwork.NetworkClientState.ToString();
    }

    static float netTime = 0;

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
    
    public static readonly byte[] memCard = new byte[3 * 4];
    private static short SerializeCard(StreamBuffer outStream, object customobject)
    {
        ThisCard thisCard = (ThisCard) customobject;
        lock (memCard)
        {
            byte[] bytes = memCard;
            int index = 0;
            Protocol.Serialize(thisCard.cost, bytes, ref index);
            Protocol.Serialize(thisCard.id, bytes, ref index);
            Protocol.Serialize(thisCard.power, bytes, ref index);
            outStream.Write(bytes, 0, 2 * 4);
        }

        return 2 * 4;
    }
    
    private static object DeserializeCard(StreamBuffer inStream, short length)
    {
        ThisCard thisCard = new ThisCard();

        lock (memCard)
        {
            inStream.Read(memCard, 0, 2 * 4);
            int index = 0;
            Protocol.Serialize(thisCard.cost, memCard, ref index);
            Protocol.Serialize(thisCard.id, memCard, ref index);
            Protocol.Serialize(thisCard.power, memCard, ref index);
        }

        return memCard;
    }
    
}