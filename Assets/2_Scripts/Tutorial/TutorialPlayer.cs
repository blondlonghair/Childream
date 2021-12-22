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

public class TutorialPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject nextPosArrow;

    [Header("플레이어 스탯")] public float MaxHp;
    public float CurHp;
    public float MaxMp;
    public float CurMp;
    public int MaxMoveCount;
    public int CurMoveCount;
    public bool DefMagic;
    public bool DefElectricity;
    public bool DefExplosion;

    [Header("플레이어 부가 스탯")] public int CurState;
    public bool IsLocked;
    public bool isPlayerTurn;

    private GameObject raycastTarget = null;
    private GameObject rangeTarget = null;
    private GameObject player = null;
    private PhotonView PV;
    private Animator animator;

    private Vector3 worldMousePos;

    void Start()
    {
        PV = this.PV();

        TryGetComponent(out animator);
        PlayerSetup();
    }

    void Update()
    {
        GetMousePos();

        CardZoom();

        // if (isPlayerTurn)
        {
            MouseInput();
            PlayerMove();
        }
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

                if (hit.collider.transform.parent.name == "GuestRange")
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
    }

    private GameObject nextPos;
    private int nextRange;

    void PlayerMove()
    {
        if (CurMoveCount <= 0 || !isPlayerTurn) return;

        if (Input.GetMouseButtonDown(0))
        {
            player = CastRay("Player");

            if (player is null) return;
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
                    (float) (CurState switch {4 => -3.5, 5 => 0, 6 => 3.5, _ => 0}), -1f, 0);
            }

            else
            {
                player.transform.position = new Vector3(
                    (float) (range switch {4 => -3.5, 5 => 0, 6 => 3.5, _ => 0}), -1f, 0);
                TutorialGameManager.Instance.AddBattleList(range, 10);
                CurMoveCount--;
            }
        }
    }

    TutorialThisCard cardInfo = null;

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            raycastTarget = CastRay("Card");

            if (raycastTarget == null) return;
            cardInfo = raycastTarget.GetComponent<TutorialThisCard>();

            cardInfo.CardStopCoroutine();
            raycastTarget.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (Input.GetMouseButton(0))
        {
            if (raycastTarget == null) return;

            if (CheckCastRay("EffectRange"))
            {
                cardInfo.ChangetoEffect(true);
            }

            if (CastRayRange().Item1 == null)
            {
                raycastTarget.transform.position = worldMousePos;
                raycastTarget.transform.localScale = Vector3.one;
                // isLerping = false;
            }

            else
            {
                CardLerp();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            TutorialCardManager.Instance.CardAlignment();

            if (raycastTarget == null) return;
            raycastTarget.GetComponent<TutorialThisCard>().ChangetoEffect(false);
            cardInfo.EffectScale(1);

            if (CastRayRange().Item1 == null || CurMp < cardInfo.cost) return;

            switch (cardInfo.targetType)
            {
                case TargetType.All:
                    TutorialGameManager.Instance.AddBattleList(CastRayRange().Item2,
                        raycastTarget is Move ? 10 : cardInfo.id);
                    TutorialCardManager.Instance.DestroyCard(raycastTarget, false);
                    TutorialCardManager.Instance.CardAlignment();
                    CurMp -= cardInfo.cost;
                    break;

                case TargetType.EnemyAll:
                    if (CastRayRange().Item2 < 4) goto case TargetType.All;
                    break;
                case TargetType.EnemySellect:
                    if (CastRayRange().Item2 < 4) goto case TargetType.All;
                    break;
                case TargetType.MeAll:
                    if (CastRayRange().Item2 > 3) goto case TargetType.All;
                    break;
                case TargetType.MeSellect:
                    if (CastRayRange().Item2 > 3) goto case TargetType.All;
                    break;
                case TargetType.None: break;
            }

            isLerping = false;
            raycastTarget = null;
        }
    }

    private GameObject card = null;
    private GameObject card2 = null;
    private TutorialThisCard thisCard;
    private bool isLerping;

    private enum CardIn
    {
        Enter,
        Exit,
        On
    }

    private CardIn cardIn = CardIn.Exit;

    void CardZoom()
    {
        card = CastRay("Card");

        if (card != null && !Input.GetMouseButton(0) && card2 == card)
        {
            card.TryGetComponent(out thisCard);
            if (cardIn == CardIn.Exit)
            {
                thisCard.CardZoomIn();
                cardIn = CardIn.On;
            }
        }

        if (card2 != card && !isLerping)
        {
            if (thisCard != null && cardIn == CardIn.On)
            {
                thisCard.CardZoomOut();
                cardIn = CardIn.Exit;
            }

            card2 = card;
        }
    }

    void CardLerp()
    {
        switch (cardInfo.targetType)
        {
            case TargetType.All:
                cardInfo.EffectScale(3);
                raycastTarget.transform.position =
                    Vector3.Lerp(raycastTarget.transform.position, new Vector3(0, 0, 0), 0.2f);
                isLerping = true;
                break;

            case TargetType.EnemyAll:
                if (CastRayRange().Item2 < 4)
                {
                    cardInfo.EffectScale(2);
                    raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                        new Vector3(0, CastRayRange().Item1.transform.position.y, 0), 0.2f);
                    isLerping = true;
                }
                else goto case TargetType.None;

                break;

            case TargetType.EnemySellect:
                if (CastRayRange().Item2 < 4)
                {
                    raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                        CastRayRange().Item1.transform.position, 0.2f);
                    isLerping = true;
                }
                else goto case TargetType.None;

                break;

            case TargetType.MeAll:
                if (CastRayRange().Item2 > 3)
                {
                    cardInfo.EffectScale(2);
                    raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                        new Vector3(0, CastRayRange().Item1.transform.position.y, 0), 0.2f);
                    isLerping = true;
                }
                else goto case TargetType.None;

                break;

            case TargetType.MeSellect:
                if (CastRayRange().Item2 > 3)
                {
                    raycastTarget.transform.position = Vector3.Lerp(raycastTarget.transform.position,
                        CastRayRange().Item1.transform.position, 0.2f);
                    isLerping = true;
                }
                else goto case TargetType.None;

                break;

            case TargetType.None:
                raycastTarget.transform.position = worldMousePos;
                raycastTarget.transform.localScale = Vector3.one;
                isLerping = false;
                break;
        }
    }

    public void SetAnimation(string animation)
    {
        animator.SetTrigger(animation);
    }
}