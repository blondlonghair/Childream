using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Range : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();

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
}
