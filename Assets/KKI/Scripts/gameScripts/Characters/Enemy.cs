using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private bool m_isCurrentEnemyCharacter;
    public bool IsCurrentEnemyCharacter
    {
        get => m_isCurrentEnemyCharacter;
        set => m_isCurrentEnemyCharacter = value;
    }
    public override void SetData(CharacterCard card, Material material, int currentIndex)
    {
        base.SetData(card, material, currentIndex);
        healthBar.SetMaxHealth(card.health);
        healthBar.SetHealth(card.health, 1);
        GetComponent<MeshRenderer>().sharedMaterial = material;
        m_card = card;
        m_index = currentIndex;
    }
}
