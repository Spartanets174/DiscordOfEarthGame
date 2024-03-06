using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MaterialBladesSecondSupportCardAbility : BaseSupport—ardAbility, ITurnCountable
{
    public event Action<ITurnCountable> OnReturnToNormal;

    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<Character> characters=new();
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("¬˚·ÂËÚÂ ‚‡ÊÂÒÍÓ„Ó ÔÂÒÓÌ‡Ê‡ ‰Îˇ ‡Ú‡ÍË", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("¬˚·ÂËÚÂ ‚‡ÊÂÒÍÓ„Ó ÔÂÒÓÌ‡Ê‡", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelected—haractersBehaviour(1f, battleSystem, "\"Ã‡ÚÂË‡Î¸Ì˚Â ÍÎËÌÍË\""));

        TurnCount = 1;
        m_isBuff = false;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.OnClick += SelectCharacter;
            }
        }
    }


    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            characters.Add(battleSystem.EnemyController.CurrentEnemyCharacter);
        }
        else
        {
            characters.Add(battleSystem.PlayerController.CurrentPlayerCharacter);
        }

        if (characters.Count>=2)
        {
            foreach (var character in characters)
            {
                character.IsFreezed = true;
                UseCard(character.gameObject);
            }        
        }
        else
        {
            SelectSecondCard();
        }      
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.OnClick += SelectCharacter;
            enemyCharacter.IsChosen = false;
            enemyCharacter.IsEnabled = true;
        }
    }
    public void ReturnToNormal()
    {
        foreach(var character in characters)
        {
            character.IsFreezed = false;
        }
        
    }
}
