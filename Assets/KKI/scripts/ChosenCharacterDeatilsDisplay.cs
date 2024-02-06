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
    private Sprite �avalrySprite;

    public void SetData(Character character)
    {
        characterNameText.text = character.CharacterName;
        healthBar.SetMaxHealth(character.Card.health);
        healthBar.SetHealth(character.Health);
        physAttackText.text = $"���������� �����: {character.Card.physAttack * 100}";
        magAttackText.text = $"���������� �����: {character.Card.magAttack * 100}";
        physDefenceText.text = $"���������� ������: {character.Card.physDefence * 100}";
        magDefenceText.text = $"���������� ������: {character.Card.magDefence * 100}";

        cardImage.sprite = character.Card.image;
        switch (character.Card.Class)
        {
            case enums.Classes.�������:
                cardClassImage.sprite = warriorSprite;
                break;
            case enums.Classes.������:
                cardClassImage.sprite = archerSprite;
                break;
            case enums.Classes.���:
                cardClassImage.sprite = wizardSprite;
                break;
            case enums.Classes.���������:
                cardClassImage.sprite = �avalrySprite;
                break;
        }

        switch (character.Card.race)
        {
            case enums.Races.�����:
                cardRaceText.text = "�";
                break;
            case enums.Races.����:
                cardRaceText.text = "�";
                break;
            case enums.Races.������������������:
                cardRaceText.text = "��";
                break;
            case enums.Races.Ҹ���������:
                cardRaceText.text = "��";
                break;
            case enums.Races.�����:
                cardRaceText.text = "�";
                break;
        }
    }
}
