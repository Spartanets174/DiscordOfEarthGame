using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAllPlayerUnitsBehaviour : ICardSelectable
{
    private BattleSystem battleSystem;
    private string m_selectCardTipText;
    public event Action OnSelected;

    public string SelectCardTipText
    {
        get
        {
            return m_selectCardTipText;
        }
    }

    public SelectAllPlayerUnitsBehaviour(string text, BattleSystem battleSystem)
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
                enemyCharacter.IsEnabled = false;
            }

            foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
            {
                playerCharacter.IsChosen = true;
            }
        }
       
        OnSelected?.Invoke();
    }

}
