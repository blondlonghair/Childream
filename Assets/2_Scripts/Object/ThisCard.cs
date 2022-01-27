using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Utils;
using Enums;
using Unity.Mathematics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TMPro;

namespace Online
{
    public class ThisCard : MonoBehaviourPunCallbacks
    {
        [Header("카드 아이디 입력")] [SerializeField] int cardId;
        [Header("카드 정보")] public int id;
        public string cardName;
        public int cost;
        public CardType cardType;
        public TargetType targetType;
        public int power;
        [TextArea(5, 10)] public string cardDesc;
        public Sprite cardImage;
        public Sprite cardImageBG;

        [Header("카드 요소")] [SerializeField] TextMeshPro nameText;
        [SerializeField] TextMeshPro costText;
        [SerializeField] TextMeshPro descText;
        [SerializeField] SpriteRenderer CardImage;
        [SerializeField] SpriteRenderer CardImageBG;

        [HideInInspector] public PhotonView PV;

        [HideInInspector] public PRS originRPS;
        // public Canvas canvas;

        private bool isLerp;
        private float time = 0;

        private Coroutine coroutine;
        // private bool isLerping;

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
            if (PV.IsMine) CardFront(true);
            else CardFront(false);
        }

        void Update()
        {
            if (isLerp)
            {
                OrderInLayer(originRPS.index);
                transform.position = Vector3.Lerp(transform.position, originRPS.pos, 0.2f);
                transform.rotation = Quaternion.Lerp(transform.rotation, originRPS.rot, 0.2f);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);

                if (transform.position == originRPS.pos)
                {
                    isLerp = false;
                }
            }
        }

        public void CardZoomIn()
        {
            OrderInLayer(100);
            transform.localScale = new Vector3(2, 2, 2);
            transform.position = new Vector3(transform.position.x, PhotonNetwork.IsMasterClient ? 5 : -5, -9);
            transform.rotation = Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0);

            // print($"ZoomIn {id}");
            // if (coroutine != null) StopCoroutine(coroutine);
            // coroutine = StartCoroutine(Co_CardZoomIn());
        }

        public void CardZoomOut()
        {
            OrderInLayer(originRPS.index);
            transform.localScale = Vector3.one;
            transform.position = originRPS.pos;
            transform.rotation = originRPS.rot;

            // print($"ZoomOut {id}");
            // if (coroutine != null) StopCoroutine(coroutine);
            // coroutine = StartCoroutine(Co_CardZoomOut());
        }

        public void CardStopCoroutine()
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }

        IEnumerator Co_CardZoomIn()
        {
            OrderInLayer(100);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);

            while (Mathf.Approximately(transform.position.y, PhotonNetwork.IsMasterClient ? 5 : -5) &&
                   Mathf.Approximately(transform.rotation.z, PhotonNetwork.IsMasterClient ? 180 : 0) &&
                   Mathf.Approximately(transform.localScale.x, 2))
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2, 2, 2), 0.5f);
                transform.position = Vector3.Lerp(transform.position,
                    new Vector3(transform.position.x, PhotonNetwork.IsMasterClient ? 5 : -5, 9), 0.5f);
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.Euler(0, 0, PhotonNetwork.IsMasterClient ? 180 : 0), 0.5f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        IEnumerator Co_CardZoomOut()
        {
            OrderInLayer(originRPS.index);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            while (Mathf.Approximately(transform.position.y, originRPS.pos.y) &&
                   Mathf.Approximately(transform.rotation.z, originRPS.rot.z) &&
                   Mathf.Approximately(transform.localScale.x, 1))
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.5f);
                transform.position = Vector3.Lerp(transform.position, originRPS.pos, 0.5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, originRPS.rot, 0.5f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void Setup(int _id, bool isFront)
        {
            cardId = _id;

            id = CardData.CardList[cardId].id;
            cardName = CardData.CardList[cardId].cardName;
            cost = CardData.CardList[cardId].cost;
            cardType = CardData.CardList[cardId].cardType;
            targetType = CardData.CardList[cardId].targetType;
            cardDesc = CardData.CardList[cardId].cardDesc;
            cardImage = CardData.CardList[cardId].cardImage;
            cardImageBG = CardData.CardList[cardId].cardImageBG;

            nameText.text = cardName;
            costText.text = cost.ToString();
            descText.text = cardDesc;
            CardImage.sprite = cardImage;
            CardImageBG.sprite = cardImageBG;

            CardFront(isFront);
        }

        void CardFront(bool isFront)
        {
            if (isFront)
            {
                gameObject.transform.Find("Front").gameObject.SetActive(true);
                gameObject.transform.Find("Back").gameObject.SetActive(false);
            }

            else
            {
                gameObject.transform.Find("Front").gameObject.SetActive(false);
                gameObject.transform.Find("Back").gameObject.SetActive(true);
            }
        }

        public void MoveTransform()
        {
            isLerp = true;
        }

        public void ChangetoEffect(bool isChange)
        {
            if (!PV.IsMine) return;

            if (isChange)
            {
                gameObject.transform.Find("Front").gameObject.SetActive(false);
                gameObject.transform.Find("Back").gameObject.SetActive(false);
                gameObject.transform.Find("Effect").gameObject.SetActive(true);
            }

            else
            {
                gameObject.transform.Find("Front").gameObject.SetActive(true);
                gameObject.transform.Find("Back").gameObject.SetActive(false);
                gameObject.transform.Find("Effect").gameObject.SetActive(false);
            }
        }

        private void OrderInLayer(int index)
        {
            nameText.sortingOrder = index;
            costText.sortingOrder = index;
            descText.sortingOrder = index;
            CardImage.sortingOrder = index - 1;
            CardImageBG.sortingOrder = index - 1;
        }

        public void EffectScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}