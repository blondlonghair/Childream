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
}
