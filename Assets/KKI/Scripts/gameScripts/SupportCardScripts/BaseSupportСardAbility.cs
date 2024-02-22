using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseSupportСardAbility: MonoBehaviour
{
    protected BattleSystem battleSystem;

    protected ICardSelectable m_cardSelectBehaviour;
    public ICardSelectable CardSelectBehaviour => m_cardSelectBehaviour;
    protected ICardUsable m_useCardBehaviour;
    public ICardUsable UseCardBehaviour => m_useCardBehaviour;
    protected ICharacterSelectable m_selectCharacterBehaviour;
    public ICharacterSelectable SelectCharacterBehaviour => m_selectCharacterBehaviour;

    public event Action<ICardSelectable> OnSupportCardAbilitySelected;
    public event Action<ICharacterSelectable> OnSupportCardAbilityCharacterSelected;
    public event Action<ICardUsable> OnSupportCardAbilityUsed;

    protected void SetCardSelectBehaviour(ICardSelectable behaviour)
    {
        m_cardSelectBehaviour = behaviour;
    }
    protected void SetUseCardBehaviour(ICardUsable behaviour)
    {
        m_useCardBehaviour = behaviour;
    }
    protected void SetSelectCharacterBehaviour(ICharacterSelectable behaviour)
    {
        m_selectCharacterBehaviour = behaviour;
    }

    public virtual void SelectCard()
    {
        m_cardSelectBehaviour.SelectCard();
    }

    public virtual void UseCard(GameObject gameObject)
    {
        m_useCardBehaviour.UseAbility(gameObject);
    }

    public virtual void SelectCharacter(GameObject gameObject)
    {
        m_selectCharacterBehaviour.SelectCharacter(gameObject);
    }

    public void OnSupportCardAbilitySelectedInvoke()
    {
        OnSupportCardAbilitySelected?.Invoke(m_cardSelectBehaviour);
    }

    public void OnSupportCardAbilityCharacterSelectedInvoke()
    {
        OnSupportCardAbilityCharacterSelected?.Invoke(m_selectCharacterBehaviour);
    }
    public void OnSupportCardAbilityUsedInvoke()
    {
        OnSupportCardAbilityUsed?.Invoke(m_useCardBehaviour);
    }
    
   

    protected virtual void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

}
