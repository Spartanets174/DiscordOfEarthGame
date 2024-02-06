using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    private KeyCode[] keyCodes = new KeyCode[5] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
    private bool m_isCurrentPlayerCharacter;
    public bool IsCurrentPlayerCharacter
    {
        get => m_isCurrentPlayerCharacter;
        set => m_isCurrentPlayerCharacter = value;
    }


    private void Update()
    {
        if (Input.GetKey(keyCodes[m_index]))
        {
            OnClickInvoke();
        }
    }
    
    public override void SetData(CharacterCard card, Material material, int index)
    {
        m_index = index;
        healthBar.SetHealth(card.health);
        healthBar.SetMaxHealth(card.health);
        m_card = card;
    }
}
