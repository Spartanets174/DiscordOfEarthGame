public class VoidShooterCharacterPassiveAbility : BasePassiveCharacterAbility
{
    private VoidShooterCharacterPassiveAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        base.Init(battleSystem, owner);
        this.abilityData = (VoidShooterCharacterPassiveAbilityData)baseAbilityData;

        abilityOwner.OnDeath += AbilityEnd;

    }
    public override void AbilityStart(Character character)
    {

    }

    public override void AbilityEnd(Character character)
    {
    }
}