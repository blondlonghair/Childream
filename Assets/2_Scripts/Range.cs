using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Range : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, -2, 0);
            name = "MasterRange";
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, 2, 0);
            name = "GuestRange";
        }
    }
}
