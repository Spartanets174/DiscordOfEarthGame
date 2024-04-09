using TypeReferences;

public class BaseCharacterAbilityData
{
    [Inherits(typeof(BaseCharacterAbility), ShortName = true)]
    public TypeReference characterAbility;

    public string abilityName;
}
