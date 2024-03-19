using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnemyCharacter : StaticEnemyCharacter
{
    // Start is called before the first frame update
    void Start()
    {
        IsEnabled = true;

        SetData(Card,GetComponent<MeshRenderer>().material,-1);
        OnDamaged += AttackAttackedCharacter;
    }

    private void AttackAttackedCharacter(Character character, string name, float damage)
    {
        LastAttackedCharacter.Damage(LastDamageAmount * 0.3f, name);
    }

    private void OnDestroy()
    {
        OnDamaged -= AttackAttackedCharacter;
    }
}
