using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChosenCharacterDeatilsDisplay : MonoBehaviour, ILoadable
{
    [Header("Scripts")]
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private BattleSystem battleSystem;

    [Header("Card data")]
    [SerializeField]
    private TextMeshProUGUI characterNameText;
    [SerializeField]
    private TextMeshProUGUI cardRaceText;

    [SerializeField]
    private HealthBar healthBar;
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Image cardClassImage;

    [Header("Card characteristics")]
    [SerializeField]
    private TextMeshProUGUI physAttackText;
    [SerializeField]
    private TextMeshProUGUI magAttackText;
    [SerializeField]
    private TextMeshProUGUI physDefenceText;   
    [SerializeField]
    private TextMeshProUGUI magDefenceText;

    [Header("Card abilites")]
    [SerializeField]
    private OutlineClicableUI attackAbilityButton;
    [SerializeField]
    private OutlineClicableUI defenceAbilityButton;
    [SerializeField]
    private OutlineClicableUI buffAbilityButton;

    [Header("Sprites")]
    [SerializeField]
    private Sprite warriorSprite;
    [SerializeField]
    private Sprite archerSprite;
    [SerializeField]
    private Sprite wizardSprite;
    [SerializeField]
    private Sprite сavalrySprite;

    [Space, Header("Text panel")]
    [SerializeField]
    private TextMeshProUGUI tipsText;
    [SerializeField]
    private GameObject tipsTextParent;

    public ReactiveProperty<Character> currentCharacter = new();

    public ReactiveProperty<Character> usingAbilityCharacter = new();

    private CompositeDisposable disposables = new();

    public event Action OnAbilitySelected;
    public event Action OnAbilityUsed;
    public event Action OnAbilityUsingCancel;
    public void Init()
    {
        foreach (var playerCharacter in playerController.PlayerCharactersObjects)
        {
            if (playerCharacter.PassiveCharacterAbility!=null)
            {
                playerCharacter.PassiveCharacterAbility.Init(battleSystem, playerCharacter);
            }
           

            playerCharacter.AttackCharacterAbility.OnCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.AttackCharacterAbility.OnSecondCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.AttackCharacterAbility.OnThirdCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.AttackCharacterAbility.OnCardAbilityCharacterSelected += OnCardAbilityCharacterSelected;
            playerCharacter.AttackCharacterAbility.OnCardAbilityUsed += OnCardAbilityUsed;
            playerCharacter.AttackCharacterAbility.OnUsingCancel += OnUsingCancel;
            playerCharacter.AttackCharacterAbility.Init(battleSystem, playerCharacter, playerCharacter.Card.attackCharacterAbilityData);

            playerCharacter.DefenceCharacterAbility.OnCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.DefenceCharacterAbility.OnSecondCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.DefenceCharacterAbility.OnThirdCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.DefenceCharacterAbility.OnCardAbilityCharacterSelected += OnCardAbilityCharacterSelected;
            playerCharacter.DefenceCharacterAbility.OnCardAbilityUsed += OnCardAbilityUsed;
            playerCharacter.DefenceCharacterAbility.OnUsingCancel += OnUsingCancel;
            playerCharacter.DefenceCharacterAbility.Init(battleSystem, playerCharacter, playerCharacter.Card.defenceCharacterAbilityData);

            playerCharacter.BuffCharacterAbility.OnCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.BuffCharacterAbility.OnSecondCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.BuffCharacterAbility.OnThirdCardAbilitySelected += OnCardAbilitySelected;
            playerCharacter.BuffCharacterAbility.OnCardAbilityCharacterSelected += OnCardAbilityCharacterSelected;
            playerCharacter.BuffCharacterAbility.OnCardAbilityUsed += OnCardAbilityUsed;
            playerCharacter.BuffCharacterAbility.OnUsingCancel += OnUsingCancel;
            playerCharacter.BuffCharacterAbility.Init(battleSystem, playerCharacter, playerCharacter.Card.buffCharacterAbilityData);
        }
    }
    private void OnDestroy()
    {
        disposables.Dispose();
        disposables.Clear();
    }
    private void OnCardAbilitySelected(ICardSelectable selectable)
    {
        SetAbilityButtonsState(false);
        tipsTextParent.SetActive(true);
        SetTipsText($"{selectable.SelectCardTipText}");       
        OnAbilitySelected?.Invoke();
    }

    private void OnCardAbilityCharacterSelected(ICharacterSelectable characterSelectable)
    {
        SetTipsText($"{characterSelectable.SelectCharacterTipText}");
        playerController.PlayerTurn.ClearDisposables();
    }
    private void OnCardAbilityUsed(BaseCharacterAbility ability)
    {
        StartCoroutine(OnCardAbilityUsedDelayed(ability));
    }

    private IEnumerator OnCardAbilityUsedDelayed(BaseCharacterAbility ability)
    {
        yield return new WaitForEndOfFrame();
        if (ability.TypeOfAbility == Enums.TypeOfAbility.attack)
        {
            attackAbilityButton.IsEnabled = false;
            usingAbilityCharacter.Value.IsAttackAbilityUsed = true;
        }
        if (ability.TypeOfAbility == Enums.TypeOfAbility.defence)
        {
            defenceAbilityButton.IsEnabled = false;
            usingAbilityCharacter.Value.IsDefenceAbilityUsed = true;
        }
        if (ability.TypeOfAbility == Enums.TypeOfAbility.buff)
        {
            buffAbilityButton.IsEnabled = false;
            usingAbilityCharacter.Value.IsBuffAbilityUsed = true;
        }

        SetRulesAbilityButtonsState(usingAbilityCharacter.Value);

        playerController.PlayerTurn.SetStateToNormal();
        battleSystem.FieldController.TurnOnCells();
        playerController.PlayerTurn.ClearDisposables();

        battleSystem.PointsOfAction.Value -= usingAbilityCharacter.Value.UseAbilityCost;

        tipsTextParent.SetActive(false);
        SetTipsText("");
        OnAbilityUsed?.Invoke();
        StopCoroutine(OnCardAbilityUsedDelayed(ability));
    }

    private void OnUsingCancel(BaseCharacterAbility ability)
    {       
        playerController.PlayerTurn.SetStateToNormal();
        battleSystem.FieldController.TurnOnCells();
        playerController.PlayerTurn.ClearDisposables();

        SetRulesAbilityButtonsState(usingAbilityCharacter.Value);
        if (ability is ITurnCountable turnCountable)
        {
            if (turnCountable.IsBuff)
            {
                battleSystem.PlayerTurnCountables.Remove(turnCountable);
            }
            else
            {
                battleSystem.EnemyTurnCountables.Remove(turnCountable);
            }
        }

        tipsTextParent.SetActive(false);
        SetTipsText("");
        OnAbilityUsingCancel?.Invoke();
    }

    public void SetData(Character character)
    {
        if (character == null) return;

        if (currentCharacter != null)
        {
            attackAbilityButton.OnClick -= UseAttackAbility;
            defenceAbilityButton.OnClick -= UseDefencebility;
            buffAbilityButton.OnClick -= UseBuffAbility;
        }

        currentCharacter.Value = character;

        characterNameText.text = currentCharacter.Value.CharacterName;
        healthBar.SetMaxHealth(currentCharacter.Value.Card.health);
        healthBar.SetHealth(currentCharacter.Value.Health);
        physAttackText.text = $"Физическая атака: {currentCharacter.Value.PhysAttack * 100}";
        magAttackText.text = $"Магическая атака: {currentCharacter.Value.MagAttack * 100}";
        physDefenceText.text = $"Физическая защита: {currentCharacter.Value.PhysDefence * 100}";
        magDefenceText.text = $"Магическая защита: {currentCharacter.Value.MagDefence * 100}";

        cardImage.sprite = currentCharacter.Value.Card.image;
        switch (currentCharacter.Value.Card.Class)
        {
            case Enums.Classes.Паладин:
                cardClassImage.sprite = warriorSprite;
                break;
            case Enums.Classes.Лучник:
                cardClassImage.sprite = archerSprite;
                break;
            case Enums.Classes.Маг:
                cardClassImage.sprite = wizardSprite;
                break;
            case Enums.Classes.Кавалерия:
                cardClassImage.sprite = сavalrySprite;
                break;
        }

        switch (currentCharacter.Value.Card.race)
        {
            case Enums.Races.Гномы:
                cardRaceText.text = "Г";
                break;
            case Enums.Races.Люди:
                cardRaceText.text = "Л";
                break;
            case Enums.Races.МагическиеСущества:
                cardRaceText.text = "МС";
                break;
            case Enums.Races.ТёмныеЭльфы:
                cardRaceText.text = "ТЭ";
                break;
            case Enums.Races.Эльфы:
                cardRaceText.text = "Э";
                break;
        }

        if (currentCharacter.Value is PlayerCharacter && battleSystem.State is PlayerTurn)
        {
            SetRulesAbilityButtonsState(currentCharacter.Value);
            attackAbilityButton.OnClick += UseAttackAbility;
            defenceAbilityButton.OnClick += UseDefencebility;
            buffAbilityButton.OnClick += UseBuffAbility;            
        }
        else
        {
            SetAbilityButtonsState(false);
        }
    }

    private void UseAttackAbility(GameObject gameObject)
    {
        usingAbilityCharacter.Value = currentCharacter.Value;
        battleSystem.OnAttackAbilityButton(currentCharacter.Value.gameObject);      
    }
    private void UseDefencebility(GameObject gameObject)
    {
        usingAbilityCharacter.Value = currentCharacter.Value;
        battleSystem.OnDefensiveAbilityButton(currentCharacter.Value.gameObject);       
    }
    private void UseBuffAbility(GameObject gameObject)
    {
        usingAbilityCharacter.Value = currentCharacter.Value;
        battleSystem.OnBuffAbilityButton(currentCharacter.Value.gameObject);        
    }

    public void SetRulesAbilityButtonsState(Character character)
    {
        if (currentCharacter.Value == null) return;
        if (!currentCharacter.Value.CanUseAbilities)
        {
            SetAbilityButtonsState(false);
            return;
        }
        attackAbilityButton.IsEnabled = !character.IsAttackAbilityUsed;
        defenceAbilityButton.IsEnabled = !character.IsDefenceAbilityUsed;
        buffAbilityButton.IsEnabled = !character.IsBuffAbilityUsed;
    }

    public void SetAbilityButtonsState(bool state)
    {
        if (currentCharacter.Value == null) return;
        if (!currentCharacter.Value.CanUseAbilities)
        {
            state = false;
        }
        attackAbilityButton.IsEnabled = state;
        defenceAbilityButton.IsEnabled = state;
        buffAbilityButton.IsEnabled = state;
    }
    private void SetTipsText(string message)
    {
        tipsText.text = message;
    }
}
