using System;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "CardInfo", menuName = "CardInfo", order = 0)]
public class CardInfo : ScriptableObject
{
    public int id;
    public string name;
    public int cost;
    public string desc;
    public Sprite cardImage;
    public Sprite cardImageBG;
    public CardType cardType;
    public TargetType targetType;
}