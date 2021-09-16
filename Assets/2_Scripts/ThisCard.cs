using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Utils;
using Enums;

public class ThisCard : MonoBehaviourPunCallbacks
{
    [Header("카드 아이디 입력")] [SerializeField] int cardId;
    [Header("카드 정보")] public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
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

    void CastRay()
    {
        target = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Card"))
        {
            target = hit.collider.gameObject;
        }
    }

    public void Setup(int _id, bool isFront)
    {
        cardId = _id;

        if (CardData.CardList[cardId] is Card)
        {
            id = ((Card)CardData.CardList[cardId]).id;
            cardName = ((Card)CardData.CardList[cardId]).cardName;
            cost = ((Card)CardData.CardList[cardId]).cost;
            cardType = ((Card)CardData.CardList[cardId]).cardType;
            cardDesc = ((Card)CardData.CardList[cardId]).cardDesc;
            cardImage = ((Card)CardData.CardList[cardId]).cardImage;
            cardImageBG = ((Card) CardData.CardList[cardId]).cardImageBG;
        }

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