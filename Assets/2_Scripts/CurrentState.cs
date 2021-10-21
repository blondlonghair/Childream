using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CurrentState : MonoBehaviourPunCallbacks
{
    private Text curText;

    private void Start()
    {
        curText = GetComponent<Text>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        curText.text = PhotonNetwork.NetworkClientState.ToString();
    }
}
