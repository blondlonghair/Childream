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
    public bool DefMagic;
    public bool DefElectricity;
    public bool DefExplosion;

    public int CurState;
    public bool IsLocked = false;
    public int SelectRange;

    GameObject raycastTarget = null;
    GameObject rangeTarget = null;
    PhotonView PV;

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

    void CastRay(ref GameObject obj, string tag)
    {
        obj = null;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit2D in hits)
        {
            if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag(tag))
            {
                obj = hit2D.collider.gameObject;
                return;
            }
        }
    }

    GameObject CastRay(string tag)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit2D in hits)
        {
            if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag(tag))
            {
                return hit2D.collider.gameObject;
            }
        }

        return null;
    }

    bool CheckCastRay(string tag)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false;
        
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit2D in hits)
        {
            if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }

    void CastRayRange(ref GameObject obj)
    {
        obj = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.tag.Contains("Range") &&
                !hits[i].collider.gameObject.CompareTag("EffectRange"))
            {
                SelectRange = int.Parse(hits[i].collider.gameObject.tag.Replace("Range", ""));
                obj = hits[i].collider.gameObject;
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
                transform.position = new Vector3(0, 2.5f, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "HostPlayer";
            }
            else
            {
                transform.position = new Vector3(0, -5, 0);
                transform.Rotate(0, 0, 180);
                gameObject.name = "GuestPlayer";
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -2.5f, 0);
                gameObject.name = "GuestPlayer";
            }
            else
            {
                transform.position = new Vector3(0, 5, 0);
                gameObject.name = "HostPlayer";
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
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
            CastRay(ref raycastTarget, "Card");

            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<PhotonView>().IsMine)
            {
                raycastTarget.transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null)
                return;

            if (CheckCastRay("EffectRange"))
            {
                raycastTarget.GetComponent<ThisCard>().ChangetoEffect(true);
            }
            
            if (raycastTarget.GetComponent<PhotonView>().IsMine)
            {
                raycastTarget.transform.position = worldMousePos;
                raycastTarget.transform.localScale = Vector3.one;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);

            CastRayRange(ref rangeTarget);
            
            raycastTarget.GetComponent<ThisCard>().ChangetoEffect(false);

            if (rangeTarget == null)
                return;
            
            if (!rangeTarget.GetComponent<PhotonView>().IsMine)
            {
                //마나 0보다 작으면 return
                if (CurMp < raycastTarget.GetComponent<ThisCard>().cost)
                    return;

                //카드 코스트만큼 마나 제거
                CurMp -= raycastTarget.GetComponent<ThisCard>().cost;

                //카드 리스트에 행동추가
                GameManager.Instance.AddBattleList(SelectRange,
                    raycastTarget is Move ? 10 : raycastTarget.GetComponent<ThisCard>().id,
                    PhotonNetwork.IsMasterClient);

                //다 하고 나면 카드 삭제
                CardManager.Instance.DestroyCard(raycastTarget, PhotonNetwork.IsMasterClient);
            }

            //카드 정렬
            CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);

            raycastTarget = null;
        }
    }
    
    void CardZoom()
    {
        GameObject card = CastRay("Card");

        if (card == null) return;
        
        Canvas canvas = card.GetComponentInChildren<Canvas>();
        PRS originRPS = card.GetComponent<ThisCard>().originRPS;
    
        if (!Input.GetMouseButton(0))
        {
            card.transform.localScale = Vector3.Lerp(card.transform.localScale, new Vector3(2, 2, 2), 0.5f);
            canvas.sortingOrder = 100;
    
            if (PhotonNetwork.IsMasterClient)
            {
                card.transform.position = Vector3.Lerp(card.transform.position, new Vector3(card.transform.position.x, 7, -9), 0.5f);
                card.transform.rotation = Quaternion.Lerp(card.transform.rotation, Quaternion.Euler(0, 0, 180), 0.5f);
            }
            else
            {
                card.transform.position = Vector3.Lerp(card.transform.position, new Vector3(card.transform.position.x, -7, -9), 0.5f);
                card.transform.rotation = Quaternion.Lerp(card.transform.rotation, Quaternion.Euler(0, 0, 0), 0.5f);
            }
        }
    }
}