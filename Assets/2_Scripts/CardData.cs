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
    public CardType cardType;
    public int power;
    [TextArea(5, 10)] public string cardDesc;
    public Sprite cardImage;

    public Card(int _id, string _cardName, int _cost, CardType _cardType, int _power, string _cardDesc, Sprite _cardImage)
    {
        id = _id;
        cardName = _cardName;
        cost = _cost;
        cardType = _cardType;
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
        CardList.Add(new Card(_id : 0,_cardName : "none",_cost : 0, _cardType : CardType.NONE, _power : 0, _cardDesc : "none", Resources.Load<Sprite>("Character/None")));
    }
}
