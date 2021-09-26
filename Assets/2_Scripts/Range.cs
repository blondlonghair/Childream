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

    private SpriteRenderer SR;
    [SerializeField] private Sprite frontRange;
    [SerializeField] private Sprite backRange;

    void Awake()
    {
        PV = this.PV();
        SR = GetComponent<SpriteRenderer>();
        
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, -2, 0);
            transform.Rotate(0,0,180);
            
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, 7, 0);
                (childs[0].transform.position, childs[2].transform.position) = (childs[2].transform.position, childs[0].transform.position);
                gameObject.name = "MasterRange";
                SR.sprite = frontRange;
            }

            else
            {
                transform.position = new Vector3(0, -2, 0);
                SR.sprite = backRange;
                gameObject.name = "GuestRange";
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -7, 0);
                SR.sprite = frontRange;
                gameObject.name = "GuestRange";
                (childs[0].transform.position, childs[2].transform.position) = (childs[2].transform.position, childs[0].transform.position);
            }

            else
            {
                transform.position = new Vector3(0, 2, 0);
                SR.sprite = backRange;
                gameObject.name = "MasterRange";
            }
        }
    }
}
