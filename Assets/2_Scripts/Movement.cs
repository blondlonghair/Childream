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
            Move();
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 1 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, -1 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(1 * Time.deltaTime, 0, 0);
        }
    }

    [PunRPC]
    void ChangeColor()
    {
        SR.color = PV.IsMine ? Color.blue : Color.red;
    }
}
