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

    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, 5, 0);
            transform.Rotate(0, 0, 180);
            name = "HostPlayer";
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = new Vector3(0, -5, 0);
            name = "GuestPlayer";
        }
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
}
