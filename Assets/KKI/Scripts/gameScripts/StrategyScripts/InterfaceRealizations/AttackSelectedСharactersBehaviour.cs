using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelectedСharactersBehaviour : ICardUsable
{
    public List<Character> charactersToAttack = new List<Character>();
    private float damage;
    private BattleSystem battleSystem;
    private string abilityName;
    public AttackSelectedСharactersBehaviour(float damage, BattleSystem battleSystem, string abilityName)
    {
        this.damage = damage;
        this.battleSystem = battleSystem;
        this.abilityName = abilityName;
    }

    public event Action OnCardUse;
    public void UseAbility(GameObject gameObject)
    {
        foreach (var tempCharacter in charactersToAttack)
        {
            Character character = tempCharacter;
            if (character is KostilEnemy kostilEnemy)
            {
                character = kostilEnemy.WallEnemyCharacter;
            }

            bool isDeath = character.Damage(damage, abilityName);

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

        }

        OnCardUse?.Invoke();
    }
}