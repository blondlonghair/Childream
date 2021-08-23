using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class ThisCard : MonoBehaviourPunCallbacks
{
    [Header("카드 아이디 입력")]
    [SerializeField] int cardId;

    [Header("카드 정보")]
    public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
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

    PhotonView PV;
    public PRS originRPS;

    GameObject target = null;
    bool isFlip = false;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(0, 0, 180);
        }

        if (!PV.IsMine)
        {
            CardFlip();
        }
    }

    public static Vector3 worldMousePos;

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;

        if (Input.GetMouseButtonDown(0) && PV.IsMine)
        {
            CastRay();
        }

        if (target == gameObject)
        {
            if (targetType == TargetType.ALL)
            {
                target.transform.position = worldMousePos;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //gameObject.transform.parent = null;
            }

            if (Input.GetMouseButton(0))
            {
                //transform.position = new Vector3(transform.position.x, transform.position.y, 1);
            }

            if (Input.GetMouseButtonUp(0))
            {
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

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Card"))
        {
            target = hit.collider.gameObject;
        }
    }

    public void Setup(int _id, bool isFront)
    {
        id = CardData.CardList[_id].id;
        cardName = CardData.CardList[_id].cardName;
        cost = CardData.CardList[_id].cost;
        cardType = CardData.CardList[_id].cardType;
        targetType = CardData.CardList[_id].targetType;
        power = CardData.CardList[_id].power;
        cardDesc = CardData.CardList[_id].cardDesc;
        cardImage = CardData.CardList[_id].cardImage;

        nameText.text = cardName;
        costText.text = cost.ToString();
        powerText.text = power.ToString();
        DescText.text = cardDesc;
        CardImage.sprite = cardImage;

        CardFlip(isFront);
    }

    void CardFlip(bool isFront)
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

    void CardFlip()
    {
        if (isFlip)
        {
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            isFlip = false;
        }

        else
        {
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            isFlip = true;
        }
    }

    public void MoveTransform(PRS prs)
    {
        transform.position = prs.pos;
        transform.rotation = prs.rot;
        transform.localScale = prs.scale;
    }
}
