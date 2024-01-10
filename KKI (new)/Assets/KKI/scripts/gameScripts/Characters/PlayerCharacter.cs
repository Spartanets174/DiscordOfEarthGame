using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    private void Update()
    {
        if (Input.GetKey(keyCodes[m_index]))
        {
            OnCharacterClickedInvoke();
        }
    }
    private KeyCode[] keyCodes = new KeyCode[5] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

}
