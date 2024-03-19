using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameUICoordinator : MonoBehaviour, ILoadable
{
    [Header("Scripts")]
    [SerializeField]
    private BattleSystem battleSystem;

    [Space, Header("UI Controllers")]
    [SerializeField]
    private ChosenCharacterDeatilsDisplay chosenCharacterDeatilsDisplay;
    [SerializeField]
    private PlayerControllerPresenter playerControllerPresenter;
    [SerializeField]
    private CardSupportAbilitiesController cardSupportAbilitiesController;

    [Space, Header("Action UI")]
    [SerializeField]
    private Button endMoveButton;
    [SerializeField]
    private Button toMenuButton;
    [SerializeField]
    private Button setPlayerTurnButton;
    [SerializeField]
    private Button setEnemyTurnButton;
    [SerializeField]
    private Button setWinButton;
    [SerializeField]
    private Button setLostButton;
    [SerializeField]
    private Button toMenuButtonEndGame;

    [Space, Header("Info UI")]
    [SerializeField]
    private TextMeshProUGUI gameLog;
    [SerializeField]
    private TextMeshProUGUI pointsOfActionAndСube;
    [SerializeField]
    private TextMeshProUGUI endGameText;

    [Space, Header("GameObjects")]
    [SerializeField]
    private GameObject endGameInterface;
    [SerializeField]
    private GameObject DevButtons;
    [SerializeField]
    private GameObject gameInterface;

    private CompositeDisposable disposables = new();
    private float timer = 1f;
    public void Init()
    {
        cardSupportAbilitiesController.Init();
        playerControllerPresenter.Init();
        

        battleSystem.gameLogCurrentText.Subscribe(x =>
        {
            AddMessageToGameLog(x);
        }).AddTo(disposables);

        battleSystem.PointsOfAction.Subscribe(x =>
        {
            SetPointsOfActionAndСube(x);
        }).AddTo(disposables);

        battleSystem.CurrentChosenCharacter.Subscribe(x =>
        {
            SetChosenCharDeatils(x);
        });

        battleSystem.OnGameStarted += OnGameStarted;
        battleSystem.OnEnemyTurnStarted += OnEnemyTurnStart;
        battleSystem.OnPlayerTurnStarted += OnPlayerTurnStart;
        battleSystem.OnGameEnded += SetEndGame;

        DevButtons.SetActive(false);

        endMoveButton.onClick.AddListener(battleSystem.SetEnemyTurn);
        setPlayerTurnButton.onClick.AddListener(battleSystem.SetPlayerTurn);
        setEnemyTurnButton.onClick.AddListener(battleSystem.SetEnemyTurn);
        setWinButton.onClick.AddListener(battleSystem.SetWin);
        setLostButton.onClick.AddListener(battleSystem.SetLost);
        toMenuButton.onClick.AddListener(SceneController.ToMenu);
        toMenuButtonEndGame.onClick.AddListener(SceneController.ToMenu);

        Observable.EveryUpdate().Subscribe(x =>
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.V))
                {
                    DevButtons.SetActive(!DevButtons.activeSelf);
                    timer = 1f;
                    Debug.Log(DevButtons.activeSelf);
                }
            }
        }).AddTo(disposables);
    }

    private void OnGameStarted(Begin begin)
    {
        foreach (var gameCards in playerControllerPresenter.GameCharacterCardDisplays)
        {
            gameCards.OnClick += battleSystem.OnChooseCharacterButton;
        }
        begin.OnCharacterMoved += playerControllerPresenter.GetChosenCard;
        begin.OnCharacterChosen += playerControllerPresenter.SetChosenCard;
        begin.OnStateCompleted += OnBeginStateCompleted;
    }

    private void OnBeginStateCompleted(State state)
    {
        Begin begin = (Begin)state;
        foreach (var gameCards in playerControllerPresenter.GameCharacterCardDisplays)
        {
            gameCards.OnClick -= battleSystem.OnChooseCharacterButton;
        }
        begin.OnCharacterChosen -= playerControllerPresenter.SetChosenCard;
        begin.OnStateCompleted -= OnBeginStateCompleted;
        begin.OnCharacterMoved -= playerControllerPresenter.GetChosenCard;

        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnDamaged += LogCharacterDamage;
            playerCharacter.OnDeath += LogDeath;
        }

        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.OnDamaged += LogCharacterDamage;
            enemyCharacter.OnDeath += LogDeath;
        }
        foreach (var staticEnemyCharacter in battleSystem.EnemyController.StaticEnemyCharObjects)
        {
            staticEnemyCharacter.OnDamaged += LogCharacterDamage;
            staticEnemyCharacter.OnDeath += LogDeath;
        }
    }

    private void SetEndGame()
    {
        if (battleSystem.State is Won)
        {
            endGameText.text = $"Поздравляем с победой, {battleSystem.PlayerController.PlayerDataController.CharacterName}! В награду вы получаете 3000 валюты!";
        }
        else
        {
            endGameText.text =  $"Увы, {battleSystem.PlayerController.PlayerDataController.CharacterName}, но вы проиграли! Но не отчаивайтесь, за старания мы дарим вам 500 валюты!";
        }
        
        endGameInterface.SetActive(true);
        gameInterface.SetActive(false);
    }

    private void OnDestroy()
    {
        endMoveButton.onClick.RemoveListener(battleSystem.SetEnemyTurn);
        setPlayerTurnButton.onClick.RemoveListener(battleSystem.SetPlayerTurn);
        setEnemyTurnButton.onClick.RemoveListener(battleSystem.SetEnemyTurn);
        setWinButton.onClick.RemoveListener(battleSystem.SetWin);
        setLostButton.onClick.RemoveListener(battleSystem.SetLost);
        toMenuButton.onClick.RemoveListener(SceneController.ToMenu);
        toMenuButtonEndGame.onClick.RemoveListener(SceneController.ToMenu);

        disposables.Dispose();
        disposables.Clear();
    }

    private void SetChosenCharDeatils(Character character)
    {
        chosenCharacterDeatilsDisplay.SetData(character);
    }
    private void OnPlayerTurnStart(PlayerTurn playerTurn)
    {
        cardSupportAbilitiesController.SetDragAllowToSupportCards(true);
        AddMessageToGameLog("Ваш ход.");
        endMoveButton.interactable = true;
        foreach (var supportCard in cardSupportAbilitiesController.GameSupportCards)
        {
            supportCard.IsEnabled = true;
        }
        foreach (var Card in playerControllerPresenter.GameCharacterCardDisplays)
        {
            Card.IsEnabled = true;
        }
        foreach (var SupportCard in cardSupportAbilitiesController.GameSupportCards)
        {
            SupportCard.IsEnabled = true;
        }  
    }


    private void OnEnemyTurnStart(EnemyTurn enemyTurn)
    {
        endMoveButton.interactable = false;

        cardSupportAbilitiesController.SetDragAllowToSupportCards(false);
        AddMessageToGameLog("Враг планирует свой ход...");

        foreach (var supportCard in cardSupportAbilitiesController.GameSupportCards)
        {
            supportCard.IsEnabled = false;
        }
        foreach (var Card in playerControllerPresenter.GameCharacterCardDisplays)
        {
            Card.IsEnabled = false;
        }
    }
    private void LogDeath(Character character)
    {
        if (character is PlayerCharacter)
        {
            AddMessageToGameLog($"Союзный юнит {character.CharacterName} убит");
        }
        else
        {
            AddMessageToGameLog($"Вражеский юнит {character.name} убит");
        }
    }
    private void LogCharacterDamage(Character character, string enemyName, float finalDamage)
    {
        SetChosenCharDeatils(character);
        if (finalDamage > 0)
        {
            AddMessageToGameLog($"{enemyName} наносит  юниту {character.CharacterName} {finalDamage * 100:00.00} урона");
        }
        else
        {
            AddMessageToGameLog($"{character.CharacterName} избежал получения урона от {enemyName}");
        }
    }
    private void AddMessageToGameLog(string message)
    {
        gameLog.text = gameLog.text.Insert(0, message + "\n");
    }

    private void SetPointsOfActionAndСube(float value)
    {
        pointsOfActionAndСube.text = value.ToString();

    }
}
