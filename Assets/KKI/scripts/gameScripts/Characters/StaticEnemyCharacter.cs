using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class StaticEnemyCharacter : Enemy
{
    private List<Cell> m_cellsToAttack=new();
    public List<Cell> CellsToAttack
    {
        get => m_cellsToAttack;
        set => m_cellsToAttack = value;
    }

    public void AttackEnemyCharacters(BattleSystem battleSystem)
    {
        List<EnemyCharacter> enemyCharactersToAttack = new();
        foreach (var cell in CellsToAttack)
        {
            EnemyCharacter currentTarget = cell.GetComponentInChildren<EnemyCharacter>();
            if (currentTarget != null)
            {
                enemyCharactersToAttack.Add(currentTarget);
            }
        }
        if (enemyCharactersToAttack.Count!=0)
        {
            EnemyCharacter target = enemyCharactersToAttack.OrderBy(x => x.Health).ToList()[0];
            AttackEnemyCharacter(target, battleSystem);
        }        
    }

    public void AttackPlayerCharacters(BattleSystem battleSystem)
    {
        List<PlayerCharacter> playerCharacterToAttack = new();
        foreach (var cell in CellsToAttack)
        {
            PlayerCharacter currentTarget = cell.GetComponentInChildren<PlayerCharacter>();
            if(currentTarget!=null)
            {
                playerCharacterToAttack.Add(currentTarget);
            }
        }
        if (playerCharacterToAttack.Count!=0)
        {
            PlayerCharacter target = playerCharacterToAttack.OrderBy(x => x.Health).ToList()[0];
            AttackPlayerCharacter(target, battleSystem);
        }
        
    }

    public void AttackEnemyCharacter(BattleSystem battleSystem, EnemyCharacter enemyCharacter)
    {
        foreach (var cell in CellsToAttack)
        {
            EnemyCharacter currentTarget = cell.GetComponentInChildren<EnemyCharacter>();
            if (currentTarget == enemyCharacter)
            {
                AttackEnemyCharacter(currentTarget, battleSystem);
            }
        }
    }

    public void AttackPlayerCharacter(BattleSystem battleSystem, PlayerCharacter playerCharacter)
    {
        foreach (var cell in CellsToAttack)
        {
            PlayerCharacter currentTarget = cell.GetComponentInChildren<PlayerCharacter>();
            if (currentTarget == playerCharacter)
            {
                AttackPlayerCharacter(currentTarget, battleSystem);
            }
        }
    }


    public void AttackPlayerCharacter(PlayerCharacter currentTarget, BattleSystem battleSystem)
    {
        float finalDamage = currentTarget.Damage(this);
        bool isDeath = currentTarget.Health == 0;
        if (finalDamage > 0)
        {
            battleSystem.GameUIPresenter.AddMessageToGameLog($"{this.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");
        }
        else
        {
            battleSystem.GameUIPresenter.AddMessageToGameLog($"{currentTarget.CharacterName} избежал получения урона от {this.CharacterName}");
        }


        if (isDeath)
        {
            battleSystem.PlayerController.PlayerCharactersObjects.Remove(currentTarget);
            battleSystem.GameUIPresenter.AddMessageToGameLog($"Союзный юнит {currentTarget.CharacterName} убит");
            GameObject.Destroy(currentTarget.gameObject);
        }

        if (battleSystem.PlayerController.PlayerCharactersObjects.Count == 0)
        {
            battleSystem.SetLost();
        }
    }

    public void AttackEnemyCharacter(EnemyCharacter currentTarget, BattleSystem battleSystem)
    {
        float finalDamage = currentTarget.Damage(this);
        bool isDeath = currentTarget.Health == 0;
        if (finalDamage > 0)
        {
            battleSystem.GameUIPresenter.AddMessageToGameLog($"{this.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");
        }
        else
        {
            battleSystem.GameUIPresenter.AddMessageToGameLog($"{currentTarget.CharacterName} избежал получения урона от {this.CharacterName}");
        };

        if (isDeath)
        {
            battleSystem.EnemyController.EnemyCharObjects.Remove(currentTarget);
            GameObject.Destroy(currentTarget.gameObject);
            battleSystem.GameUIPresenter.AddMessageToGameLog($"Вражеский юнит {currentTarget.name} убит");
        }

        if (battleSystem.EnemyController.EnemyCharObjects.Count == 0)
        {
            battleSystem.SetWin();
        }
    }
}
