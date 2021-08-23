using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class CardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<ThisCard> hostCards;
    [SerializeField] List<ThisCard> guestCards;

    [SerializeField] Transform hostCardLeft;
    [SerializeField] Transform hostCardRight;
    [SerializeField] Transform guestCardLeft;
    [SerializeField] Transform guestCardRight;

    public static CardManager Instance;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddCard(PhotonNetwork.IsMasterClient);
    }

    public void AddCard(bool isMine)
    {
        var cardObject = PhotonNetwork.Instantiate("Prefab/Card", Vector3.zero, Quaternion.identity);
        var card = cardObject.GetComponent<ThisCard>();
        card.Setup(1, isMine);
        (isMine ? hostCards : guestCards).Add(card);

        CardAlignment(isMine);
    }

    void SetupCard()
    {

    }

    void CardAlignment(bool isMine)
    {
        List<PRS> originCardRPS = new List<PRS>();
        if (isMine)
        {
            originCardRPS = RondAlignment(hostCardLeft, hostCardRight, hostCards.Count, 0.5f, Vector3.one/* * 1.9f*/);
        }
        else
        {
            originCardRPS = RondAlignment(guestCardLeft, guestCardRight, guestCards.Count, -0.5f, Vector3.one/* * 1.9f*/);
        }
        var targetCards = isMine ? hostCards : guestCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originRPS = originCardRPS[i];
            targetCard.MoveTransform(targetCard.originRPS);
        }
    }

    List<PRS> RondAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
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
            print("var targetRot = Quaternion.identity;");

            if (PhotonNetwork.IsMasterClient && objCount < 4)
            {
                targetRot = Quaternion.Euler(0, 0, targetRot.eulerAngles.z + 180);
            }

            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? -curve : curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
                print("targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);");
            }

            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }
}
