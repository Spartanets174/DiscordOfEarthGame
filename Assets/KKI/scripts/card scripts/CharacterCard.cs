using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//scriptable object для карт со всеми параметрами карты
[CreateAssetMenu(fileName = "New Card",menuName = "Card")]
public class CharacterCard: Card
{   
    public enums.Classes Class;

    [Space, Header("Characteristics")]
    public float health;
    public int speed;
    public float physAttack;
    public float magAttack;
    public int range;
    public float physDefence;
    public float magDefence;
    public float critChance;
    public float critNum;
    public string passiveAbility;
    public string attackAbility;
    public string defenceAbility;
    public string buffAbility;
}
