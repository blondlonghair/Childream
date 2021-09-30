using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Text.RegularExpressions;
using Utils;
using Enums;
using UnityEngine.EventSystems;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("플레이어 정보")] public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;
    public int MaxMoveCount;
    public int CurMoveCount;

    public int CurState;
    public bool IsLocked = false;
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

        PlayerMove();
        MouseInput();
    }

    bool CastRay(params string[] _tag)
    {
        raycastTarget = null;

        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hits = Physics2D.Raycast(pos, Vector2.zero, 0f);

        foreach (var tag in _tag)
        {
            if (hits.collider != null && hits.collider.gameObject.CompareTag(tag))
            {
                raycastTarget = hits.collider.gameObject;
            }
        }

        return true;
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
        CurState = 2;
        CurMoveCount = MaxMoveCount;
        CurHp = MaxHp;
        CurMp = MaxMp;

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
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
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
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                // print("Host");
            }
        }
    }

    void PlayerMove() //키보드
    {
        if (CurMoveCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.AddBattleList(1, 10, PhotonNetwork.IsMasterClient);
            CurMoveCount--;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.Instance.AddBattleList(2, 10, PhotonNetwork.IsMasterClient);
            CurMoveCount--;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.AddBattleList(3, 10, PhotonNetwork.IsMasterClient);
            CurMoveCount--;
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
                raycastTarget.transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);
                // print("down");
            }
        }

        if (CastRay("Card") == )
        {
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<PhotonView>().IsMine)
                raycastTarget.transform.position = worldMousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRayRange();

            CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);

            if (rangeTarget == null)
                return;

            if (!rangeTarget.GetComponent<PhotonView>().IsMine)
            {
                if (CurMp < raycastTarget.GetComponent<ThisCard>().cost)
                    return;

                CurMp -= raycastTarget.GetComponent<ThisCard>().cost;

                GameManager.Instance.AddBattleList(SelectRange,
                    raycastTarget is Move ? 10 : raycastTarget.GetComponent<ThisCard>().id,
                    PhotonNetwork.IsMasterClient);

                var targetPV = raycastTarget.GetComponent<PhotonView>().ViewID;
                PhotonNetwork.Destroy(PhotonView.Find(targetPV));
            }

            raycastTarget = null;
        }
    }
}