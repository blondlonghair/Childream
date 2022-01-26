using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Enums;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using Utils;

public class Card : MonoBehaviour
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

    //카드 효과들 First는 시전시 발동, Second는 카드 인보크시 발동
    public virtual void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);
    }

    public virtual void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        CardManager.Instance.ShowWatIUsed(_caster, _target, id);
        EffectManager.Instance.InitEffect(_caster, _target, _index, id);
    }
}

public abstract class AtkCard : Card
{
    public int damage;

    protected void EnemyDefence(Player _caster, Player _target, int _index)
    {
        StartCoroutine(Co_Defence(_target));
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);

        StartCoroutine(Co_Attack(_caster));
    }

    IEnumerator Co_Defence(Player _target)
    {
        Vector3 firstPos = _target.transform.position;
        
        for (int i = 0; i < 5; i++)
        {
            _target.transform.position = new Vector3(firstPos.x - 0.5f, _target.transform.position.y, 0);
            yield return new WaitForSeconds(0.05f);
            _target.transform.position = new Vector3(firstPos.x + 0.5f, _target.transform.position.y, 0);
            yield return new WaitForSeconds(0.05f);
        }

        _target.transform.position = new Vector3(firstPos.x, _target.transform.position.y, 0);
    }

    IEnumerator Co_Attack(Player _caster)
    {
        Vector3 movePos = new Vector3(_caster.transform.position.x, (_caster.photonView.IsMine ? 1 : 3) * (PhotonNetwork.IsMasterClient ? -1 : 1));
        while (!Mathf.Approximately(_caster.transform.position.y, movePos.y))
        {
            _caster.transform.position = Vector3.Lerp(_caster.transform.position, movePos, 0.5f);
            // _caster.transform.position = new Vector3(_caster.transform.position.x,
            //     Mathf.Lerp(transform.position.y, movePos.y, 0.5f), 0);
            yield return new WaitForSeconds(0.01f);
        }
        
        movePos = new Vector3(_caster.transform.position.x, (_caster.photonView.IsMine ? -1 : 5) * (PhotonNetwork.IsMasterClient ? -1 : 1));

        while (!Mathf.Approximately(_caster.transform.position.y, movePos.y))
        {
            _caster.transform.position = Vector3.Lerp(_caster.transform.position, movePos, 0.2f);
            // _caster.transform.position = new Vector3(_caster.transform.position.x,
            //     Mathf.Lerp(transform.position.y, movePos.y, 0.2f), 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}

public abstract class DefCard : Card
{
    public int counter;
    public int defence;
}

public abstract class SupCard : Card
{
}

public class EmptyCard : Card
{
    public EmptyCard()
    {
        id = CardData.CardTable[0].id;
        cardName = CardData.CardTable[0].name;
        cost = CardData.CardTable[0].cost;
        cardDesc = CardData.CardTable[0].desc;
        cardImage = CardData.CardTable[0].cardImage;
        cardImageBG = CardData.CardTable[0].cardImageBG;
        cardType = CardData.CardTable[0].cardType;
        targetType = CardData.CardTable[0].targetType;
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
        cardImage = CardData.CardTable[1].cardImage;
        cardImageBG = CardData.CardTable[1].cardImageBG;
        cardType = CardData.CardTable[1].cardType;
        targetType = CardData.CardTable[1].targetType;
        damage = 5;
        // damage = 20;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);

        if (_target.CurState == _index)
        {
            if (_target.DefMagic)
            {
                EffectManager.Instance.InitEffect(_caster, _target, _index, 4);
                CardManager.Instance.ShowWatIUsed(_target, _caster, 4);
                // SoundManager.Instance.PlaySFXSound("Attack_01");
                _target.DefMagic = false;
            }
            else
            {
                EnemyDefence(_caster, _target, _index);
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
        cardImage = CardData.CardTable[2].cardImage;
        cardImageBG = CardData.CardTable[2].cardImageBG;
        cardType = CardData.CardTable[2].cardType;
        targetType = CardData.CardTable[2].targetType;
        damage = 2;
        // damage = 20;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);

        if (_target.DefElectricity)
        {
            EffectManager.Instance.InitEffect(_caster, _target, _index, 5);
            CardManager.Instance.ShowWatIUsed(_target, _caster, 5);
            // SoundManager.Instance.PlaySFXSound("Attack_02");
            _caster.CurHp -= 1;
            _target.DefElectricity = false;   
        }
        else
        {
            EnemyDefence(_caster, _target, _index);
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
        cardImage = CardData.CardTable[3].cardImage;
        cardImageBG = CardData.CardTable[3].cardImageBG;
        cardType = CardData.CardTable[3].cardType;
        targetType = CardData.CardTable[3].targetType;
        damage = 10;
        // damage = 20;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);
        
        if (_target.CurState == _index)
        {
            if (_target.DefExplosion)
            {
                EffectManager.Instance.InitEffect(_caster, _target, _index, 6);
                CardManager.Instance.ShowWatIUsed(_target, _caster, 6);
                // SoundManager.Instance.PlaySFXSound("Attack_03");
                _caster.CurHp -= damage / 2;
                _target.DefExplosion = false;
            }
            else
            {
                EnemyDefence(_caster, _target, _index);
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
        cardImage = CardData.CardTable[4].cardImage;
        cardImageBG = CardData.CardTable[4].cardImageBG;
        cardType = CardData.CardTable[4].cardType;
        targetType = CardData.CardTable[4].targetType;
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
        cardImage = CardData.CardTable[5].cardImage;
        cardImageBG = CardData.CardTable[5].cardImageBG;
        cardType = CardData.CardTable[5].cardType;
        targetType = CardData.CardTable[5].targetType;
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
        cardImage = CardData.CardTable[6].cardImage;
        cardImageBG = CardData.CardTable[6].cardImageBG;
        cardType = CardData.CardTable[6].cardType;
        targetType = CardData.CardTable[6].targetType;
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
        cardImage = CardData.CardTable[7].cardImage;
        cardImageBG = CardData.CardTable[7].cardImageBG;
        cardType = CardData.CardTable[7].cardType;
        targetType = CardData.CardTable[7].targetType;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
        // SoundManager.Instance.PlaySFXSound("Sub_01");
        _target.IsLocked = true;
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
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
        cardImage = CardData.CardTable[8].cardImage;
        cardImageBG = CardData.CardTable[8].cardImageBG;
        cardType = CardData.CardTable[8].cardType;
        targetType = CardData.CardTable[8].targetType;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);

        // SoundManager.Instance.PlaySFXSound("Sub_02");
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
        cardImage = CardData.CardTable[9].cardImage;
        cardImageBG = CardData.CardTable[9].cardImageBG;
        cardType = CardData.CardTable[9].cardType;
        targetType = CardData.CardTable[9].targetType;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        base.CardSecondAbility(_caster, _target, _index);

        // SoundManager.Instance.PlaySFXSound("Sub_03");
        
        CardManager.Instance.AddCard(_caster.PV().IsMine);
        CardManager.Instance.AddCard(_caster.PV().IsMine);
    }
}

public class Move : Card
{
    private Player caster;
    public Move()
    {
        id = 10;
        targetType = TargetType.MeSellect;
    }

    public override void CardFirstAbility(Player _caster, Player _target, int _index)
    {
    }

    public override void CardSecondAbility(Player _caster, Player _target, int _index)
    {
        if (_caster.IsLocked) return;

        // SoundManager.Instance.PlaySFXSound("Move");
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (_caster.gameObject.GetPhotonView().IsMine)
            {
                StartCoroutine(Co_Move(_caster, new Vector3((float) (_index switch {4 => 3.5f, 5 => 0, 6 => -3.5, _ => 0}), _caster.transform.position.y, 0)));
            }
            else
            {
                StartCoroutine(Co_Move(_caster, new Vector3((float)(_index switch{4 => 2.7f, 5 => 0, 6 => -2.7, _ => 0}), _caster.transform.position.y, 0)));
            }
        }
        else
        {
            if (_caster.gameObject.GetPhotonView().IsMine)
            {
                StartCoroutine(Co_Move(_caster, new Vector3((float) (_index switch {4 => -3.5f, 5 => 0, 6 => 3.5, _ => 0}), _caster.transform.position.y, 0)));
            }
            else
            {
                StartCoroutine(Co_Move(_caster, new Vector3((float) (_index switch {4 => -2.7f, 5 => 0, 6 => 2.7, _ => 0}), _caster.transform.position.y, 0)));
            }
        }

        IEnumerator Co_Move(Player caster, Vector3 pos)
        {
            while (!Mathf.Approximately(caster.transform.position.x, pos.x))
            {
                caster.transform.position = Vector3.Lerp(caster.transform.position, pos, 0.2f);
                yield return null;
            }
        }

        _caster.CurState = _index - 3;
    }
}

public class CardData : MonoBehaviour
{
    public static List<Card> CardList;
    public static List<CardInfo> CardTable;

    [SerializeField] private CardInfo[] cardTable;

    private void Awake()
    {
        CardList = new List<Card>();
        CardTable = new List<CardInfo>();
        
        CardTable.AddRange(cardTable);

        EmptyCard emptyCard = gameObject.AddComponent<EmptyCard>();
        AtkCard1 atkCard1 = gameObject.AddComponent<AtkCard1>();
        AtkCard2 atkCard2 = gameObject.AddComponent<AtkCard2>();
        AtkCard3 atkCard3 = gameObject.AddComponent<AtkCard3>();
        DefCard1 defCard1 = gameObject.AddComponent<DefCard1>();
        DefCard2 defCard2 = gameObject.AddComponent<DefCard2>();
        DefCard3 defCard3 = gameObject.AddComponent<DefCard3>();
        SupCard1 supCard1 = gameObject.AddComponent<SupCard1>();
        SupCard2 supCard2 = gameObject.AddComponent<SupCard2>();
        SupCard3 supCard3 = gameObject.AddComponent<SupCard3>();
        Move move = gameObject.AddComponent<Move>();
        
        CardList.Add(emptyCard);
        CardList.Add(atkCard1);
        CardList.Add(atkCard2);
        CardList.Add(atkCard3);
        CardList.Add(defCard1);
        CardList.Add(defCard2);
        CardList.Add(defCard3);
        CardList.Add(supCard1);
        CardList.Add(supCard2);
        CardList.Add(supCard3);
        CardList.Add(move);
    }
}