using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    [Space, Header("Info")]
    public string characterName;
    public string description;
    public enums.Races race;
    public Sprite image;
    public enums.Rarity rarity;
    public int Price;
    public int id;
}
