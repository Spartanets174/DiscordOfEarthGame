using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    private bool m_isCurrentPlayerCharacter;
    public bool IsCurrentPlayerCharacter
    {
        get => m_isCurrentPlayerCharacter;
        set => m_isCurrentPlayerCharacter = value;
    }
    
    public override void SetData(CharacterCard card, Material material, int index)
    {
        base.SetData(card, material, index);
        m_index = index;
        healthBar.SetHealth(card.health, 1);
        healthBar.SetMaxHealth(card.health);
        m_card = card;
    }
}
