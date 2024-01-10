using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPresenter : MonoBehaviour, ILoadable
{
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


    private List<GameCharacterCardDisplay> gameCharacterCards=new();
    private List<GameSupportCardDisplay> gameSupportCards = new();
    private BattleSystem battleSystem;

    public void Init()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        foreach (var Card in battleSystem.PlayerManager.deckUserCharCards)
        {
            GameCharacterCardDisplay cardDisplay = Instantiate(gameCharacterCardPrefab, Vector3.zero, Quaternion.identity, gameCharacterCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.SetData(Card);
            gameCharacterCards.Add(cardDisplay);
        }
        foreach (var SupportCard in battleSystem.PlayerManager.deckUserSupportCards)
        {
            GameSupportCardDisplay cardDisplay = Instantiate(gameSupportCardPrefab, Vector3.zero, Quaternion.identity, gameSupportCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.SetData(SupportCard);
            gameSupportCards.Add(cardDisplay);
        }
    }

    public void AddMessagToGameLog(string message)
    {
        gameLog.text.Insert(0, message + "\n");
    }

    public void SetPointsOfActionAnd—ube(float value)
    {
        pointsOfActionAnd—ube.text = value.ToString();
    }
}
