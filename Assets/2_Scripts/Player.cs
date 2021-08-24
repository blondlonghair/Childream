using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [Header("플레이어 정보")]
    public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;

    PhotonView PV;
    GameObject raycastTarget;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        //PV.RPC(nameof(PlayerSetup), RpcTarget.AllBuffered);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay("Card");

            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<PhotonView>().IsMine)
            {
                print("down");
            }
        }

        if (Input.GetMouseButton(0))
        {
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRay("Range");

            if (raycastTarget == null)
                return;

            if (!raycastTarget.GetComponentInParent<PhotonView>().IsMine)
            {
                print("up");
            }
        }
    }

    void CastRay(string _tag)
    {
        raycastTarget = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag(_tag))
        {
            raycastTarget = hit.collider.gameObject;
        }
    }

    [PunRPC]
    void PlayerSetup()
    {
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = new Vector3(0, 5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "HostPlayer";
                print("IsMasterClient");
            }
            else
            {
                transform.position = new Vector3(0, -5, 0);
                gameObject.name = "GuestPlayer";
                print("IsGuestClient");
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, 5, 0);
            transform.Rotate(0, 0, 180);
            gameObject.name = "HostPlayer";
            print("IsMasterClient");
        }
        else
        {
            transform.position = new Vector3(0, -5, 0);
            gameObject.name = "GuestPlayer";
            print("IsGuestClient");
        }
    }
}
