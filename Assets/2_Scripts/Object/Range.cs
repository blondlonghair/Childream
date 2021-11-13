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

    void Start()
    {
        PV = this.PV();
        SR = GetComponent<SpriteRenderer>();
        
        if (PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(0,0,180);
            
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, 3.5f, 0);
                gameObject.name = "MasterRange";
                SR.sprite = frontRange;
            }

            else
            {
                transform.position = new Vector3(0, -3.5f, 0);
                SR.sprite = backRange;
                gameObject.name = "GuestRange";
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -3.5f, 0);
                SR.sprite = frontRange;
                gameObject.name = "GuestRange";
            }

            else
            {
                transform.position = new Vector3(0, 3.5f, 0);
                SR.sprite = backRange;
                gameObject.name = "MasterRange";
            }
        }
    }
}
