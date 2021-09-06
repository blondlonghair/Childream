using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Text.RegularExpressions;
using Utils;
using Enums;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("�÷��̾� ����")] 
    public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;
    public int MaxMoveCount;
    public int CurMoveCount;

    [HideInInspector] public int PlayerCurState;
    [HideInInspector] public bool IsPlayerLocked;
    [HideInInspector] public int SelectRange;

    PhotonView PV;
    GameObject raycastTarget;
    GameObject rangeTarget;

    public static Vector3 worldMousePos;

    void Start()
    {
        PV = this.PV();

        // GameManager.Instance.AddPlayer(this, PhotonNetwork.IsMasterClient);
        PlayerSetup();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        if (!PV.IsMine) return;

        PlayerMove();
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
                // print("Host");
            }
            else
            {
                transform.position = new Vector3(0, -5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "GuestPlayer";
                // print("Guest");
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -5, 0);
                gameObject.name = "GuestPlayer";
                // print("Guest");
            }
            else
            {
                transform.position = new Vector3(0, 5, 0);
                gameObject.name = "HostPlayer";
                // print("Host");
            }
        }
    }

    void PlayerMove()
    {
        if (CurMoveCount <= MaxMoveCount)
            return;
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PV.RPC(nameof(MovePlayerIndex), RpcTarget.AllBuffered, 1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            PV.RPC(nameof(MovePlayerIndex), RpcTarget.AllBuffered, 2);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PV.RPC(nameof(MovePlayerIndex), RpcTarget.AllBuffered, 3);
        }
    }

    [PunRPC]
    void MovePlayerIndex(int index)
    {
        if (index == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = new Vector3(3.5f, transform.position.y, 0);
            }
            else
            {
                transform.position = new Vector3(-3.5f, transform.position.y, 0);
            }
        }

        else if (index == 2)
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }

        else if (index == 3)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = new Vector3(-3.5f, transform.position.y, 0);
            }
            else
            {
                transform.position = new Vector3(3.5f, transform.position.y, 0);
            }
        }

        CurMoveCount--;
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
                // print("down");
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null)
                return;

            raycastTarget.transform.position = worldMousePos;

            // if (raycastTarget.GetComponent<ThisCard>().targetType == TargetType.ALL)
            // {
            //     raycastTarget.transform.position = worldMousePos;
            // }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRayRange();

            if (rangeTarget == null)
                return;

            if (!rangeTarget.GetComponent<PhotonView>().IsMine)
            {
                // print("up" + rangeTarget.tag);
                // print(SelectRange);
                // print(raycastTarget.GetComponent<ThisCard>().id);
                // print(PhotonNetwork.IsMasterClient);

                GameManager.Instance.AddBattleList(SelectRange, raycastTarget.GetComponent<ThisCard>().id,
                    PhotonNetwork.IsMasterClient);
            }
        }
    }
}