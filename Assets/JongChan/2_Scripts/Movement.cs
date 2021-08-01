using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Movement : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    SpriteRenderer SR;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        SR = GetComponent<SpriteRenderer>();
        PV.RPC(nameof(ChangeColor), RpcTarget.All);
    }

    void Update()
    {
        if (PV.IsMine)
        {
            PV.RPC(nameof(Move), RpcTarget.All);
        }
    }

    [PunRPC]
    void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (PV.IsMine)
                transform.Translate(0, 1 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (PV.IsMine)
                transform.Translate(-1 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (PV.IsMine)
                transform.Translate(0, -1 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (PV.IsMine)
                transform.Translate(1 * Time.deltaTime, 0, 0);
        }
    }

    [PunRPC]
    void ChangeColor()
    {
        SR.color = PV.IsMine ? Color.blue : Color.red;
    }
}
