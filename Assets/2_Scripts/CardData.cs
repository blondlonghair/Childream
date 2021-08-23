using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum CardType
{
    NONE,
    ATK,
    DEF,
    ACT
}

public enum TargetType
{
    NONE,
    ONE,
    ALL
}

[System.Serializable]
public class Card
{
    public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
    public TargetType targetType;
    public int power;
    [TextArea(5, 10)] public string cardDesc;
    public Sprite cardImage;

    public Card(int _id, string _cardName, int _cost, CardType _cardType, TargetType _targetType, int _power, string _cardDesc, Sprite _cardImage)
    {
        id = _id;
        cardName = _cardName;
        cost = _cost;
        cardType = _cardType;
        targetType = _targetType;
        power = _power;
        cardDesc = _cardDesc;
        cardImage = _cardImage;
    }
}

public class CardData : MonoBehaviour
{
    public static List<Card> CardList = new List<Card>();

    private void Awake()
    {
        CardList.Add(new Card(_id: 0, _cardName: "none", _cost: 0, _cardType: CardType.NONE, _targetType: TargetType.NONE, _power: 0, _cardDesc: "none", Resources.Load<Sprite>("Character/None")));
        CardList.Add(new Card(_id: 1, _cardName: "Atk1", _cost: 1, _cardType: CardType.ATK, _targetType: TargetType.ONE, _power: 1, _cardDesc: "적을 한번 공격할 수 있습니다.", Resources.Load<Sprite>("Character/Atk1")));
        CardList.Add(new Card(_id: 2, _cardName: "Atk2", _cost: 2, _cardType: CardType.ATK, _targetType: TargetType.ONE, _power: 2, _cardDesc: "none", Resources.Load<Sprite>("Character/Atk2")));
        CardList.Add(new Card(_id: 3, _cardName: "Atk3", _cost: 3, _cardType: CardType.ATK, _targetType: TargetType.ALL, _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Atk3")));
        CardList.Add(new Card(_id: 4, _cardName: "Def1", _cost: 1, _cardType: CardType.DEF, _targetType: TargetType.ONE, _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Def1")));
        CardList.Add(new Card(_id: 5, _cardName: "Def2", _cost: 2, _cardType: CardType.DEF, _targetType: TargetType.ONE, _power: 2, _cardDesc: "none", Resources.Load<Sprite>("Character/Def2")));
        CardList.Add(new Card(_id: 6, _cardName: "Def3", _cost: 3, _cardType: CardType.DEF, _targetType: TargetType.ALL, _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Def3")));
        CardList.Add(new Card(_id: 7, _cardName: "Act1", _cost: 1, _cardType: CardType.ACT, _targetType: TargetType.ONE, _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Act1")));
        CardList.Add(new Card(_id: 8, _cardName: "Act2", _cost: 2, _cardType: CardType.ACT, _targetType: TargetType.ONE, _power: 2, _cardDesc: "none", Resources.Load<Sprite>("Character/Act2")));
        CardList.Add(new Card(_id: 9, _cardName: "Act3", _cost: 3, _cardType: CardType.ACT, _targetType: TargetType.ALL, _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Act3")));
    }
}
