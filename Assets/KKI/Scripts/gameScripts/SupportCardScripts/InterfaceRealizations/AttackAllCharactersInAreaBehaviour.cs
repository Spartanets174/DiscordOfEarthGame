using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAllCharactersInAreaBehaviour : ICardUsable
{
    public List<Cell> cellsToAttack;
    private float damage;
    private BattleSystem battleSystem;
    private string abilityName;
    public AttackAllCharactersInAreaBehaviour(float damage, BattleSystem battleSystem, string abilityName)
    {
        this.damage = damage;
        this.battleSystem = battleSystem;
        this.abilityName = abilityName;
    }

    public event Action OnCardUse;
    public void UseAbility(GameObject gameObject)
    {
        foreach (var cell in cellsToAttack)
        {
            Character character = cell.GetComponentInChildren<Character>();

            float finalDamage = character.Damage(damage);
            battleSystem.GameUIPresenter.AddMessageToGameLog($"{character.CharacterName} получил урон в количестве {finalDamage * 100:00.00} от {abilityName}");
            bool isDeath = character.Health == 0;

            if (isDeath)
            {
                string characterType = "";
                if (character is StaticEnemyCharacter staticEnemyCharacter)
                {
                    battleSystem.EnemyController.StaticEnemyCharObjects.Remove(staticEnemyCharacter);
                    characterType = "Юнит";
                }
                if (character is PlayerCharacter playerCharacter)
                {
                    battleSystem.PlayerController.PlayerCharactersObjects.Remove(playerCharacter);
                    characterType = "Союзный юнит";
                }
                if (character is EnemyCharacter enemyCharacter)
                {
                    battleSystem.EnemyController.EnemyCharObjects.Remove(enemyCharacter);
                    characterType = "вражеский юнит";
                }
                battleSystem.GameUIPresenter.AddMessageToGameLog($"{characterType} {character.CharacterName} убит");
                GameObject.Destroy(character.gameObject);
            }
        }
        OnCardUse?.Invoke();
    }

}
