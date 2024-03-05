using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MaterialBladesSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    public event Action<ITurnCountable> OnReturnToNormal;

    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private Character character;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåğèòå âğàæåñêîãî ïåğñîíàæà äëÿ àòàêè", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelectedÑharactersBehaviour(1f, battleSystem, "\"Ìàòåğèàëüíûå êëèíêè\""));

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
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;           
        }

        character.IsFreezed = true;

        UseCard(character.gameObject);
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
        character.IsFreezed = false;
    }
}
