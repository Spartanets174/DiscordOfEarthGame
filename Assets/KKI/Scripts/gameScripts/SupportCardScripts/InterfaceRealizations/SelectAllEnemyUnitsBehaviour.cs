using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAllEnemyUnitsBehaviour : ICardSelectable
{
    private BattleSystem battleSystem;
    public event Action OnSelected;
    public event Action OnCancelSelection;

    private string m_selectCardTipText;
    public string SelectCardTipText
    {
        get
        {
            return m_selectCardTipText;
        }
    }

    public SelectAllEnemyUnitsBehaviour(string text, BattleSystem battleSystem)
    {
        m_selectCardTipText = text;
        this.battleSystem = battleSystem;
    }

    public void SelectCard()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.IsChosen = true;
            }

            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.IsEnabled = false;
            }
        }

        OnSelected?.Invoke();
    }

    public void CancelSelection()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.IsChosen = false;
            }

            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.IsEnabled = true;
            }
        }
        OnCancelSelection?.Invoke();
    }
}
