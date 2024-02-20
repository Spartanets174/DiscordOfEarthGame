using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseSupportСardAbility: MonoBehaviour
{
    protected BattleSystem battleSystem;

    protected ICardSelectable cardSelectBehaviour;
    protected ICardUsable useCardBehaviour;
    protected ICharacterSelectable selectCharacterBehaviour;

    public event Action OnSupportCardAbilityUsed;

    protected void SetCardSelectBehaviour(ICardSelectable behaviour)
    {
        cardSelectBehaviour = behaviour;
    }
    protected void SetUseCardBehaviour(ICardUsable behaviour)
    {
        useCardBehaviour = behaviour;
    }
    protected void SetSelectCharacterBehaviour(ICharacterSelectable behaviour)
    {
        selectCharacterBehaviour = behaviour;
    }

    public virtual void SelectCard()
    {
        cardSelectBehaviour.SelectCard();
    }

    public virtual void UseCard(GameObject gameObject)
    {
        useCardBehaviour.UseAbility(gameObject);
    }

    public virtual void SelectCharacter(GameObject gameObject)
    {
        selectCharacterBehaviour.SelectCharacter(gameObject);
    }
    public void OnSupportCardAbilityUsedInvoke()
    {
        OnSupportCardAbilityUsed?.Invoke();
    }
    protected virtual void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

}
