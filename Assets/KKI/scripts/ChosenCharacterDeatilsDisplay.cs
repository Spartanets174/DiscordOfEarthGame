using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChosenCharacterDeatilsDisplay : MonoBehaviour
{
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

    

    [Header("Sprites")]
    [SerializeField]
    private Sprite warriorSprite;
    [SerializeField]
    private Sprite archerSprite;
    [SerializeField]
    private Sprite wizardSprite;
    [SerializeField]
    private Sprite сavalrySprite;

    public void SetData(Character character)
    {
        characterNameText.text = character.CharacterName;
        healthBar.SetMaxHealth(character.Card.health);
        healthBar.SetHealth(character.Health);
        physAttackText.text = $"Физическая атака: {character.Card.physAttack * 100}";
        magAttackText.text = $"Магическая атака: {character.Card.magAttack * 100}";
        physDefenceText.text = $"Физическая защита: {character.Card.physDefence * 100}";
        magDefenceText.text = $"Магическая защита: {character.Card.magDefence * 100}";

        cardImage.sprite = character.Card.image;
        switch (character.Card.Class)
        {
            case enums.Classes.Паладин:
                cardClassImage.sprite = warriorSprite;
                break;
            case enums.Classes.Лучник:
                cardClassImage.sprite = archerSprite;
                break;
            case enums.Classes.Маг:
                cardClassImage.sprite = wizardSprite;
                break;
            case enums.Classes.Кавалерия:
                cardClassImage.sprite = сavalrySprite;
                break;
        }

        switch (character.Card.race)
        {
            case enums.Races.Гномы:
                cardRaceText.text = "Г";
                break;
            case enums.Races.Люди:
                cardRaceText.text = "Л";
                break;
            case enums.Races.МагическиеСущества:
                cardRaceText.text = "МС";
                break;
            case enums.Races.ТёмныеЭльфы:
                cardRaceText.text = "ТЭ";
                break;
            case enums.Races.Эльфы:
                cardRaceText.text = "Э";
                break;
        }
    }
}
