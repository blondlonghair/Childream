using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<ThisCard> myCards;
    [SerializeField] List<ThisCard> otherCards;

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
        print("add");
    }

    void CardAlignment(bool isMine)
    {
        List<Transform> originCardTF = new List<Transform>();
        if (isMine)
        {
            originCardTF = RondAlignment();
        }
        else
        {
            originCardTF = RondAlignment();
        }
        var targetCards = isMine ? myCards : otherCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originTF = originCardTF[];
            targetCard.MoveTransform(targetCard.originTF, true, 0.7f);
        }
    }

    List<Transform> RondAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<Transform> results = new List<Transform>(objCount);

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
            results.Add(new Transform(targetPos, targetRot, scale));
        }
        return results;
    }
}
