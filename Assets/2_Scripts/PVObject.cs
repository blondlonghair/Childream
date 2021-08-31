using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PVObject : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(0, 0, 180);
        }
    }
}
