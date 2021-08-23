using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class CardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<ThisCard> myCards;
    [SerializeField] List<ThisCard> otherCards;

    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;

    public static CardManager Instance;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddCard(true);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            AddCard(false);
    }

    public void AddCard(bool isMine)
    {
        var cardObject = PhotonNetwork.Instantiate("Prefab/Card", Vector3.zero, Quaternion.identity);
        var card = cardObject.GetComponent<ThisCard>();
        card.Setup(1, isMine);
        (isMine ? myCards : otherCards).Add(card);

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
            originCardRPS = RondAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one/* * 1.9f*/);
        }
        else
        {
            originCardRPS = RondAlignment(otherCardLeft, otherCardRight, otherCards.Count, -0.5f, Vector3.one/* * 1.9f*/);
        }
        var targetCards = isMine ? myCards : otherCards;
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
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.7f }; break;
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
            var targetRot = Quaternion.identity;

            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
        
            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }
}
