using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Enums;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using Utils;

public class Action
{
    public virtual void CardEffective(Player _target, int _index) {}
}

public abstract class Card : Action
{
    public int id;
    public string cardName;
    public int cost;
    public string cardDesc;
    public Sprite cardImage;
    public CardType cardType;
    public abstract override void CardEffective(Player _target, int _index);
}

public abstract class AtkCard : Card
{
    public int damage;

    public AtkCard()
    {
        cardType = CardType.ATK;
    }

    public abstract override void CardEffective(Player _target, int _index);
}

public abstract class DefCard : Card
{
    public int counter;
    public int defence;

    public DefCard()
    {
        cardType = CardType.DEF;
    }

    public abstract override void CardEffective(Player _target, int _index);
}

public abstract class ActCard : Card
{
    public ActCard()
    {
        cardType = CardType.SUP;
    }

    public abstract override void CardEffective(Player _target, int _index);
}

public abstract class Actions : Card
{
    public Actions()
    {
        cardType = CardType.NONE;
    }

    public abstract override void CardEffective(Player _target, int _index);
}

public class EmptyCard : Card
{
    public EmptyCard()
    {
        cardType = CardType.NONE;
        
        id = CardData.CardTable[0].id;
        cardName = CardData.CardTable[0].name;
        cost = CardData.CardTable[0].cost;
        cardDesc = CardData.CardTable[0].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/None");
    }

    public override void CardEffective(Player _target, int _index)
    {
    }
}

public class AtkCard1 : AtkCard
{
    public AtkCard1()
    {
        id = CardData.CardTable[1].id;
        cardName = CardData.CardTable[1].name;
        cost = CardData.CardTable[1].cost;
        cardDesc = CardData.CardTable[1].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Atk_1");
        damage = 5;
    }

    public override void CardEffective(Player _target, int _index)
    {
        if (_target.CurState == _index)
        {
            _target.CurHp -= damage;
        }
    }
}

public class AtkCard2 : AtkCard
{
    public AtkCard2()
    {
        id = CardData.CardTable[2].id;
        cardName = CardData.CardTable[2].name;
        cost = CardData.CardTable[2].cost;
        cardDesc = CardData.CardTable[2].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Atk_2");
        damage = 2;
    }

    public override void CardEffective(Player _target, int _index)
    {
        _target.CurHp -= damage;
    }
}

public class AtkCard3 : AtkCard
{
    public AtkCard3()
    {
        id = CardData.CardTable[3].id;
        cardName = CardData.CardTable[3].name;
        cost = CardData.CardTable[3].cost;
        cardDesc = CardData.CardTable[3].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Atk_3");
        damage = 10;
    }

    public override void CardEffective(Player _target, int _index)
    {
        if (_target.CurState == _index)
        {
            _target.CurHp -= damage;
        }
    }
}

public class DefCard1 : DefCard
{
    public DefCard1()
    {
        id = CardData.CardTable[4].id;
        cardName = CardData.CardTable[4].name;
        cost = CardData.CardTable[4].cost;
        cardDesc = CardData.CardTable[4].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Def_1");
    }

    public override void CardEffective(Player _target, int _index)
    {
    }
}

public class DefCard2 : DefCard
{
    public DefCard2()
    {
        id = CardData.CardTable[5].id;
        cardName = CardData.CardTable[5].name;
        cost = CardData.CardTable[5].cost;
        cardDesc = CardData.CardTable[5].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Def_2");
    }

    public override void CardEffective(Player _target, int _index)
    {
    }
}

public class DefCard3 : DefCard
{
    public DefCard3()
    {
        id = CardData.CardTable[6].id;
        cardName = CardData.CardTable[6].name;
        cost = CardData.CardTable[6].cost;
        cardDesc = CardData.CardTable[6].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Def_3");
    }

    public override void CardEffective(Player _target, int _index)
    {
    }
}

public class SupCard1 : ActCard
{
    public SupCard1()
    {
        id = CardData.CardTable[7].id;
        cardName = CardData.CardTable[7].name;
        cost = CardData.CardTable[7].cost;
        cardDesc = CardData.CardTable[7].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_1");
    }

    public override void CardEffective(Player _target, int _index)
    {
        _target.IsLocked = true;
    }
}

public class SupCard2 : ActCard
{
    public SupCard2()
    {
        id = CardData.CardTable[8].id;
        cardName = CardData.CardTable[8].name;
        cost = CardData.CardTable[8].cost;
        cardDesc = CardData.CardTable[8].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_2");
    }

    public override void CardEffective(Player _target, int _index)
    {
        for (int i = 0; i < 3; i++)
        {
            if (_target.CurHp == _target.MaxHp)
            {
                break;
            }

            _target.CurHp++;
        }
    }
}

public class SupCard3 : ActCard
{
    public SupCard3()
    {
        id = CardData.CardTable[9].id;
        cardName = CardData.CardTable[9].name;
        cost = CardData.CardTable[9].cost;
        cardDesc = CardData.CardTable[9].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_3");
    }

    public override void CardEffective(Player _target, int _index)
    {
        CardManager.Instance.AddCard(!_target.PV().IsMine);
        CardManager.Instance.AddCard(!_target.PV().IsMine);
    }
}

public class Move : Action
{
    public override void CardEffective(Player _target, int _index)
    {
        if (_index == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _target.transform.position = new Vector3(3.5f, _target.transform.position.y, 0);
            }
            else
            {
                _target.transform.position = new Vector3(-3.5f, _target.transform.position.y, 0);
            }
        }

        else if (_index == 2)
        {
            _target.transform.position = new Vector3(0, _target.transform.position.y, 0);
        }

        else if (_index == 3)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _target.transform.position = new Vector3(-3.5f, _target.transform.position.y, 0);
            }
            else
            {
                _target.transform.position = new Vector3(3.5f, _target.transform.position.y, 0);
            }
        }

        _target.CurState = _index;
        _target.CurMoveCount--;
    }
}

//아이디	이름	마나	설명	종류
public class CardTable
{
    public int id;
    public string name;
    public int cost;
    public string desc;

    public CardTable(int id, string name, int cost, string desc)
    {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.desc = desc;
    }
}

public class CardData : MonoBehaviour
{
    public static readonly List<Card> CardList = new List<Card>();
    public static readonly List<CardTable> CardTable = new List<CardTable>();

    [SerializeField] private TextAsset cardTable;

    private void Awake()
    {
        ExcelParsing();

        CardList.Add(new EmptyCard());
        CardList.Add(new AtkCard1());
        CardList.Add(new AtkCard2());
        CardList.Add(new AtkCard3());
        CardList.Add(new DefCard1());
        CardList.Add(new DefCard2());
        CardList.Add(new DefCard3());
        CardList.Add(new SupCard1());
        CardList.Add(new SupCard2());
        CardList.Add(new SupCard3());
    }

    private void ExcelParsing()
    {
        string currentText = cardTable.text.Substring(0, cardTable.text.Length - 1);
        string[] lines = currentText.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split('\t');
            CardTable.Add(new CardTable(int.Parse(words[0]), words[1], int.Parse(words[2]), words[3]));
        }
    }
}