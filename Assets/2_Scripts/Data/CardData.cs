using System;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "CardData", menuName = "CardData", order = 0)]
public class CardDatas : ScriptableObject
{
    [Serializable]
    public class Card
    {
        public int id;
        public string cardName;
        public int cost;
        public string cardDesc;
        public Sprite cardImage;
        public Sprite cardImageBG;
        public CardType cardType;
        public TargetType targetType;
        public GameObject Effect;
    }
    
    public Card[] card;
}