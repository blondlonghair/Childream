using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;
using System;

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

        PV = GetComponent<PhotonView>();
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
        //PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
        PhotonNetwork.Instantiate("Card", new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update() 
    {
        text.text = PhotonNetwork.NetworkClientState.ToString();
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect(); 
    }

    IEnumerator WebCheck()
    {
        while (true)
        {
            _ = new UnityWebRequest();
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
                    TimeSpan timestamp = dateTime - new DateTime(2021, 8, 7, 0, 0, 0);

                    int stopwatch = (int)timestamp.TotalSeconds - PlayerPrefs.GetInt("net", (int)timestamp.TotalSeconds);
                    print(stopwatch + "sec");
                    PlayerPrefs.SetInt("net", (int)timestamp.TotalSeconds);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
