using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCellBehaviour : ICardUsable
{
    private BattleSystem battleSystem;

    public event Action OnCardUse;

    public MoveToCellBehaviour(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
    }

    public void UseAbility(GameObject gameObject)
    {
        Cell currentCell = gameObject.GetComponent<Cell>();
        Character character = battleSystem.State is PlayerTurn?battleSystem.PlayerController.CurrentPlayerCharacter: battleSystem.EnemyController.CurrentEnemyCharacter;
        Vector2 pos = character.PositionOnField;

        character.Move(0, currentCell.transform);

        OnCardUse?.Invoke();
    }

}
