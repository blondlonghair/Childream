using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Utils;
using Enums;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("플레이어 정보")]
    public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;

    public int PlayerCurState;
    public bool IsPlayerLocked;
    public int SelectRange;

    PhotonView PV;
    GameObject raycastTarget;
    GameObject rangeTarget;

    public static Vector3 worldMousePos;

    void Start()
    {
        PV = this.PV();

        PlayerSetup();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        if (!PV.IsMine) return;

        MouseInput();
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

    void CastRayRange()
    {
        rangeTarget = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.tag.Contains("Range"))
            {
                SelectRange = int.Parse(hits[i].collider.gameObject.tag.Replace("Range", ""));
                rangeTarget = hits[i].collider.gameObject;
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
                print("Host");
            }
            else
            {
                transform.position = new Vector3(0, -5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "GuestPlayer";
                print("Guest");

            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -5, 0);
                gameObject.name = "GuestPlayer";
                print("Guest");

            }
            else
            {
                transform.position = new Vector3(0, 5, 0);
                gameObject.name = "HostPlayer";
                print("Host");

            }
        }
    }

    void MouseInput()
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
            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<ThisCard>().targetType == TargetType.ALL)
            {
                raycastTarget.transform.position = worldMousePos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRayRange();

            if (rangeTarget == null)
                return;

            if (!rangeTarget.GetComponent<PhotonView>().IsMine)
            {
                print("up" + rangeTarget.tag);
                print(SelectRange);
            }
        }
    }
}
