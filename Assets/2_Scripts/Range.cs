using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class Range : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    void Awake()
    {
        PV = this.PV();

        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, 5, 0);
            gameObject.name = "MasterRange";
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, -5, 0);
            gameObject.name = "GuestRange";
        }
    }

    void OnEnter()
    {
        
    }
}
