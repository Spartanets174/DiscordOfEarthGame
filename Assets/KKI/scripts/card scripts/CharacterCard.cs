using Org.BouncyCastle.Bcpg.Sig;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

//scriptable object для карт со всеми параметрами карты
[CreateAssetMenu(fileName = "New Card",menuName = "Card")]
public class CharacterCard: Card
{
    public GameObject characterPrefab;
    public Enums.Classes Class;

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

    [Space, Header("Effects")]
    public GameObject damageEffect;
    public GameObject healEffect;
    public GameObject attackEffect;

/*    [Space, Header("Sounds")]
    public AudioClip damageSound;
    public AudioClip healSound;
    public AudioClip attackSound;*/


    [Header("Abilities")]
    [SerializeReference, SubclassSelector]
    public BasePassiveCharacterAbilityData passiveCharacterAbilityData;   

    [SerializeReference, SubclassSelector]
    public BaseCharacterAbilityData attackCharacterAbilityData;

    [SerializeReference, SubclassSelector]
    public BaseCharacterAbilityData defenceCharacterAbilityData;

    [SerializeReference, SubclassSelector]
    public BaseCharacterAbilityData buffCharacterAbilityData;
}
