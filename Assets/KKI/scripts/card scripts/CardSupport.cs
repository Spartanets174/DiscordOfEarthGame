using UnityEngine;
using TypeReferences;

[CreateAssetMenu(fileName = "New Card Support", menuName = "Card support")]
public class CardSupport : Card
{
    public enums.TypeOfSupport type;
    public string abilityText;

    [Inherits(typeof(BaseSupportÑardAbility), ShortName = true),SerializeField]
    private TypeReference m_gameSupportÑardAbility;
    public TypeReference GameSupportÑardAbility => m_gameSupportÑardAbility;
}
