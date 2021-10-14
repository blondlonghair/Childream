using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        print($"hostcard : {hostCards.Count}, guestcard : {guestCards.Count}");

        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddCard(PhotonNetwork.IsMasterClient);
    }

    //카드 뽑기
    public void AddCard(bool isMine)
    {
        if (isMine)
        {
            // print("카드 생성");
            var cardObject = PhotonNetwork.Instantiate("Prefab/Card", Vector3.zero, Quaternion.identity);
            var card = cardObject.GetComponent<ThisCard>();
            card.Setup(PopCard(), isMine);
        }

        CardAlignment(isMine);
    }

    //카드 없에기
    public void DestroyCard(GameObject card, bool isMaster)
    {
        print("DestroyCard");
        int index;

        if (isMaster)
        {
            index = hostCards.FindIndex(x => x == card.GetComponent<ThisCard>());
        }

        else
        {
            index = guestCards.FindIndex(x => x == card.GetComponent<ThisCard>());
        }

        print($"index : {index}");

        PV.RPC(nameof(_DestroyCard), RpcTarget.AllBuffered, index, isMaster);

        if (isMaster)
        {
            var targetPV = card.GetComponent<PhotonView>().ViewID;
            PhotonNetwork.Destroy(PhotonView.Find(targetPV));
            print($"ismine : {isMaster}");
        }
        else
        {
            var targetPV = card.GetComponent<PhotonView>().ViewID;
            PhotonNetwork.Destroy(PhotonView.Find(targetPV));
            print($"ismine : {isMaster}");
        }
    }

    [PunRPC]
    void _DestroyCard(int index, bool isMaster)
    {
        if (isMaster)
        {
            print("_DestroyCard host");
            hostCards.RemoveAt(index);
        }

        else
        {
            print("_DestroyCard host");
            guestCards.RemoveAt(index);
        }

        CardAlignment(isMaster);
        CardAlignment(!isMaster);
        // PV.RPC(nameof(sync_CardAlignment), RpcTarget.AllBuffered, PV.IsMine);
        // PV.RPC(nameof(sync_CardAlignment), RpcTarget.AllBuffered, !PV.IsMine);
    }

    public int PopCard()
    {
        if (cardBuffer.Count == 0)
        {
            SetupCard();
        }

        int temp = cardBuffer[0].id;
        cardBuffer.RemoveAt(0);
        return temp;
    }

    //카드 뽑을때 카드 9장 랜덤 셔플
    void SetupCard()
    {
        for (int i = 1; i < CardData.CardList.Count - 1; i++)
        {
            Card card = CardData.CardList[i];
            cardBuffer.Add(card);
        }

        for (int i = 0; i < cardBuffer.Count - 1; i++)
        {
            int rand = Random.Range(i, cardBuffer.Count);
            (cardBuffer[i], cardBuffer[rand]) = (cardBuffer[rand], cardBuffer[i]);
        }
    }

    //카드 ismine확인 후 정렬하는 함수
    public void CardAlignment(bool isMine)
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

    [PunRPC]
    public void sync_CardAlignment(bool isMine)
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

    //카드 정렬하는 함수
    List<PRS> RondAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale, bool isMine)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        //카드 로테이션
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

        //카드 위치
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

            //카드 z값 변경
            targetPos.z = -i + 10;

            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }
}