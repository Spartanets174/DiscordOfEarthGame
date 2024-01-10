using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GameSupportСardAbility
{
    public event Action<GameSupportСardAbility> onCardSupportUsed;
    public abstract void UseAbility(BattleSystem battleSystem);
}
