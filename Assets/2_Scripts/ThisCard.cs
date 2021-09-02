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
    [Header("카드 아이디 입력")]
    [SerializeField] int cardId;

    [Header("카드 정보")]
    public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
    public ActType actType;
    public TargetType targetType;
    public int power;
    [TextArea(5, 10)] public string cardDesc;
    public Sprite cardImage;

    [Header("카드 요소")]
    [SerializeField] Text nameText;
    [SerializeField] Text costText;
    [SerializeField] Text powerText;
    [SerializeField] Text DescText;
    [SerializeField] Image CardImage;

    public PhotonView PV;
    public PRS originRPS;

    GameObject target = null;

    void Start()
    {
        PV = this.PV();

        if (PV.IsMine)
        {
            CardFront(true);
        }

        else
        {
            CardFront(false);
        }
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && PV.IsMine)
        {
            CastRay();
        }

        if (target == gameObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);
            }

            if (Input.GetMouseButton(0))
            {
            }

            if (Input.GetMouseButtonUp(0))
            {
                CardManager.Instance.CardAlignment(PhotonNetwork.IsMasterClient);
                target = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void CastRay()
    {
        target = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null && hit.collider.gameObject.tag.Contains("Range"))
        {
            target = hit.collider.gameObject;
        }
    }

    public void Setup(int _id, bool isFront)
    {
        cardId = _id;

        id = CardData.CardList[cardId].id;
        cardName = CardData.CardList[cardId].cardName;
        cost = CardData.CardList[cardId].cost;
        cardType = CardData.CardList[cardId].cardType;
        actType = CardData.CardList[cardId].actType;
        targetType = CardData.CardList[cardId].targetType;
        power = CardData.CardList[cardId].power;
        cardDesc = CardData.CardList[cardId].cardDesc;
        cardImage = CardData.CardList[cardId].cardImage;

        nameText.text = cardName;
        costText.text = cost.ToString();
        powerText.text = power.ToString();
        DescText.text = cardDesc;
        CardImage.sprite = cardImage;

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
}
