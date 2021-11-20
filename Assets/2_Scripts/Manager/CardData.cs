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

public class Card
{
    //카드 정보들
    public int id;
    public string cardName;
    public int cost;
    public string cardDesc;
    public Sprite cardImage;
    public Sprite cardImageBG;
    public CardType cardType;
    public TargetType targetType;
    public GameObject Effect;

    //카드 효과들 First는 시전시 발동, Second는 카드 인보크시 발동
    public virtual void CardFirstAbility(Player _caster, Player _target, int _index) {}

    public virtual void CardSecondAbility(Player _caster, Player _target, int _index)
    {
    }
}

public abstract class AtkCard : Card
{
    public int damage;

    public AtkCard()
    {
        cardImageBG = Resources.Load<Sprite>("Card/카드-공격");
        cardType = CardType.ATK;
        targetType = TargetType.ENEMY;
    }

    public abstract override void CardFirstAbility(Player _caster, Player _target, int _index);
    public abstract override void CardSecondAbility(Player _caster, Player _target, int _index);
}

public abstract class DefCard : Card
{
    public int counter;
    public int defence;

    public DefCard()
    {
        cardImageBG = Resources.Load<Sprite>("Card/카드-수비");
        cardType = CardType.DEF;
        targetType = TargetType.ME;
    }

    public abstract override void CardFirstAbility(Player _caster, Player _target, int _index);
    public abstract override void CardSecondAbility(Player _caster, Player _target, int _index);
}

public abstract class SupCard : Card
{
    public SupCard()
    {
        cardImageBG = Resources.Load<Sprite>("Card/카드-지원");
        cardType = CardType.SUP;
    }

    public abstract override void CardFirstAbility(Player _caster, Player _target, int _index);
    public abstract override void CardSecondAbility(Player _caster, Player _target, int _index);
}

public class EmptyCard : Card
{
    public EmptyCard()
    {
        cardType = CardType.NONE;
        targetType = TargetType.NONE;

        id = CardData.CardTable[0].id;
        cardName = CardData.CardTable[0].name;
        cost = CardData.CardTable[0].cost;
        cardDesc = CardData.CardTable[0].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/None");
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
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

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        
    }
    
    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);

        if (_target.CurState == _index)
        {
            if (_target.DefMagic)
            {
                EffectManager.Instance.InitEffect(_caster, _target, _index, 4);
                _target.DefMagic = false;
            }
            else
            {
                _target.CurHp -= damage;
            }
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

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        
    }
    
    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);
        
        if (_target.DefElectricity)
        {
            EffectManager.Instance.InitEffect(_caster, _target, _index, 5);
            _caster.CurHp -= 1;
            _target.DefElectricity = false;   
        }
        else
        {
            _target.CurHp -= damage;
        }
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
    
    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);

        if (_target.CurState == _index)
        {
            if (_target.DefExplosion)
            {
                EffectManager.Instance.InitEffect(_caster, _target, _index, 6);
                _caster.CurHp -= damage / 2;
                _target.DefExplosion = false;
            }
            else
            {
                _target.CurHp -= damage;
            }
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

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        _caster.DefMagic = true;
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
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

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        _caster.DefElectricity = true;
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
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

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        _caster.DefExplosion = true;
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
    }
}

public class SupCard1 : SupCard
{
    public SupCard1()
    {
        id = CardData.CardTable[7].id;
        cardName = CardData.CardTable[7].name;
        cost = CardData.CardTable[7].cost;
        cardDesc = CardData.CardTable[7].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_1");
        targetType = TargetType.ENEMY;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);

        _target.IsLocked = true;
    }
}

public class SupCard2 : SupCard
{
    public SupCard2()
    {
        id = CardData.CardTable[8].id;
        cardName = CardData.CardTable[8].name;
        cost = CardData.CardTable[8].cost;
        cardDesc = CardData.CardTable[8].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_2");
        targetType = TargetType.ME;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);

        for (int i = 0; i < 3; i++)
        {
            if (_caster.CurHp == _caster.MaxHp)
            {
                break;
            }

            _caster.CurHp++;
        }
    }
}

public class SupCard3 : SupCard
{
    public SupCard3()
    {
        id = CardData.CardTable[9].id;
        cardName = CardData.CardTable[9].name;
        cost = CardData.CardTable[9].cost;
        cardDesc = CardData.CardTable[9].desc;
        cardImage = Resources.Load<Sprite>("Card/Cards/Sup_3");
        targetType = TargetType.ME;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);

        CardManager.Instance.AddCard(_target.PV().IsMine);
        CardManager.Instance.AddCard(_target.PV().IsMine);
    }
}

public class Move : Card
{
    public Move()
    {
        id = 10;
        targetType = TargetType.ME;
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        if (_caster.IsLocked)
        {
            return;
        }

        _caster.transform.position = new Vector3(PhotonNetwork.IsMasterClient ? 
                (float)(_index switch{1 => 3.5, 2 => 0, 3 => -3.5, _ => 0}) : 
                (float)(_index switch{1 => -3.5, 2 => 0, 3 => 3.5, _ => 0}), 
            _caster.transform.position.y, 0);

        _caster.CurState = _index;
        _caster.CurMoveCount--;
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
    public static List<Card> CardList;
    public static List<CardTable> CardTable;

    [SerializeField] private TextAsset cardTable;
    [SerializeField] private GameObject testEffect;

    private void Awake()
    {
        CardList = new List<Card>();
        CardTable = new List<CardTable>();
        
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
        CardList.Add(new Move());
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