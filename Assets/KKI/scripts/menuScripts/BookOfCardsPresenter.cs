using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        List<string> stringRaces = Enum.GetNames(typeof(enums.Races)).ToList();
        for (int i = 0; i < stringRaces.Count; i++)
        {
            if (stringRaces[i] == "ТёмныеЭльфы")
            {
                stringRaces[i] = "Тёмные Эльфы";
            }
            if (stringRaces[i] == "МагическиеСущества")
            {
                stringRaces[i] = "Магические существа";
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
        addToDeckCharacterButton.onClick.AddListener(AddToDeckCharacterCard);
        addToDeckSupportButton.onClick.AddListener(AddToDeckSupportCard);
        raceDropdown.AddOptions(stringRaces);
        raceDropdown.onValueChanged.AddListener(RaceDropdownCLick);
    }

    private void OnEnable()
    {
        SpawnCharacterCards();
        SpawnSupportCards();
        tooMuchCardCaption.gameObject.SetActive(false);
    }

    private void AddToDeckCharacterCard()
    {
        if (bookOfCardsController.CanEquipCharacterCard(characterCardDisplay.ChosenCharCard))
        {
            SpawnCharacterCards();
            SpawnCharacterDeckCard(characterCardDisplay.ChosenCharCard);
        }
        else
        {
            switch (characterCardDisplay.ChosenCharCard.Class)
            {       
                case enums.Classes.Лучник:
                    tooMuchCardCaption.text = "В колоде уже максимум лучников (2)";
                    break;
                case enums.Classes.Маг:
                    tooMuchCardCaption.text = "В колоде уже максимум магов (2)";
                    break;
                case enums.Classes.Кавалерия:
                    tooMuchCardCaption.text = "В колоде уже максимум кавалерии (1)";
                    break;
                default:
                    tooMuchCardCaption.text = "В колоде уже максимальное количество карт (5)";
                    break;
            }
            StartCoroutine(ShowTooMuchCardCaption());
        }
    }

    private void AddToDeckSupportCard()
    {
        if (bookOfCardsController.CanEquipSupportCard(supportCardDisplay.ChosenCardSupport))
        {
            SpawnSupportCards();
            SpawnSupportDeckCard(supportCardDisplay.ChosenCardSupport);
        }
        else
        {
            tooMuchCardCaption.text = "У вас уже максимальное количество карт помощи (7)";
            StartCoroutine(ShowTooMuchCardCaption());
        }
    }
    private void RaceDropdownCLick(int value)
    {
        bookOfCardsController.SetCurrentRace((enums.Races)value);
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

            cardObject.Clicable.OnClick += characterCardDisplay.SetCharacterData;
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
            cardSupportObject.Clicable.OnClick += supportCardDisplay.SetSupportCardData;

            supportCardObjects.Add(cardSupportObject);
        }
    }

    private void SpawnCharacterDeckCard(CharacterCard card)
    {
        DeckCharacterCardDisplay deckCharacterCardDisplay = Instantiate(deckCharacterCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnDeckCharacterCards);
        deckCharacterCardDisplay.transform.localPosition = Vector3.zero;
        deckCharacterCardDisplay.SetData(card);
        deckCharacterCardDisplay.onCharacterDelete += DeleteDeckCharacterCard;
    }

    private void SpawnSupportDeckCard(CardSupport supportCard)
    {
        DeckSupportCardDisplay deckSupportCardDisplay = Instantiate(deckSupportCardObjectPrefab, Vector3.zero, Quaternion.identity, parentToSpawnDeckSupportCards);
        deckSupportCardDisplay.transform.localPosition = Vector3.zero;
        deckSupportCardDisplay.SetData(supportCard);
        deckSupportCardDisplay.onSupportCardDelete += DeleteDeckSupportCard;
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

    private IEnumerator ShowTooMuchCardCaption()
    {
        tooMuchCardCaption.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        currentSequence = DOTween.Sequence();
        currentSequence.id = 1;
        currentSequence.Append(tooMuchCardCaption.DOFade(0, 2f))
        .OnComplete(() => {
            tooMuchCardCaption.color = Color.red;
            tooMuchCardCaption.gameObject.SetActive(false);
            currentSequence.Kill();
        });
        currentSequence.Play();

    }
}
