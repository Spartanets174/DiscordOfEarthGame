using System;
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

    public void AttackEnemyCharacters()
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
            AttackCharacter(target);
        }        
    }

    public void AttackPlayerCharacters()
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
            AttackCharacter(target);
        }
    }

    public void AttackEnemyCharacter(EnemyCharacter currentTarget)
    {
        foreach (var cell in CellsToAttack)
        {
            EnemyCharacter enemyCharacter = cell.GetComponentInChildren<EnemyCharacter>();
            if (enemyCharacter == currentTarget)
            {
                AttackCharacter(currentTarget);
            }
        }
    }

    public void AttackPlayerCharacter(PlayerCharacter currentTarget)
    {
        foreach (var cell in CellsToAttack)
        {
            PlayerCharacter playerCharacter = cell.GetComponentInChildren<PlayerCharacter>();
            if (playerCharacter == currentTarget)
            {
                AttackCharacter(currentTarget);
            }
        }
    }

    private void AttackCharacter(Character currentTarget)
    {
        float finalDamage = currentTarget.Damage(this);
        bool isDeath = currentTarget.Health == 0;
        if (finalDamage > 0)
        {
            BattleSystem.Instance.GameUIPresenter.AddMessageToGameLog($"{this.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");
        }
        else
        {
            BattleSystem.Instance.GameUIPresenter.AddMessageToGameLog($"{currentTarget.CharacterName} избежал получения урона от {this.CharacterName}");
        }
        if (BattleSystem.Instance.State is PlayerTurn)
        {
            if (currentTarget is PlayerCharacter playerCharacter)
            {
                CheckPlayerDeath(playerCharacter, isDeath);
            }
            
        }
        else
        {
            if (currentTarget is EnemyCharacter enemyCharacter)
            {
                CheckEnemyDeath(enemyCharacter, isDeath);
            }         
        }
    }

    private void CheckPlayerDeath(PlayerCharacter currentTarget, bool isDeath)
    {     
        if (isDeath)
        {
            BattleSystem.Instance.PlayerController.PlayerCharactersObjects.Remove(currentTarget);
            BattleSystem.Instance.GameUIPresenter.AddMessageToGameLog($"Союзный юнит {currentTarget.CharacterName} убит");
            GameObject.Destroy(currentTarget.gameObject);
        }

        if (BattleSystem.Instance.PlayerController.PlayerCharactersObjects.Count == 0)
        {
            BattleSystem.Instance.SetLost();
        }
    }

    public void CheckEnemyDeath(EnemyCharacter currentTarget, bool isDeath)
    {
        if (isDeath)
        {
            BattleSystem.Instance.EnemyController.EnemyCharObjects.Remove(currentTarget);
            GameObject.Destroy(currentTarget.gameObject);
            BattleSystem.Instance.GameUIPresenter.AddMessageToGameLog($"Вражеский юнит {currentTarget.name} убит");
        }

        if (BattleSystem.Instance.EnemyController.EnemyCharObjects.Count == 0)
        {
            BattleSystem.Instance.SetWin();
        }
    }
}
