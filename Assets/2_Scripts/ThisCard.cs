using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Utils;
using Enums;
using Unity.Mathematics;

public class ThisCard : MonoBehaviourPunCallbacks
{
    [Header("카드 아이디 입력")] [SerializeField] int cardId;
    [Header("카드 정보")] public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
    public TargetType targetType;
    public int power;
    [TextArea(5, 10)] public string cardDesc;
    public Sprite cardImage;
    public Sprite cardImageBG;

    [Header("카드 요소")] [SerializeField] Text nameText;
    [SerializeField] Text costText;
    [SerializeField] Text powerText;
    [SerializeField] Text DescText;
    [SerializeField] Image CardImage;
    [SerializeField] Image CardImageBG;

    public PhotonView PV;
    public PRS originRPS;

    GameObject target = null;

    private void Awake()
    {
        PV = this.PV();

        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
                CardManager.Instance.hostCards.Add(this);
            else
                CardManager.Instance.guestCards.Add(this);
        }

        else
        {
            if (PhotonNetwork.IsMasterClient)
                CardManager.Instance.guestCards.Add(this);
            else
                CardManager.Instance.hostCards.Add(this);
        }

        CardManager.Instance.CardAlignment(PV.IsMine);
        CardManager.Instance.CardAlignment(!PV.IsMine);
    }

    void Start()
    {
        if (PV.IsMine)
            CardFront(true);
        else
            CardFront(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (PV.IsMine)
            {
                if (PhotonNetwork.IsMasterClient)
                    CardManager.Instance.hostCards.Remove(this);
                else
                    CardManager.Instance.guestCards.Remove(this);

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        CastRay();

        CardZoom();
    }

    void CardZoom()
    {
        if (!PV.IsMine || EventSystem.current.IsPointerOverGameObject())
            return;

        if (target == gameObject && !Input.GetMouseButton(0))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2, 2, 2), 0.5f);

            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 7, -9), 0.5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 180), 0.5f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -7, -9), 0.5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.5f);
            }
        }

        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.5f);
            transform.position = Vector3.Lerp(transform.position, originRPS.pos, 0.5f);
            if (!Input.GetMouseButton(0))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, originRPS.rot, 0.5f);
            }
        }
    }

    void CastRay()
    {
        target = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, Vector2.zero, 0f);

        foreach (var hit2D in hit)
        {
            if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag("Card"))
            {
                target = hit2D.collider.gameObject;
                return;
            }
        }
    }

    public void Setup(int _id, bool isFront)
    {
        cardId = _id;

        id = CardData.CardList[cardId].id;
        cardName = CardData.CardList[cardId].cardName;
        cost = CardData.CardList[cardId].cost;
        cardType = CardData.CardList[cardId].cardType;
        targetType = CardData.CardList[cardId].targetType;
        cardDesc = CardData.CardList[cardId].cardDesc;
        cardImage = CardData.CardList[cardId].cardImage;
        cardImageBG = CardData.CardList[cardId].cardImageBG;

        nameText.text = cardName;
        costText.text = cost.ToString();
        powerText.text = power.ToString();
        DescText.text = cardDesc;
        CardImage.sprite = cardImage;
        CardImageBG.sprite = cardImageBG;

        CardFront(isFront);
    }

    void CardFront(bool isFront)
    {
        if (isFront)
        {
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }

        else
        {
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    public void MoveTransform(PRS prs)
    {
        transform.position = prs.pos;
        transform.rotation = prs.rot;
        transform.localScale = prs.scale;
    }

    private void OnDestroy()
    {
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
                CardManager.Instance.hostCards.Remove(this);
            else
                CardManager.Instance.guestCards.Remove(this);
        }

        else
        {
            if (PhotonNetwork.IsMasterClient)
                CardManager.Instance.guestCards.Remove(this);
            else
                CardManager.Instance.hostCards.Remove(this);
        }

        CardManager.Instance.CardAlignment(PV.IsMine);
        CardManager.Instance.CardAlignment(!PV.IsMine);
    }

    // public void OnPhotonInstantiate(PhotonMessageInfo info)
    // {
    //     PV = this.PV();
    //     if (PV.IsMine)
    //     {
    //         CardFront(true);
    //         CardManager.Instance.CardAlignment(PV.IsMine);
    //     }
    //
    //     else
    //     {
    //         CardFront(false);
    //         CardManager.Instance.CardAlignment(PV.IsMine);
    //     }
    // }
}