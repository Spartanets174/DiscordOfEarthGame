using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemyCharacter : Character
{
    public void SetData(CharacterCard card, Material material)
    {
        healthBar.SetMaxHealth(card.health);
        healthBar.SetHealth(card.health);
        GetComponent<MeshRenderer>().sharedMaterial = material;
        m_card = card;
    }
}
