using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using Utils;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviourPunCallbacks
{
    public List<ThisCard> hostCards;
    public List<ThisCard> guestCards;

    [Header("카드 덱 위치")] 
    [SerializeField] Transform hostCardLeft;
    [SerializeField] Transform hostCardRight;
    [SerializeField] Transform guestCardLeft;
    [SerializeField] Transform guestCardRight;

    [Header("카드 드로우 위치")] 
    [SerializeField] private Transform hostCardDraw;
    [SerializeField] private Transform guestCardDraw;

    [Header("카드 효과발동 위치")] 
    [SerializeField] private Transform hostCardActive;
    [SerializeField] private Transform guestCardActive;
    [SerializeField] private GameObject resultCard;

    [Header("max카드 수")] 
    [SerializeField] private int maxCard = 4;

    public static CardManager Instance;
    private PhotonView PV;

    private List<Card> cardBuffer = new List<Card>();

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
    }

    //카드 뽑기
    public void AddCard(bool isMine)
    {
        if (!isMine) return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (hostCards.Count >= maxCard) return;
        }
        else
        {
            if (guestCards.Count >= maxCard) return;
        }

        Vector3 initPos;
        Quaternion initRot;

        if (PhotonNetwork.IsMasterClient)
        {
            initPos = hostCardDraw.position;
            initRot = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            initPos = guestCardDraw.position;
            initRot = Quaternion.Euler(0, 0, 0);
        }

        var cardObject = PhotonNetwork.Instantiate("Prefab/Card", initPos, initRot);
        var card = cardObject.GetComponent<ThisCard>();
        card.Setup(PopCard(), isMine);

        CardAlignment(isMine);
    }

    //카드 없에기
    public void DestroyCard(GameObject card, bool isMaster)
    {
        print("DestroyCard");
        int index;

        index = isMaster
            ? hostCards.FindIndex(x => x == card.GetComponent<ThisCard>())
            : guestCards.FindIndex(x => x == card.GetComponent<ThisCard>());

        PV.RPC(nameof(_DestroyCard), RpcTarget.AllBuffered, index, isMaster);

        if (isMaster)
        {
            var targetPV = card.GetComponent<PhotonView>().ViewID;
            PhotonNetwork.Destroy(PhotonView.Find(targetPV));
        }
        else
        {
            var targetPV = card.GetComponent<PhotonView>().ViewID;
            PhotonNetwork.Destroy(PhotonView.Find(targetPV));
        }
    }

    [PunRPC]
    void _DestroyCard(int index, bool isMaster)
    {
        if (isMaster)
        {
            hostCards.RemoveAt(index);
        }

        else
        {
            guestCards.RemoveAt(index);
        }

        CardAlignment(isMaster);
        CardAlignment(!isMaster);
    }

    public int PopCard()
    {
        if (cardBuffer.Count <= 0)
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
            targetCard.MoveTransform();
        }
    }

    // [PunRPC]
    // public void sync_CardAlignment(bool isMine)
    // {
    //     var originCardRPS = new List<PRS>();
    //     originCardRPS = isMine
    //         ? RondAlignment(hostCardLeft, hostCardRight, hostCards.Count, 0.5f, Vector3.one, isMine)
    //         : RondAlignment(guestCardLeft, guestCardRight, guestCards.Count, -0.5f, Vector3.one, isMine);
    //
    //     var targetCards = isMine ? hostCards : guestCards;
    //     for (var i = 0; i < targetCards.Count; i++)
    //     {
    //         var targetCard = targetCards[i];
    //
    //         targetCard.originRPS = originCardRPS[i];
    //         targetCard.MoveTransform();
    //     }
    // }

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

            results.Add(new PRS(targetPos, targetRot, scale, i));
        }

        return results;
    }

    public void ShowWatIUsed(Player _caster, Player _target, int cardId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_caster.GetComponent<PhotonView>().IsMine)
            {
                
            }
        }
        
        print(_caster.GetComponent<PhotonView>().IsMine);
        
        GameObject card = Instantiate(resultCard, new Vector3(0,0,0),
            PhotonNetwork.IsMasterClient ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0),
        PhotonNetwork.IsMasterClient ? _caster.GetComponent<PhotonView>().IsMine ? hostCardActive : guestCardActive :_caster.GetComponent<PhotonView>().IsMine ? guestCardActive : hostCardActive);

        if (card.TryGetComponent(out ShowResultCard result))
        {
            result.cardId = cardId;
        }
    }
}