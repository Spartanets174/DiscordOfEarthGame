using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FormulaAttackSelectedСharactersBehaviour : ICardUsable
{
    public List<Character> characters = new List<Character>();
    private float damage;
    private BattleSystem battleSystem;
    private string abilityName;
    private Character owner;
    public FormulaAttackSelectedСharactersBehaviour(float damage, BattleSystem battleSystem, Character character, string abilityName)
    {
        this.damage = damage;
        this.battleSystem = battleSystem;
        this.abilityName = abilityName;
        this.owner = character;
    }

    public event Action OnCardUse;
    public void UseAbility(GameObject gameObject)
    {
        foreach (var tempCharacter in characters)
        {
            Character character = tempCharacter;
            if (character is KostilEnemy kostilEnemy)
            {
                character = kostilEnemy.WallEnemyCharacter;
            }

            bool isDeath = character.Damage(owner, abilityName, damage);

            if (isDeath)
            {
                string characterType = "";
                if (character is PlayerCharacter)
                {
                    characterType = "Союзный";
                }
                if (character is EnemyCharacter)
                {
                    characterType = "Вражеский";
                }
                battleSystem.gameLogCurrentText.Value = $"{characterType} персонаж {character.CharacterName} погибает от эффекта карты \"{abilityName}\"";
                GameObject.Destroy(character.gameObject);
            }

            OnCardUse?.Invoke();
        }       
    }
}