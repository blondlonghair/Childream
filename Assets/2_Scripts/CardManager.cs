using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class CardManager : MonoBehaviourPunCallbacks
{
    public List<ThisCard> hostCards;
    public List<ThisCard> guestCards;

    [SerializeField] Transform hostCardLeft;
    [SerializeField] Transform hostCardRight;
    [SerializeField] Transform guestCardLeft;
    [SerializeField] Transform guestCardRight;

    public static CardManager Instance;
    private PhotonView PV;

    List<Card> cardBuffer = new List<Card>();

    private void Awake() => Instance = this;

    private void Start()
    {
        PV = this.PV();
        SetupCard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddCard(PhotonNetwork.IsMasterClient);
        
        // print($"{cardBuffer[0].id}{cardBuffer[1].id}{cardBuffer[2].id}{cardBuffer[3].id}{cardBuffer[4].id}{cardBuffer[5].id}{cardBuffer[6].id}{cardBuffer[7].id}{cardBuffer[8].id}");
    }

    public void AddCard(bool isMine)
    {
        _AddCard(isMine);
        // PV.RPC(nameof(_AddCard), RpcTarget.AllBuffered, isMine);
    }

    // [PunRPC]
    public void _AddCard(bool isMine)
    {
        var cardObject = PhotonNetwork.Instantiate("Prefab/Card", Vector3.zero, Quaternion.identity);
        var card = cardObject.GetComponent<ThisCard>();
        card.Setup(PopCard(), isMine);
        // (isMine ? hostCards : guestCards).Add(card);

        CardAlignment(isMine);
    }

    // [PunRPC]
    // public void AddList(bool isMine, ThisCard card)
    // {
    //     if (isMine)
    //         hostCards.Add(card);
    //     else
    //         guestCards.Add(card);
    // }

    public int PopCard()
    {
        if (cardBuffer.Count == 0)
        {
            SetupCard();
        }

        int temp = cardBuffer[0].id;
        cardBuffer.RemoveAt(0);
        print("PopIndex" + temp);
        return temp;
    }

    void SetupCard()
    {
        // print(CardData.CardList.Count + "CardListCount");
        // print($"CardDataList Id {CardData.CardList[0].id}{CardData.CardList[1].id}{CardData.CardList[2].id}{CardData.CardList[3].id}{CardData.CardList[4].id}{CardData.CardList[5].id}{CardData.CardList[6].id}{CardData.CardList[7].id}{CardData.CardList[8].id}");
        for (int i = 1; i < CardData.CardList.Count; i++)
        {
            // print(i);
            Card card = CardData.CardList[i];
            cardBuffer.Add(card);
        }

        for (int i = 0; i < cardBuffer.Count; i++)
        {
            int rand = Random.Range(i, cardBuffer.Count);
            // print($"RandomAddCard {rand}");
            (cardBuffer[i], cardBuffer[rand]) = (cardBuffer[rand], cardBuffer[i]);
        }
    }

    public void CardAlignment(bool isMine)
    {
        _CardAlignment(isMine);
        // PV.RPC(nameof(_CardAlignment), RpcTarget.AllBuffered, isMine);
    }

    // [PunRPC]
    public void _CardAlignment(bool isMine)
    {
        var originCardRPS = new List<PRS>();
        originCardRPS = isMine
            ? RondAlignment(hostCardLeft, hostCardRight, hostCards.Count, 0.5f, Vector3.one, isMine)
            : RondAlignment(guestCardLeft, guestCardRight, guestCards.Count, -0.5f, Vector3.one, isMine);

        var targetCards = isMine ? hostCards : guestCards;
        for (var i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originRPS = originCardRPS[i];
            targetCard.MoveTransform(targetCard.originRPS);
        }
    }

    List<PRS> RondAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale, bool isMine)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1:
                objLerps = new float[] {0.5f};
                break;
            case 2:
                objLerps = new float[] {0.27f, 0.73f};
                break;
            case 3:
                objLerps = new float[] {0.1f, 0.5f, 0.9f};
                break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                {
                    objLerps[i] = interval * i;
                }

                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            Quaternion targetRot = Quaternion.identity;

            if (isMine && objCount < 4)
            {
                targetRot = Quaternion.Euler(0, 0, targetRot.eulerAngles.z + 180);
            }

            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? -curve : curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }

            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }
}