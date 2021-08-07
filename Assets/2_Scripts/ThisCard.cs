using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class ThisCard : MonoBehaviourPunCallbacks
{
    [Header("카드 아이디 입력")]
    [SerializeField] int cardId;

    [Header("카드 정보")]
    public int id;
    public string cardName;
    public int cost;
    public AtkType atkType;
    public MoveType moveType;
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

    GameObject target = null;

    void Start()
    {
        id = CardData.CardList[cardId].id;
        cardName = CardData.CardList[cardId].cardName;
        cost = CardData.CardList[cardId].cost;
        atkType = CardData.CardList[cardId].atkType;
        moveType = CardData.CardList[cardId].moveType;
        power = CardData.CardList[cardId].power;
        cardDesc = CardData.CardList[cardId].cardDesc;
        cardImage = CardData.CardList[cardId].cardImage;

        nameText.text = cardName;
        costText.text = cost.ToString();
        powerText.text = power.ToString();
        DescText.text = cardDesc;
        CardImage.sprite = cardImage;

        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(0, 0, 180);
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
            //if (PV.IsMine)
            target.transform.position = worldMousePos;
            //if (!PV.IsMine)
            //    PV.RPC(nameof(nn), RpcTarget.OthersBuffered);

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
    }

    //[PunRPC]
    //public void nn()
    //{
    //    target.transform.position = -worldMousePos;
    //}

    //[PunRPC]
    //public void Drag()
    //{
    //    if (PV.IsMine)
    //    {
    //        transform.position = mousePosition;
    //    }
    //}

    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    transform.parent = GameObject.Find("MainCanvas").transform;
    //    GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    //}

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
}
