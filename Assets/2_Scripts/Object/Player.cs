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
    [Header("플레이어 스탯")] public float MaxHp;
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

    public bool isPlayerTurn = false;

    GameObject raycastTarget = null;
    GameObject rangeTarget = null;
    GameObject player = null;
    PhotonView PV;

    private Vector3 worldMousePos;

    void Start()
    {
        PV = this.PV();

        PlayerSetup();
    }

    void Update()
    {
        if (PV.IsMine && Input.GetKeyDown(KeyCode.F1))
        {
            PV.RPC(nameof(DieInput), RpcTarget.AllBuffered);
        }

        GetMousePos();

        if (!PV.IsMine) return;

        CardZoom();

        if (isPlayerTurn)
        {
            MouseInput();
            PlayerMove();
        }
    }

    [PunRPC]
    private void DieInput()
    {
        CurHp = 0;
    }

    void GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;
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

    (GameObject, int) CastRayRange()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);
        int range = 0;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.tag.Contains("Range") && !hit.collider.gameObject.CompareTag("EffectRange"))
            {
                range = int.Parse(hit.collider.gameObject.tag.Replace("Range", ""));

                if (hit.collider.gameObject.GetPhotonView().IsMine)
                {
                    range += 3;
                }

                return (hit.collider.gameObject, range);
            }
        }

        return (null, range);
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
                transform.position = new Vector3(0, 1f, 0);
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
                transform.position = new Vector3(0, -1, 0);
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

            if (player == null || !player.GetPhotonView().IsMine) return;
        }

        if (Input.GetMouseButton(0))
        {
            if (player is null) return;

            player.transform.position = worldMousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (player is null) return;

            int range = CastRayRange().Item2;

            if (range == 0)
            {
                player.transform.position = new Vector3(
                    PhotonNetwork.IsMasterClient
                        ? (float) (CurState switch {4 => 3.5, 5 => 0, 6 => -3.5, _ => 0})
                        : (float) (CurState switch {4 => -3.5, 5 => 0, 6 => 3.5, _ => 0}),
                    PhotonNetwork.IsMasterClient ? player.GetPhotonView().IsMine ? 1f : -5f :
                    player.GetPhotonView().IsMine ? -1f : 5f, 0);
            }

            else
            {
                player.transform.position = new Vector3(
                    PhotonNetwork.IsMasterClient
                        ? (float) (range switch {4 => 3.5, 5 => 0, 6 => -3.5, _ => 0})
                        : (float) (range switch {4 => -3.5, 5 => 0, 6 => 3.5, _ => 0}),
                    PhotonNetwork.IsMasterClient ? player.GetPhotonView().IsMine ? 1f : -5f :
                    player.GetPhotonView().IsMine ? -1f : 5f, 0);
                GameManager.Instance.AddBattleList(range, 10, PhotonNetwork.IsMasterClient);
                CurMoveCount--;
            }
        }
    }

    ThisCard cardInfo = null;

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            raycastTarget = CastRay("Card");

            if (raycastTarget == null) return;
            cardInfo = raycastTarget.GetComponent<ThisCard>();

            if (raycastTarget.GetPhotonView().IsMine)
            {
                raycastTarget.transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null) return;

            if (CheckCastRay("EffectRange"))
            {
                cardInfo.ChangetoEffect(true);
            }

            if (raycastTarget.GetPhotonView().IsMine)
            {
                if (CastRayRange().Item1 == null)
                {
                    raycastTarget.transform.position = worldMousePos;
                    raycastTarget.transform.localScale = Vector3.one;
                    isLerping = false;
                }

                else
                {
                    // switch (cardInfo.targetType)
                    // {
                    //     case TargetType.All:
                    //         cardInfo.EffectScale(3);
                    //         raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position, new Vector3(0, 0, 0), 0.2f);
                    //         isLerping = true;
                    //         break;
                    //     case TargetType.EnemyAll:
                    //         if (CastRayRange().Item2 < 4)
                    //         {
                    //             cardInfo.EffectScale(2);
                    //             raycastTarget.transform.position =
                    //                 new Vector3(0, CastRayRange().Item1.transform.position.y, 0);
                    //             isLerping = true;
                    //         }
                    //
                    //         break;
                    //     case TargetType.EnemySellect:
                    //         if (CastRayRange().Item2 < 4)
                    //         {
                    //             raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                    //                 CastRayRange().Item1.transform.position, 0.2f);
                    //             isLerping = true;
                    //         }
                    //
                    //         break;
                    //     case TargetType.MeAll:
                    //         if (CastRayRange().Item2 > 3)
                    //         {
                    //             cardInfo.EffectScale(2);
                    //             raycastTarget.transform.position =
                    //                 new Vector3(0, CastRayRange().Item1.transform.position.y, 0);
                    //             isLerping = true;
                    //         }
                    //
                    //         break;
                    //     case TargetType.MeSellect:
                    //         if (CastRayRange().Item2 > 3)
                    //         {
                    //             raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                    //                 CastRayRange().Item1.transform.position, 0.2f);
                    //             isLerping = true;
                    //         }
                    //
                    //         break;
                    //     case TargetType.None:
                    //         break;
                    // }
                    
                    raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                        CastRayRange().Item1.transform.position, 0.2f);
                    isLerping = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);
            
            if (raycastTarget == null) return;
            raycastTarget.GetComponent<ThisCard>().ChangetoEffect(false);
            cardInfo.EffectScale(1);

            if (CastRayRange().Item1 == null || CurMp < cardInfo.cost) return;

            CurMp -= cardInfo.cost;

            GameManager.Instance.AddBattleList(CastRayRange().Item2,
                raycastTarget is Move ? 10 : cardInfo.id, PhotonNetwork.IsMasterClient);

            CardManager.Instance.DestroyCard(raycastTarget, PhotonNetwork.IsMasterClient);

            CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);

            isLerping = false;
            raycastTarget = null;
        }
    }

    private GameObject card = null;
    private GameObject card2 = null;
    private ThisCard thisCard;
    private bool isLerping;
    
    void CardZoom()
    {
        card = CastRay("Card");

        if (card != null && !Input.GetMouseButton(0) && card2 == card)
        {
            card.TryGetComponent(out thisCard);

            if (thisCard.TryGetComponent(out PhotonView c))
            {
                if (c.IsMine)
                {
                    thisCard.CardZoomIn();
                }
            }
        }
        
        if (card2 != card && !isLerping)
        {
            if (thisCard != null && thisCard.gameObject.GetPhotonView().IsMine)
            {
                thisCard.CardZoomOut();
            }
            
            card2 = card;
        }
    }
}