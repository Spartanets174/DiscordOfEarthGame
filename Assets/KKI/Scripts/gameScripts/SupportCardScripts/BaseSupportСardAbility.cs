using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
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

    public event Action OnUsingCancel;

    CompositeDisposable disposables = new();

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

    public void OnUsingCancelInvoke()
    {
        OnUsingCancel?.Invoke();
    }

    protected virtual void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();

        OnSupportCardAbilityUsed += ActionOnCardUse;
        OnSupportCardAbilityCharacterSelected += ClearDisposables;
        OnUsingCancel += CancelUsingCard;
    }

    private void CancelUsingCard()
    {
        CardSelectBehaviour.CancelSelection();
        SetStateToNormal();
        ClearDisposables();
    }

    private void ActionOnCardUse(ICardUsable usable)
    {
        SetStateToNormal();
        ClearDisposables();
    }

    public void LockState()
    {        
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnCompleted();
        }
        Observable.EveryUpdate().Where(x => Input.GetKey(KeyCode.Escape)).Subscribe(x =>
        {
            Debug.Log("f");
            OnUsingCancelInvoke();
        }).AddTo(disposables);
    }

    private void SetStateToNormal()
    {
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnStarted();
            if (battleSystem.CurrentPlayerCharacter !=null)
            {
                battleSystem.CurrentPlayerCharacter.IsChosen = false;
            }
            
        }
        else
        {
            battleSystem.EnemyController.CurrentEnemyCharacter.IsChosen = false;
        }
    }

    private void ClearDisposables(ICharacterSelectable selectable)
    {
        ClearDisposables();
    }

    private void ClearDisposables()
    {
        disposables.Dispose();
        disposables.Clear();
    }

    private void OnDestroy()
    {
        ClearDisposables();
        OnSupportCardAbilityUsed -= ActionOnCardUse;
        OnSupportCardAbilityCharacterSelected -= ClearDisposables;
        OnUsingCancel -= CancelUsingCard;
    }


}
