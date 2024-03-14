using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MaterialBladesSupportCardAbility : BaseSupport—ardAbility, ITurnCountable
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
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("¬˚·ÂËÚÂ ‚‡ÊÂÒÍÓ„Ó ÔÂÒÓÌ‡Ê‡ ‰Îˇ ‡Ú‡ÍË", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("¬˚·ÂËÚÂ ‚ÚÓÓ„Ó ‚‡ÊÂÒÍÓ„Ó ÔÂÒÓÌ‡Ê‡ ‰Îˇ ‡Ú‡ÍË", battleSystem));
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
        battleSystem.EnemyController.SetEnemiesState(true, x =>
        {
            x.OnClick += SelectCharacter;
            x.IsChosen = false;
        });
    }
    public void ReturnToNormal()
    {
        character.IsFreezed = false;
    }
}
