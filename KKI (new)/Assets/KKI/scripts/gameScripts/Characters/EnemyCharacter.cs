using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public override void SetData(CharacterCard card, Material material, int currentIndex)
    {
        healthBar.SetMaxHealth(card.health);
        healthBar.SetHealth(card.health);
        GetComponent<MeshRenderer>().sharedMaterial = material;
        m_card = card;
        m_index = currentIndex;
    }
}
