using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPresenter : MonoBehaviour, ILoadable
{
    [Header("Model")]
    [SerializeField]
    private PlayerController playerController;


    [Space, Header("UI Controllers")]
    [SerializeField]
    private ChosenCharacterDeatilsDisplay chosenCharacterDeatilsDisplay;

    [SerializeField]
    private CardSupportAbilitiesController m_cardSupportAbilitiesController;
    public CardSupportAbilitiesController CardSupportAbilitiesController => m_cardSupportAbilitiesController;

    [Space, Header("Prefabs")]
    [SerializeField]
    private GameCharacterCardDisplay gameCharacterCardPrefab;

    [Space, Header("Parents")]
    [SerializeField]
    private Transform gameCharacterCardsParent;


    [Space, Header("Action UI")]
    [SerializeField]
    private Button m_endMoveButton;
    public Button EndMoveButton=> m_endMoveButton;
    [SerializeField]
    private Button toMenuButton;

    [Space, Header("Info UI")]
    [SerializeField]
    private TextMeshProUGUI tipsText;
    [SerializeField]
    private TextMeshProUGUI gameLog;
    [SerializeField]
    private TextMeshProUGUI pointsOfActionAnd�ube;
    [SerializeField]
    private TextMeshProUGUI endGameText;

    [Space, Header("GameObjects")]
    [SerializeField]
    private GameObject tipsTextParent;
    [SerializeField]
    private GameObject centerBlocker;
    [SerializeField]
    private GameObject topBlocker;
    [SerializeField]
    private GameObject bottomBlocker;
    [SerializeField]
    private GameObject gameInterface;
    [SerializeField]
    private GameObject endGameInterface;


    private List<GameCharacterCardDisplay> m_gameCharacterCards=new();
    public List<GameCharacterCardDisplay> GameCharacterCardDisplays => m_gameCharacterCards;

    public List<GameSupportCardDisplay> GameSupportCards => CardSupportAbilitiesController.GameSupportCards;

    private ReactiveProperty<GameSupportCardDisplay> currentGameSupportCardDisplay = new();

    private CompositeDisposable disposables = new();
    public void Init()
    {
        SetBlockersState(false);
        tipsTextParent.SetActive(false);
        foreach (var Card in playerController.PlayerDataController.DeckUserCharCards)
        {
            GameCharacterCardDisplay cardDisplay = Instantiate(gameCharacterCardPrefab, Vector3.zero, Quaternion.identity, gameCharacterCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.SetData(Card);
            m_gameCharacterCards.Add(cardDisplay);
            cardDisplay.IsEnabled = true;
        }

        foreach (var cardDisplay in GameSupportCards)
        {
            cardDisplay.DragAndDropComponent.OnBeginDragEvent += OnBeginDrag;
            cardDisplay.DragAndDropComponent.OnEndDragEvent += OnEndDrag;
            cardDisplay.DragAndDropComponent.OnDropEvent += OnDropEvent;


            if (cardDisplay.GameSupport�ardAbility != null)
            {
                cardDisplay.GameSupport�ardAbility.OnUsingCancel += OnUsingCancel;
                cardDisplay.GameSupport�ardAbility.OnSecondSupportCardAbilitySelected += OnSecondSupportCardAbilitySelected;
                cardDisplay.GameSupport�ardAbility.OnSupportCardAbilityCharacterSelected += OnSupportCardAbilityCharacterSelected;
                cardDisplay.GameSupport�ardAbility.OnSupportCardAbilityUsed += OnSupportCardAbilityUsed;
            }

            currentGameSupportCardDisplay.Where(x => x != null).Subscribe(x =>
            {
                SetTipsText(x.GameSupport�ardAbility.CardSelectBehaviour.SelectCardTipText);
            }).AddTo(disposables);
        }
        
    }

    private void OnSecondSupportCardAbilitySelected(ICardSelectable selectable)
    {
        SetTipsText($"{selectable.SelectCardTipText}");
    }

    private void OnUsingCancel()
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



    public void SetDragAllowToSupportCards(bool state)
    {
        foreach (var cardDisplay in GameSupportCards)
        {
            cardDisplay.DragAndDropComponent.IsAllowedToDrag = state;
        }
    }

    private void OnDropEvent(GameObject gameObject)
    {
        gameObject.SetActive(false);
        SetBlockersState(false);
        currentGameSupportCardDisplay.Value = gameObject.GetComponent<GameSupportCardDisplay>();       
    }
    private void OnBeginDrag(GameObject gameObject)
    {
        tipsTextParent.SetActive(true);
        SetBlockersState(true);
        SetTipsText("���������� ����� � �������");
    }

    private void OnEndDrag(GameObject gameObject)
    {
        tipsTextParent.SetActive(false);
        SetBlockersState(false);
        SetTipsText("");
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables.Clear();
        foreach (var cardDisplay in GameSupportCards)
        {
            cardDisplay.DragAndDropComponent.OnBeginDragEvent -= OnBeginDrag;
            cardDisplay.DragAndDropComponent.OnEndDragEvent -= OnEndDrag;
            cardDisplay.DragAndDropComponent.OnDropEvent -= OnDropEvent;
            if (cardDisplay.GameSupport�ardAbility != null)
            {
                cardDisplay.GameSupport�ardAbility.OnSupportCardAbilityUsed -= OnSupportCardAbilityUsed;

            }
        }
    }

    public void SetChosenCharDeatils(Character character)
    {
        chosenCharacterDeatilsDisplay.SetData(character);
    }
    public void OnPlayerTurnStart()
    {
        SetDragAllowToSupportCards(true);
        AddMessageToGameLog("��� ���.");
        m_endMoveButton.interactable = true;
        foreach (var supportCard in GameSupportCards)
        {
            supportCard.IsEnabled = true;
        }
        foreach (var Card in m_gameCharacterCards)
        {
            Card.IsEnabled = true;
        }
        foreach (var SupportCard in GameSupportCards)
        {
            SupportCard.IsEnabled = true;
        }

    }

    public void OnEnemyTurnStart()
    {
        m_endMoveButton.interactable = false;

        SetDragAllowToSupportCards(false);
        AddMessageToGameLog("���� ��������� ���� ���...");

        foreach (var supportCard in GameSupportCards)
        {
            supportCard.IsEnabled = false;
        }
        foreach (var Card in m_gameCharacterCards)
        {
            Card.IsEnabled = false;
        }
    }

    public void AddMessageToGameLog(string message)
    {
        gameLog.text = gameLog.text.Insert(0, message + "\n");
    }

    public void SetTipsText(string message)
    {
        tipsText.text = message;
    }

    public void SetPointsOfActionAnd�ube(float value)
    {
        pointsOfActionAnd�ube.text = value.ToString();
    
    }
    public void SetChosenStateToCards(bool State)
    {
        foreach (var item in m_gameCharacterCards)
        {
            item.IsChosen = State;
        }
    }

    public void EbableUnspawnedCards()
    {
        foreach (var item in m_gameCharacterCards)
        {
            if (!item.IsCharacterSpawned)
            {
                item.IsEnabled = true;
            }
        }
    }

    public void SetChosenCard(GameCharacterCardDisplay cardDisplay)
    {
        foreach (var item in m_gameCharacterCards)
        {
            item.IsChosen = false;           
        }
        cardDisplay.IsChosen = true;
    }


    public GameCharacterCardDisplay GetChosenCard()
    {
        foreach (var item in m_gameCharacterCards)
        {
            if (item.IsChosen)
            {
                return item;
            }
        }
        return null;
    }

    private void SetBlockersState(bool state)
    {
        topBlocker.SetActive(state);
        bottomBlocker.SetActive(state);
        centerBlocker.SetActive(state);
    }

}
