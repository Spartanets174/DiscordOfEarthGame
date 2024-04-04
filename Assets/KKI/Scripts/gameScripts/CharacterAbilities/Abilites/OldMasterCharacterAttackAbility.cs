using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class OldMasterCharacterAttackAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private FormulaAttackSelectedÑharacterBehaviour formulaAttackSelectedÑharacterBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("Íàæìèòå íà ïåğñîíàæà â êğàñíîé êëåòêå äëÿ àòàêè", battleSystem, abilityOwner, range, "attack"));
        SetUseCardBehaviour(new FormulaAttackSelectedÑharacterBehaviour(damage, battleSystem, abilityOwner, "\"Âàì íå óáåæàòü\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;
        formulaAttackSelectedÑharacterBehaviour = (FormulaAttackSelectedÑharacterBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
            {
                enemyCharacter.OnClick += UseCard;
            }
        }
    }


    private void OnCardUse()
    {
        formulaAttackSelectedÑharacterBehaviour.attackedCharacter.IsFreezed = true;
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= UseCard;
        }
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }

    public void ReturnToNormal()
    {

        formulaAttackSelectedÑharacterBehaviour.attackedCharacter.IsFreezed = false;

        OnReturnToNormal?.Invoke(this);
    }
}