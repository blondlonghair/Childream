using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("플레이어 정보")]
    public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;

    PhotonView PV;
    GameObject raycastTarget;

    //bool isStart = false;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        PlayerSetup();
    }

    void Update()
    {
        if (PV.IsMine)
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
    }

    void CastRay(string _tag)
    {
        raycastTarget = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.CompareTag(_tag))
            {
                raycastTarget = hits[i].collider.gameObject;
            }
        }
    }

    void PlayerSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, 5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "HostPlayer";
            }
            else
            {
                transform.position = new Vector3(0, -5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "GuestPlayer";
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -5, 0);
                gameObject.name = "HostPlayer";
            }
            else
            {
                transform.position = new Vector3(0, 5, 0);
                gameObject.name = "GuestPlayer";
            }
        }
    }
}
