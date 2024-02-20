using UnityEngine;
using TypeReferences;

[CreateAssetMenu(fileName = "New Card Support", menuName = "Card support")]
public class CardSupport : Card
{
    public enums.TypeOfSupport type;
    public string abilityText;

    [Inherits(typeof(BaseSupport�ardAbility), ShortName = true),SerializeField]
    private TypeReference m_gameSupport�ardAbility;
    public TypeReference GameSupport�ardAbility => m_gameSupport�ardAbility;
}
