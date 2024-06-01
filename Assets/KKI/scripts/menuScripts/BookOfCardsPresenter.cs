using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BookOfCardsPresenter : CardPresenter, ILoadable
{
    [SerializeField]
    protected Transform parentToSpawnDeckCharacterCards;
    [SerializeField]
    protected Transform parentToSpawnDeckSupportCards;

    [Space, Header("Prefabs")]
    [SerializeField]
    private CardDisplay characterCardObjectPrefab;
    [SerializeField]
    private CardSupportDisplay supportCardObjectPrefab;
    [SerializeField]
    private DeckCharacterCardDisplay deckCharacterCardObjectPrefab;
    [SerializeField]
    private DeckSupportCardDisplay deckSupportCardObjectPrefab;

    [Space, Header("Action UI")]
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Dropdown raceDropdown;
    [SerializeField]
    private List<ButtonClass> classButtons;
    [SerializeField]
    private List<ButtonSupportType> supportTypeButtons;
    [SerializeField]
    private Button addToDeckCharacterButton;
    [SerializeField]
    private Button addToDeckSupportButton;

    [Space, Header("UI")]
    [SerializeField]
    private ChosenCharacterCardDisplay characterCardDisplay;
    [SerializeField]
    private ChosenSupportCardDisplay supportCardDisplay;
    [SerializeField]
    private TextMeshProUGUI tooMuchCardCaption;

    private List<CardDisplay> characterCardObjects = new();
    private List<CardSupportDisplay> supportCardObjects = new();
    private BookOfCardsController bookOfCardsController;
    private Sequence currentSequence;

    private CompositeDisposable disposables = new();
    public void Init()
    {
        bookOfCardsController = FindObjectOfType<BookOfCardsController>();
        SpawnCharacterCards();
        SpawnSupportCards();
        foreach (var item in bookOfCardsController.CharacterDeckUserCards)
        {
            SpawnCharacterDeckCard(item);
        }
        foreach (var item in bookOfCardsController.SupportDeckUserCards)
        {
            SpawnSupportDeckCard(item);
        }
        List<string> stringRaces = Enum.GetNames(typeof(Enums.Races)).ToList();
        for (int i = 0; i < stringRaces.Count; i++)
        {
            if (stringRaces[i] == "Ҹ���������")
            {
                stringRaces[i] = "Ҹ���� �����";
            }
            if (stringRaces[i] == "������������������")
            {
                stringRaces[i] = "���������� ��������";
            }
        }
        
        foreach (var classButton in classButtons)
        {
            classButton.OnButtonClick += ClassButtonCLick;
        }
        foreach (var supportTypeButton in supportTypeButtons)
        {
            supportTypeButton.OnButtonClick += SupportTypeButtonCLick;
        }

        closeButton.onClick.AddListener(CloseBookOfCards);
        addToDeckCharacterButton.onClick.AddListener(AddToDeckCharacterCard);
        addToDeckSupportButton.onClick.AddListener(AddToDeckSupportCard);
        raceDropdown.AddOptions(stringRaces);
        raceDropdown.onValueChanged.AddListener(RaceDropdownCLick);

        characterCardDisplay.chosenCharCard.Subscribe(x =>
        {
            addToDeckCharacterButton.interactable = x != null;
        }).AddTo(disposables);

        supportCardDisplay.chosenCardSupport.Subscribe(x =>
        {
            addToDeckSupportButton.interactable = x != null;
        }).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables.Clear();
        disposables = new();
    }

    private void CloseBookOfCards()
    {
        gameObject.SetActive(false);
        foreach (var outlineInteractableObject in bookOfCardsController.OutlineInteractableObjects)
        {
            outlineInteractableObject.IsEnabled = true;
        }
    }

    private void OnEnable()
    {
        SpawnCharacterCards();
        SpawnSupportCards();
        tooMuchCardCaption.gameObject.SetActive(false);
    }

    private void AddToDeckCharacterCard()
    {
        if (characterCardDisplay.chosenCharCard.Value == null) return;

        if (bookOfCardsController.CanEquipCharacterCard(characterCardDisplay.chosenCharCard.Value))
        {
            SpawnCharacterCards();
            SpawnCharacterDeckCard(characterCardDisplay.chosenCharCard.Value);
            characterCardDisplay.chosenCharCard.Value = null;
        }
        else
        {
            if (bookOfCardsController.PlayerDataController.DeckUserCharCards.Count>=5)
            {
                tooMuchCardCaption.text = "� ������ ��� ������������ ���������� ���� (5)";
            }
            else
            {
                switch (characterCardDisplay.chosenCharCard.Value.Class)
                {
                    case Enums.Classes.������:
                        tooMuchCardCaption.text = "� ������ ��� �������� �������� (2)";
                        break;
                    case Enums.Classes.���:
                        tooMuchCardCaption.text = "� ������ ��� �������� ����� (2)";
                        break;
                    case Enums.Classes.���������:
                        tooMuchCardCaption.text = "� ������ ��� �������� ��������� (1)";
                        break;
                }
            }            
            ShowTooMuchCardCaption();
        }
    }

    private void AddToDeckSupportCard()
    {
        if (supportCardDisplay.chosenCardSupport.Value == null) return;

        if (bookOfCardsController.CanEquipSupportCard(supportCardDisplay.chosenCardSupport.Value))
        {
            SpawnSupportCards();
            SpawnSupportDeckCard(supportCardDisplay.chosenCardSupport.Value);
            supportCardDisplay.chosenCardSupport.Value = null;
        }
        else
        {
            tooMuchCardCaption.text = "� ��� ��� ������������ ���������� ���� ������ (7)";
            ShowTooMuchCardCaption();
        }
    }
    private void RaceDropdownCLick(int value)
    {
        bookOfCardsController.SetCurrentRace((Enums.Races)value);
        SpawnCharacterCards();
    }
    private void ClassButtonCLick(ButtonClass buttonClass)
    {
        foreach (var classButton in classButtons)
        {
            if (classButton != buttonClass)
            {
                classButton.IsEnabled = false;
            }

        }
        bookOfCardsController.SetCurrentClass(buttonClass.Class);
        SpawnCharacterCards();
    }
    private void SupportTypeButtonCLick(ButtonSupportType buttonSupportType)
    {
        foreach (var supportTypeButton in supportTypeButtons)
        {
            if (buttonSupportType != supportTypeButton)
            {
                supportTypeButton.IsEnabled = false;
            }
        }
        bookOfCardsController.SetCurrentTypeOfSupport(buttonSupportType.TypeOfSupport);
        SpawnSupportCards();
    }

    protected override void SpawnCharacterCards()
    {
        foreach (var characterCardObject in characterCardObjects)
        {
            Destroy(characterCardObject.gameObject);
        }
        characterCardObjects.Clear();

        List<CharacterCard> cards = bookOfCardsController.FilterCharacterCards(bookOfCardsController.CharacterUserCards);

        foreach (var card in cards)
        {
            CardDisplay cardObject = Instantiate(characterCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnCharacterCards);
            cardObject.transform.localPosition = Vector3.zero;
            cardObject.SetValues(card);

            cardObject.OnClick += characterCardDisplay.SetCharacterData;
            cardObject.OnDoubleClick += (x) => AddToDeckCharacterCard();


            cardObject.IsEnabled = true;
            characterCardObjects.Add(cardObject);
        }
    }



    protected override void SpawnSupportCards()
    {
        foreach (var supportCardObject in supportCardObjects)
        {
            Destroy(supportCardObject.gameObject);
        }
        supportCardObjects.Clear();

        List<CardSupport> cardsSupport = bookOfCardsController.FilterSupportCards(bookOfCardsController.SupportUserCards);


        foreach (var cardSupport in cardsSupport)
        {
            CardSupportDisplay cardSupportObject = Instantiate(supportCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnSupportCards);
            cardSupportObject.transform.localPosition = Vector3.zero;
            cardSupportObject.SetValues(cardSupport);
            cardSupportObject.OnClick += supportCardDisplay.SetSupportCardData;
            cardSupportObject.OnDoubleClick += (x) => AddToDeckSupportCard();
            cardSupportObject.IsEnabled = true;

            supportCardObjects.Add(cardSupportObject);
        }
    }

    private void SpawnCharacterDeckCard(CharacterCard card)
    {
        DeckCharacterCardDisplay deckCharacterCardDisplay = Instantiate(deckCharacterCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnDeckCharacterCards);
        deckCharacterCardDisplay.transform.localPosition = Vector3.zero;
        deckCharacterCardDisplay.SetData(card);
        deckCharacterCardDisplay.onCharacterDelete += DeleteDeckCharacterCard;
        deckCharacterCardDisplay.OnClick += characterCardDisplay.SetOnlyData;
    }

    private void SpawnSupportDeckCard(CardSupport supportCard)
    {
        DeckSupportCardDisplay deckSupportCardDisplay = Instantiate(deckSupportCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnDeckSupportCards);
        deckSupportCardDisplay.transform.localPosition = Vector3.zero;
        deckSupportCardDisplay.SetData(supportCard);
        deckSupportCardDisplay.onSupportCardDelete += DeleteDeckSupportCard;
        deckSupportCardDisplay.OnClick += supportCardDisplay.SetOnlyData;
    }

    private void DeleteDeckCharacterCard(CharacterCard card)
    {       
        bookOfCardsController.RemoveCardFromDeck(card);
        SpawnCharacterCards();
    }

    private void DeleteDeckSupportCard(CardSupport card)
    {      
        bookOfCardsController.RemoveCardFromDeck(card);
        SpawnSupportCards();
    }

    private void ShowTooMuchCardCaption()
    {
        tooMuchCardCaption.color = Color.red;
        tooMuchCardCaption.DOFade(1, 0f);

        currentSequence.Kill();

        tooMuchCardCaption.gameObject.SetActive(true);
        currentSequence = DOTween.Sequence();

        currentSequence.AppendInterval(2);
        currentSequence.Append(tooMuchCardCaption.DOFade(0, 2f))
        .OnComplete(() => {
            tooMuchCardCaption.color = Color.red;
            tooMuchCardCaption.gameObject.SetActive(false);
        });
        currentSequence.Play();

    }
}
