using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DarkElfArcherCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private Enums.Classes classToDefence;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour(""));
        SetUseCardBehaviour(new EmptyUseAbilityBehaviour());

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        abilityOwner.CanBeDamagedByClassesDict[classToDefence] = false;
        abilityOwner.OnDamaged += OnDamaged;
        UseCard(abilityOwner.gameObject);
    }

    private void OnDamaged(Character character, string arg2, float arg3)
    {
        if (character.LastAttackedCharacter.Class == classToDefence)
        {
            abilityOwner.CanBeDamagedByClassesDict[classToDefence] = true;
            abilityOwner.OnDamaged -= OnDamaged;
        }       
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        
    }
}
[Serializable]
public class DarkElfArcherCharacterDefenceAbilityData : BaseCharacterAbilityData
{
    public Enums.Classes classToDefence;
}
