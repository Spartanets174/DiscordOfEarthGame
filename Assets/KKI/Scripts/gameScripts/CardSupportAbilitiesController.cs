using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class CardSupportAbilitiesController : MonoBehaviour, ILoadable
{
    [Header("Scripts")]
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private BattleSystem battleSystem;

    [Space, Header("Parents")]
    [SerializeField]
    private Transform gameSupportCardsParent;

    [Space, Header("Prefabs")]
    [SerializeField]
    private GameSupportCardDisplay gameSupportCardPrefab;

    [Space, Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI tipsText;

    [Space, Header("Game objects")]
    [SerializeField]
    private GameObject topBlocker;
    [SerializeField]
    private GameObject bottomBlocker;
    [SerializeField]
    private GameObject tipsTextParent;
    [SerializeField]
    private GameObject centerBlocker;

    private CompositeDisposable disposables = new();

    private List<GameSupportCardDisplay> m_gameSupportCards = new();
    public List<GameSupportCardDisplay> GameSupportCards => m_gameSupportCards;

    private ReactiveProperty<GameSupportCardDisplay> currentGameSupportCardDisplay = new();
    public ReactiveProperty<GameSupportCardDisplay> CurrentGameSupportCardDisplay => currentGameSupportCardDisplay;

    public void Init()
    {
        foreach (var SupportCard in playerController.PlayerDataController.DeckUserSupportCards)
        {
            GameSupportCardDisplay cardDisplay = Instantiate(gameSupportCardPrefab, Vector3.zero, Quaternion.identity, gameSupportCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;


            cardDisplay.SetData(SupportCard, battleSystem);
            cardDisplay.DragAndDropComponent.StartPos = cardDisplay.transform.localPosition;

            StartCoroutine(SetPosDelayed(cardDisplay.DragAndDropComponent));

            
            cardDisplay.IsEnabled = false;

            cardDisplay.DragAndDropComponent.OnBeginDragEvent += OnBeginDrag;
            cardDisplay.DragAndDropComponent.OnEndDragEvent += OnEndDrag;
            cardDisplay.DragAndDropComponent.OnDropEvent += OnDropEvent;


            if (cardDisplay.GameSupport—ardAbility != null)
            {
                cardDisplay.GameSupport—ardAbility.OnUsingCancel += OnUsingCancel;
                cardDisplay.GameSupport—ardAbility.OnSecondSupportCardAbilitySelected += OnSecondSupportCardAbilitySelected;
                cardDisplay.GameSupport—ardAbility.OnSupportCardAbilityCharacterSelected += OnSupportCardAbilityCharacterSelected;
                cardDisplay.GameSupport—ardAbility.OnSupportCardAbilityUsed += OnSupportCardAbilityUsed;
            }
            m_gameSupportCards.Add(cardDisplay);
        }

        foreach (var item in GameSupportCards)
        {
            item.DragAndDropComponent.OnDropEvent += battleSystem.OnSupportCardButton;
            item.DragAndDropComponent.OnDropEvent += (x)=> battleSystem.FieldController.TurnOnCells(); ;
        }

        currentGameSupportCardDisplay.Where(x => x != null).Subscribe(x =>
        {
            SetTipsText(x.GameSupport—ardAbility.CardSelectBehaviour.SelectCardTipText);
        }).AddTo(disposables);

        SetBlockersState(false);
        tipsTextParent.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var cardDisplay in m_gameSupportCards)
        {
            cardDisplay.DragAndDropComponent.OnBeginDragEvent += OnBeginDrag;
            cardDisplay.DragAndDropComponent.OnEndDragEvent += OnEndDrag;
            cardDisplay.DragAndDropComponent.OnDropEvent += OnDropEvent;


            if (cardDisplay.GameSupport—ardAbility != null)
            {
                cardDisplay.GameSupport—ardAbility.OnUsingCancel += OnUsingCancel;
                cardDisplay.GameSupport—ardAbility.OnSecondSupportCardAbilitySelected += OnSecondSupportCardAbilitySelected;
                cardDisplay.GameSupport—ardAbility.OnSupportCardAbilityCharacterSelected += OnSupportCardAbilityCharacterSelected;
                cardDisplay.GameSupport—ardAbility.OnSupportCardAbilityUsed += OnSupportCardAbilityUsed;
            }
        }

        disposables.Dispose();
        disposables.Clear();
    }

    private void OnSecondSupportCardAbilitySelected(ICardSelectable selectable)
    {
        SetTipsText($"{selectable.SelectCardTipText}");
    }

    private void OnUsingCancel(BaseSupport—ardAbility ability)
    {
        tipsTextParent.SetActive(false);
        SetTipsText("");
        currentGameSupportCardDisplay.Value.gameObject.SetActive(true);
    }

    private void OnSupportCardAbilityCharacterSelected(ICharacterSelectable uharacterSelectable)
    {
        SetTipsText($"{uharacterSelectable.SelectCharacterTipText}");
    }

    private void OnSupportCardAbilityUsed(ICardUsable usable)
    {
        GameSupportCards.Remove(currentGameSupportCardDisplay.Value);
        tipsTextParent.SetActive(false);
        SetBlockersState(false);
        SetTipsText("");

        foreach (var cardDisplay in GameSupportCards)
        {
            cardDisplay.DragAndDropComponent.StartPos = cardDisplay.DragAndDropComponent.transform.localPosition;
        }
    }
    private void OnDropEvent(GameObject gameObject)
    {
        foreach (var SupportCard in GameSupportCards)
        {
            SupportCard.IsEnabled = false;
        }
        gameObject.SetActive(false);
        SetBlockersState(false);
        currentGameSupportCardDisplay.Value = gameObject.GetComponent<GameSupportCardDisplay>();
    }
    private void OnBeginDrag(GameObject gameObject)
    {
        tipsTextParent.SetActive(true);
        SetBlockersState(true);
        SetTipsText("œÂÂÚ‡˘ËÚÂ Í‡ÚÛ ‚ Ó·Î‡ÒÚ¸");
    }

    private void OnEndDrag(GameObject gameObject)
    {
        tipsTextParent.SetActive(false);
        SetBlockersState(false);
        SetTipsText("");
    }

    private void SetTipsText(string message)
    {
        tipsText.text = message;
    }
    private void SetBlockersState(bool state)
    {
        topBlocker.SetActive(state);
        bottomBlocker.SetActive(state);
        centerBlocker.SetActive(state);
    }

    public void SetDragAllowToSupportCards(bool state)
    {
        foreach (var cardDisplay in GameSupportCards)
        {
            cardDisplay.DragAndDropComponent.IsAllowedToDrag = state;
        }
    }
    private IEnumerator SetPosDelayed(DragAndDropComponent dragAndDropComponent)
    {
        yield return new WaitForEndOfFrame();
        dragAndDropComponent.StartPos = dragAndDropComponent.transform.localPosition;
    }

}
