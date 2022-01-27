using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Online
{
    public class PVObject : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                transform.Rotate(0, 0, 180);
            }
        }
    }
}