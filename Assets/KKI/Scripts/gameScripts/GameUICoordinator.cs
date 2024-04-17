using System;
using TMPro;
using UniRx;
using Unity.VisualScripting;
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
    [SerializeField]
    private SettingsController settingsController;

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
    private TextMeshProUGUI pointsOfActionAnd�ube;
    [SerializeField]
    private TextMeshProUGUI endGameText;

    [Space, Header("GameObjects")]
    [SerializeField]
    private GameObject endGameInterface;
    [SerializeField]
    private GameObject DevButtons;
    [SerializeField]
    private GameObject gameInterface;

    [Space, Header("Colors")]
    [SerializeField]
    private Color enemyTextColor;
    [SerializeField]
    private Color playerTextColor;
    [SerializeField]
    private Color amountTextColor;
    [SerializeField]
    private Color changeTurnTextColor;

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
            SetPointsOfActionAnd�ube(x);
        }).AddTo(disposables);

        battleSystem.CurrentChosenCharacter.Subscribe(x =>
        {
            SetChosenCharDeatils(x);
        });

        battleSystem.OnGameStarted += OnGameStarted;
        battleSystem.OnEnemyTurnStarted += OnEnemyTurnStart;
        battleSystem.OnPlayerTurnStarted += OnPlayerTurnStart;
        battleSystem.OnGameEnded += SetEndGame;
        settingsController.OnPauseStateChanged += SetPausedPlayer;


        DevButtons.SetActive(false);

        endMoveButton.onClick.AddListener(battleSystem.SetEnemyTurn);
        setPlayerTurnButton.onClick.AddListener(battleSystem.SetPlayerTurn);
        setEnemyTurnButton.onClick.AddListener(battleSystem.SetEnemyTurn);
        setWinButton.onClick.AddListener(battleSystem.SetWin);
        setLostButton.onClick.AddListener(battleSystem.SetLost);
        toMenuButton.onClick.AddListener(settingsController.TogglePausedState);
        toMenuButtonEndGame.onClick.AddListener(SceneController.ToMenu);



        cardSupportAbilitiesController.OnSupportAbilitySelected += OnSupportAbilitySelected;
        cardSupportAbilitiesController.OnSupportAbilityUsed += OnSupportAbilityUsed;
        cardSupportAbilitiesController.OnSupportAbilityUsingCancel += OnSupportAbilityUsed;

        chosenCharacterDeatilsDisplay.OnAbilitySelected += OnAbilitySelected;
        chosenCharacterDeatilsDisplay.OnAbilityUsed += OnAbilityUsed;
        chosenCharacterDeatilsDisplay.OnAbilityUsingCancel += OnAbilityUsed;


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

    private void OnAbilityUsed()
    {
        cardSupportAbilitiesController.EnableSupportCards();
    }

    private void OnAbilitySelected()
    {
        cardSupportAbilitiesController.DisableSupportCards();
    }

    private void OnSupportAbilityUsed()
    {
        chosenCharacterDeatilsDisplay.SetRulesAbilityButtonsState(chosenCharacterDeatilsDisplay.currentCharacter.Value);
    }

    private void OnSupportAbilitySelected()
    {
        chosenCharacterDeatilsDisplay.SetAbilityButtonsState(false);
        chosenCharacterDeatilsDisplay.currentCharacter.Value = null;
        battleSystem.CurrentChosenCharacter.Value = null;
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
            playerCharacter.OnHeal += LogHeal;
        }

        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.OnDamaged += LogCharacterDamage;
            enemyCharacter.OnDeath += LogDeath;
            enemyCharacter.OnHeal += LogHeal;

        }
        foreach (var staticEnemyCharacter in battleSystem.EnemyController.StaticEnemyCharObjects)
        {
            staticEnemyCharacter.OnDamaged += LogCharacterDamage;
            staticEnemyCharacter.OnDeath += LogDeath;
            staticEnemyCharacter.OnHeal += LogHeal;

        }

        chosenCharacterDeatilsDisplay.Init();
    }


    private void SetEndGame()
    {
        if (battleSystem.State is Won)
        {
            endGameText.text = $"����������� � �������, {battleSystem.PlayerController.PlayerDataController.CharacterName}! � ������� �� ��������� 3000 ������!";
        }
        else
        {
            endGameText.text =  $"���, {battleSystem.PlayerController.PlayerDataController.CharacterName}, �� �� ���������! �� �� ������������, �� �������� �� ����� ��� 500 ������!";
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
        AddMessageToGameLog($"<color=#{changeTurnTextColor.ToHexString()}>��� ���</color>");
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

    private void SetPausedPlayer(bool state)
    {
        if (battleSystem.State is PlayerTurn playerTurn)
        {
            if (state)
            {
                playerTurn.ResetPlayer();
            }
            else
            {
                playerTurn.SetStateToNormal();
            }
        }
        if (battleSystem.State is EnemyTurn enemyTurn)
        {
            if (state)
            {
                battleSystem.EnemyController.StopTree();
            }
            else
            {
                battleSystem.EnemyController.RestartTree();
            }
        }
    }

    private void OnEnemyTurnStart(EnemyTurn enemyTurn)
    {
        endMoveButton.interactable = false;

        cardSupportAbilitiesController.DisableSupportCards();
        AddMessageToGameLog($"<color=#{changeTurnTextColor.ToHexString()}>��� ����������</color>");;

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
            AddMessageToGameLog($"<color=#{playerTextColor.ToHexString()}>������� ����</color> {character.CharacterName} ����");
        }
        else
        {
            AddMessageToGameLog($"<color=#{enemyTextColor.ToHexString()}>��������� ����</color> {character.CharacterName} ����");
        }
    }

    private void LogHeal(Character healedCharacter, string characterUsedHeal, float healAmount)
    {
        Color characterColor = healedCharacter is PlayerCharacter? playerTextColor: enemyTextColor;

        SetChosenCharDeatils(healedCharacter);
        AddMessageToGameLog($"<color=#{characterColor.ToHexString()}>{characterUsedHeal}</color> ��������������� ����� <color=#{characterColor.ToHexString()}>{healedCharacter.CharacterName}</color> <color=#{amountTextColor.ToHexString()}>{healAmount * 100:00.00}</color> ������ ��������");
    }
    private void LogCharacterDamage(Character character, string enemyName, float finalDamage)
    {
        SetChosenCharDeatils(character);
        Color characterColor = character is PlayerCharacter ? playerTextColor : enemyTextColor;
        Color secondCharacterColor;
        if (battleSystem.State is PlayerTurn)
        {
            secondCharacterColor = character is PlayerCharacter ? enemyTextColor : playerTextColor;
        }
        else
        {
            secondCharacterColor = enemyTextColor;
        }

        if (finalDamage > 0)
        {
            AddMessageToGameLog($"<color=#{secondCharacterColor.ToHexString()}>{enemyName}</color> ������� <color=#{amountTextColor.ToHexString()}>{finalDamage * 100:00.00}</color> ������ ����� ����� <color=#{characterColor.ToHexString()}>{character.CharacterName}</color>  ");
        }
        else
        {
            AddMessageToGameLog($"<color=#{characterColor.ToHexString()}>{character.CharacterName}</color> ������� ��������� ����� �� <color=#{secondCharacterColor.ToHexString()}>{enemyName}</color>");
        }
    }
    private void AddMessageToGameLog(string message)
    {
        gameLog.text = gameLog.text.Insert(0, message + "\n");
    }

    private void SetPointsOfActionAnd�ube(float value)
    {
        pointsOfActionAnd�ube.text = value.ToString();

    }
}
