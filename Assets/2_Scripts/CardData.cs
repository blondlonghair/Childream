using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Enums;

public class Card
{
    public int id;
    public string cardName;
    public int cost;
    public string cardDesc;
    public Sprite cardImage;
    public CardType cardType;
    public virtual void CardEffective() { }
}

public abstract class AtkCard : Card
{
    public AtkCard()
    {
        cardType = CardType.ACT;
    }
    public abstract override void CardEffective();
}

public abstract class DefCard : Card
{
    public DefCard()
    {
        cardType = CardType.DEF;
    }
    public abstract override void CardEffective();
}

public abstract class ActCard : Card
{
    public ActCard()
    {
        cardType = CardType.ACT;
    }
    public abstract override void CardEffective();
}

public class EmptyCard : ActCard
{
    public EmptyCard()
    {
        id = 0;
        cardName = "None";
        cost = 0;
        cardDesc = "NoneNoneNoneNoneNoneNoneNoneNoneNoneNoneNoneNoneNone";
    }
    public override void CardEffective()
    {
    }
}

public class AtkCard1 : AtkCard
{
    public AtkCard1()
    {
        id = 1;
        cardName = "Atk1";
        cost = 1;
        cardDesc = "지정한 장소를 공격합니다. 만약 해당 장소에 상대방이 있다면 피해를 5 줍니다.";
        
    }
    public override void CardEffective()
    {
        
    }
}

public class AtkCard2 : AtkCard
{
    public AtkCard2()
    {
        id = 2;
        cardName = "Atk2";
    }
    public override void CardEffective()
    {
    }
}

public class AtkCard3 : AtkCard
{
    public AtkCard3()
    {
        id = 3;
        cardName = "Atk3";
    }
    public override void CardEffective()
    {
    }
}

public class DefCard1 : DefCard
{
    public DefCard1()
    {
        id = 4;
        cardName = "Def1";
    }
    public override void CardEffective()
    {
    }
}

public class DefCard2 : DefCard
{
    public DefCard2()
    {
        id = 5;
        cardName = "Def2";
    }
    public override void CardEffective()
    {
    }
}

public class DefCard3 : DefCard
{
    public DefCard3()
    {
        id = 6;
        cardName = "Def3";
    }
    public override void CardEffective()
    {
    }
}

public class ActCard1 : ActCard
{
    public ActCard1()
    {
        id = 7;
        cardName = "Act1";
    }
    public override void CardEffective()
    {
    }
}

public class ActCard2 : ActCard
{
    public ActCard2()
    {
        id = 8;
        cardName = "Act2";
    }
    public override void CardEffective()
    {
    }
}

public class ActCard3 : ActCard
{
    public ActCard3()
    {
        id = 9;
        cardName = "Act3";
    }
    public override void CardEffective()
    {
    }
}

// [System.Serializable]
// public class Card
// {
//     public int id;
//     public string cardName;
//     public int cost;
//     public CardType cardType;
//     public ActType actType;
//     public TargetType targetType;
//     public int power;
//     [TextArea(5, 10)] public string cardDesc;
//     public Sprite cardImage;
//
//     public Card(int _id, string _cardName, int _cost, CardType _cardType, TargetType _targetType, int _power, string _cardDesc, Sprite _cardImage)
//     {
//         id = _id;
//         cardName = _cardName;
//         cost = _cost;
//         cardType = _cardType;
//         targetType = _targetType;
//         power = _power;
//         cardDesc = _cardDesc;
//         cardImage = _cardImage;
//     }
//
//     public Card(int _id, string _cardName, int _cost, CardType _cardType, ActType _actType, TargetType _targetType, int _power, string _cardDesc, Sprite _cardImage)
//     {
//         id = _id;
//         cardName = _cardName;
//         cost = _cost;
//         cardType = _cardType;
//         actType = _actType;
//         targetType = _targetType;
//         power = _power;
//         cardDesc = _cardDesc;
//         cardImage = _cardImage;
//     }
// }

public class CardData : MonoBehaviour
{
    public static readonly List<Card> CardList = new List<Card>();

    private void Awake()
    {
        // CardList.Add(new Card(_id: 0, _cardName: "none", _cost: 0, _cardType: CardType.NONE,
        //     _targetType: TargetType.NONE, _power: 0, _cardDesc: "none", Resources.Load<Sprite>("Character/None")));
        // CardList.Add(new Card(_id: 1, _cardName: "Atk1", _cost: 1, _cardType: CardType.ATK, _targetType: TargetType.ONE,
        //     _power: 5, _cardDesc: "적을 한번 공격할 수 있습니다.", Resources.Load<Sprite>("Character/Atk1")));
        // CardList.Add(new Card(_id: 2, _cardName: "Atk2", _cost: 2, _cardType: CardType.ATK, _targetType: TargetType.ALL,
        //     _power: 2, _cardDesc: "none", Resources.Load<Sprite>("Character/Atk2")));
        // CardList.Add(new Card(_id: 3, _cardName: "Atk3", _cost: 3, _cardType: CardType.ATK, _targetType: TargetType.ONE,
        //     _power: 10, _cardDesc: "none", Resources.Load<Sprite>("Character/Atk3")));
        // CardList.Add(new Card(_id: 4, _cardName: "Def1", _cost: 3, _cardType: CardType.DEF, _targetType: TargetType.ONE,
        //     _power: 3, _cardDesc: "none", Resources.Load<Sprite>("Character/Def1")));
        // CardList.Add(new Card(_id: 5, _cardName: "Def2", _cost: 2, _cardType: CardType.DEF, _targetType: TargetType.ONE,
        //     _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Def2")));
        // CardList.Add(new Card(_id: 6, _cardName: "Def3", _cost: 2, _cardType: CardType.DEF, _targetType: TargetType.ONE,
        //     _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Def3")));
        // CardList.Add(new Card(_id: 7, _cardName: "Act1", _cost: 1, _cardType: CardType.ACT, _actType: ActType.CHANGECARD, _targetType: TargetType.ONE,
        //     _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Act1")));
        // CardList.Add(new Card(_id: 8, _cardName: "Act2", _cost: 2, _cardType: CardType.ACT, _actType: ActType.HEAL, _targetType: TargetType.ONE,
        //     _power: 2, _cardDesc: "none", Resources.Load<Sprite>("Character/Act2")));
        // CardList.Add(new Card(_id: 9, _cardName: "Act3", _cost: 3, _cardType: CardType.ACT, _actType: ActType.LOCKGRID, _targetType: TargetType.ALL,
        //     _power: 1, _cardDesc: "none", Resources.Load<Sprite>("Character/Act3")));

        CardList.Add(new EmptyCard());
        CardList.Add(new AtkCard1());
        CardList.Add(new AtkCard2());
        CardList.Add(new AtkCard3());
        CardList.Add(new DefCard1());
        CardList.Add(new DefCard2());
        CardList.Add(new DefCard3());
        CardList.Add(new ActCard1());
        CardList.Add(new ActCard2());
        CardList.Add(new ActCard3());
    }
}