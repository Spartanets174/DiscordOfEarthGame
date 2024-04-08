using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeasantWithPitchforkCharacterPassiveAbilityData : BasePassiveCharacterAbility
{
    private PeasantWithPitchforkCharacterPassiveAbility abilityData;
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        base.Init(battleSystem, owner);
        this.abilityData = (PeasantWithPitchforkCharacterPassiveAbility)baseAbilityData;
        AbilityStart(abilityOwner);

        abilityOwner.OnDeath += AbilityEnd;

    }
    public override void AbilityStart(Character character)
    {
        abilityOwner.IgnoreMoveCostTroughtSwamp = true;
    }

    public override void AbilityEnd(Character character)
    {
        abilityOwner.IgnoreMoveCostTroughtSwamp = false;

        abilityOwner.OnDeath -= AbilityEnd;
    }
}