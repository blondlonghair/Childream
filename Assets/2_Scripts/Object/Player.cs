using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using Enums;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    GameObject player = null;
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

    GameObject CastRay(string tag)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return null;
        
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

    int CastRayRangeInt()
    {
        int range = 0;
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.tag.Contains("Range") &&
                !hit.collider.gameObject.CompareTag("EffectRange") && hit.collider.gameObject.GetPhotonView().IsMine)
            {
                range = int.Parse(hit.collider.gameObject.tag.Replace("Range", ""));

                return range;
            }
        }

        return 0;
    }
    
    GameObject CastRayRange()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.tag.Contains("Range") &&
                !hit.collider.gameObject.CompareTag("EffectRange"))
            {
                SelectRange = int.Parse(hit.collider.gameObject.tag.Replace("Range", ""));
                
                if (hit.collider.gameObject.GetPhotonView().IsMine)
                {
                    SelectRange += 3;
                }

                return hit.collider.gameObject;
            }
        }

        return null;
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

    void PlayerMove()
    {
        if (CurMoveCount <= 0) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            player = CastRay("Player");

            if (!player.GetPhotonView().IsMine || player == null) return;
        }

        if (Input.GetMouseButton(0))
        {
            
            player.transform.position = worldMousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            int range = CastRayRangeInt();

            if (range == 0)
            {
                player.transform.position = new Vector3(
                    (PhotonNetwork.IsMasterClient
                        ? (float) (CurState switch {1 => 3.5, 2 => 0, 3 => -3.5, _ => 0})
                        : (float) (CurState switch {1 => -3.5, 2 => 0, 3 => 3.5, _ => 0})),
                    (PhotonNetwork.IsMasterClient ? player.GetPhotonView().IsMine ? 2.5f : -5f :
                        player.GetPhotonView().IsMine ? -2.5f : 5f), 0);
                return;
            }
            
            player.transform.position = new Vector3(PhotonNetwork.IsMasterClient ? 
                    (float)(range switch{1 => 3.5, 2 => 0, 3 => -3.5, _ => 0}) : 
                    (float)(range switch{1 => -3.5, 2 => 0, 3 => 3.5, _ => 0}), 
                (PhotonNetwork.IsMasterClient ? player.GetPhotonView().IsMine ? 2.5f : -5f :
                    player.GetPhotonView().IsMine ? -2.5f : 5f), 0);
            GameManager.Instance.AddBattleList(range, 10, PhotonNetwork.IsMasterClient);
            CurMoveCount--;
        }
    }

    async Task LerpPlayer(Vector3 range)
    {
        await Task.Run(() =>
        {
            while (true)
            {
                transform.position = Vector3.Lerp(transform.position, range, 0.2f);
            }
        });
    }

    void MouseInput()
    {
        //11_08
        // GameObject player = CastRay("Card");
        
        if (Input.GetMouseButtonDown(0))
        {
            raycastTarget = CastRay("Card");

            if (raycastTarget == null)
                return;

            if (raycastTarget.GetComponent<PhotonView>().IsMine)
            {
                raycastTarget.transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null) return;

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

            rangeTarget = CastRayRange();

            if (raycastTarget == null) return;
            
            raycastTarget.GetComponent<ThisCard>().ChangetoEffect(false);

            if (rangeTarget == null) return;
            
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