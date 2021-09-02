using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class Range : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    [SerializeField] private GameObject[] childs;

    void Awake()
    {
        PV = this.PV();
        
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, 5, 0);
            transform.Rotate(0,0,180);
            
            if (PV.IsMine)
            {
                (childs[0].transform.position, childs[2].transform.position) = (childs[2].transform.position, childs[0].transform.position);
                gameObject.name = "MasterRange";
            }

            else
            {
                gameObject.name = "GuestRange";
            }
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, -5, 0);

            if (PV.IsMine)
            {
                gameObject.name = "GuestRange";
                (childs[0].transform.position, childs[2].transform.position) = (childs[2].transform.position, childs[0].transform.position);
            }

            else
            {
                gameObject.name = "MasterRange";
            }
        }
    }

    void OnEnter()
    {
        
    }
}
