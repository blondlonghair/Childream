using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AtkType
{
    NONE,
    ONCE,
    ALL
}

public enum MoveType
{
    NONE,
    RIGHT,
    LEFT,
    RANDOM
}

#if UNITY_EDITOR
public class ShowOnlyAttribute : PropertyAttribute { }
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[System.Serializable]
public class Card
{
    public int id;
    public string cardName;
    public int cost;
    public AtkType atkType;
    public MoveType moveType;
    public int power;
    [TextArea(5, 10)] public string cardDesc;
    public Sprite cardImage;

    public Card(int _id, string _cardName, int _cost, AtkType _atkType, MoveType _moveType, int _power, string _cardDesc, Sprite _cardImage)
    {
        id = _id;
        cardName = _cardName;
        cost = _cost;
        atkType = _atkType;
        moveType = _moveType;
        power = _power;
        cardDesc = _cardDesc;
        cardImage = _cardImage;
    }
}

public class CardData : MonoBehaviour
{
    //static CardData instance;

    //[SerializeField] List<Card> Cards = new List<Card>();
    public static List<Card> CardList = new List<Card>();

    private void Awake()
    {
        //instance = this;
        //CardList = instance.Cards;

        CardList.Add(new Card(_id : 0,_cardName : "none",_cost : 0,_atkType : AtkType.NONE, 
            _moveType : MoveType.NONE, _power : 0, _cardDesc : "none", Resources.Load<Sprite>("½£¹è°æ(´«)")));
    }
}
