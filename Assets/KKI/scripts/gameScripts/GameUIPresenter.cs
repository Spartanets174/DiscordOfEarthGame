using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPresenter : MonoBehaviour, ILoadable
{
    [Space, Header("Game data")]
    [SerializeField]
    private PlayerManager playerManager;
    public PlayerManager PlayerManager => playerManager;

    [Space, Header("Prefabs")]
    [SerializeField]
    private GameCharacterCardDisplay gameCharacterCardPrefab;
    [SerializeField]
    private GameSupportCardDisplay gameSupportCardPrefab;

    [Space, Header("Parents")]
    [SerializeField]
    private Transform gameCharacterCardsParent;
    [SerializeField]
    private Transform gameSupportCardsParent;

    [Space, Header("Action UI")]
    [SerializeField]
    private Button endMoveButton;
    [SerializeField]
    private Button toMenuButton;

    [Space, Header("Info UI")]
    [SerializeField]
    private TextMeshProUGUI gameLog;
    [SerializeField]
    private TextMeshProUGUI pointsOfActionAnd—ube;
    [SerializeField]
    private TextMeshProUGUI endGameText;
    [SerializeField]
    private GameObject gameInterface;
    [SerializeField]
    private GameObject endGameInterface;
    [SerializeField]
    private ChosenCharacterDeatilsDisplay chosenCharacterDeatilsDisplay;


    private List<GameCharacterCardDisplay> m_gameCharacterCards=new();
    public List<GameCharacterCardDisplay> GameCharacterCardDisplays => m_gameCharacterCards;

    private List<GameSupportCardDisplay> m_gameSupportCards = new();
    public List<GameSupportCardDisplay> GameSupportCards => m_gameSupportCards;


    public void Init()
    {
        foreach (var Card in playerManager.DeckUserCharCards)
        {
            GameCharacterCardDisplay cardDisplay = Instantiate(gameCharacterCardPrefab, Vector3.zero, Quaternion.identity, gameCharacterCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.SetData(Card);
            m_gameCharacterCards.Add(cardDisplay);
            cardDisplay.IsEnabled = true;
        }
        foreach (var SupportCard in playerManager.DeckUserSupportCards)
        {
            GameSupportCardDisplay cardDisplay = Instantiate(gameSupportCardPrefab, Vector3.zero, Quaternion.identity, gameSupportCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.SetData(SupportCard);
            m_gameSupportCards.Add(cardDisplay);
            cardDisplay.IsEnabled = false;
        }
    }

    public void SetChosenCharDeatils(Character character)
    {
        chosenCharacterDeatilsDisplay.SetData(character);

    }
    public void OnPlayerTurnStart()
    {
        endMoveButton.interactable = true;
        foreach (var supportCard in m_gameSupportCards)
        {
            supportCard.IsEnabled = true;
        }
        foreach (var Card in m_gameCharacterCards)
        {
            Card.IsEnabled = true;
        }
    }

    public void OnEnemyTurnStart()
    {
        endMoveButton.interactable = false;
        foreach (var supportCard in m_gameSupportCards)
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

    public void SetPointsOfActionAnd—ube(float value)
    {
        pointsOfActionAnd—ube.text = value.ToString();
    
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


}
