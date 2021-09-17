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

    void CastRay(params string[] _tag)
    {
        raycastTarget = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);
        
        for (int i = 0; i < hits.Length; i++)
        {
            foreach (var tag in _tag)
            {
                if (hits[i].collider != null && hits[i].collider.gameObject.CompareTag(tag))
                {
                    raycastTarget = hits[i].collider.gameObject;
                }
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

    void PlayerMove() //키보드 이동
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

    // [PunRPC]
    // void MovePlayerIndex(int index)
    // {
    //     if (index == 1)
    //     {
    //         if (PhotonNetwork.IsMasterClient)
    //         {
    //             transform.position = new Vector3(3.5f, transform.position.y, 0);
    //         }
    //         else
    //         {
    //             transform.position = new Vector3(-3.5f, transform.position.y, 0);
    //         }
    //     }
    //
    //     else if (index == 2)
    //     {
    //         transform.position = new Vector3(0, transform.position.y, 0);
    //     }
    //
    //     else if (index == 3)
    //     {
    //         if (PhotonNetwork.IsMasterClient)
    //         {
    //             transform.position = new Vector3(-3.5f, transform.position.y, 0);
    //         }
    //         else
    //         {
    //             transform.position = new Vector3(3.5f, transform.position.y, 0);
    //         }
    //     }
    //
    //     CurState = index;
    //     CurMoveCount--;
    // }

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

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<PhotonView>().IsMine)
                raycastTarget.transform.position = worldMousePos;

            // if (raycastTarget.GetComponent<ThisCard>().targetType == TargetType.ALL)
            // {
            //     raycastTarget.transform.position = worldMousePos;
            // }
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

                GameManager.Instance.AddBattleList(SelectRange, raycastTarget is Move ? 10 : raycastTarget.GetComponent<ThisCard>().id,
                    PhotonNetwork.IsMasterClient);

                var targetPV = raycastTarget.GetComponent<PhotonView>().ViewID;
                PhotonNetwork.Destroy(PhotonView.Find(targetPV));

                // var card = (PhotonNetwork.IsMasterClient
                //     ? CardManager.Instance.hostCards
                //     : CardManager.Instance.guestCards).Remove((PhotonNetwork.IsMasterClient
                //     ? CardManager.Instance.hostCards
                //     : CardManager.Instance.guestCards).Find(x => x.id == rangeTarget.GetComponent<ThisCard>().id));

                // Destroy(raycastTarget.gameObject);
            }

            raycastTarget = null;
        }
    }
}