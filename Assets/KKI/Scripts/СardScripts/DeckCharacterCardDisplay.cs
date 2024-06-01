using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCharacterCardDisplay : OutlineClicableUI
{
    [Space, Header("Card Info")]
    [SerializeField]
    private Image characterImage;
    [SerializeField]
    private Image classImage;
    [SerializeField]
    private Image rarityImage;
    [SerializeField]
    private TextMeshProUGUI characterName;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color mythColor;


    [Space]
    [SerializeField]
    private Button deleteButton;

    [Space, Header("Class images")]
    [SerializeField]
    private Sprite warriorSprite;
    [SerializeField]
    private Sprite archerSprite;
    [SerializeField]
    private Sprite wizardSprite;
    [SerializeField]
    private Sprite �avalrySprite;

    
   
    public event Action<CharacterCard> onCharacterDelete;

    private CharacterCard currentCharacterCard;
    public CharacterCard CurrentCharacterCard => currentCharacterCard;
    private void Awake()
    {
        IsEnabled = true;
        deleteButton.onClick.AddListener(DeleteCard);
        OnDoubleClick += x => DeleteCard();
    }
    private void OnDestroy()
    {
        deleteButton.onClick.RemoveListener(DeleteCard);
        OnDoubleClick -= x => DeleteCard();
    }
    public void SetData(CharacterCard characterCard)
    {
        currentCharacterCard = characterCard;
        if (characterCard.rarity.ToString() == "�������")
        {
            rarityImage.color = normalColor;
        }
        else
        {
            rarityImage.color = mythColor;
        }
        characterImage.sprite = characterCard.image;
        characterName.text = characterCard.cardName;
        switch (characterCard.Class)
        {
            case Enums.Classes.�������:
                classImage.sprite = warriorSprite;
                break;
            case Enums.Classes.������:
                classImage.sprite = archerSprite;
                break;
            case Enums.Classes.���:
                classImage.sprite = wizardSprite;
                break;
            case Enums.Classes.���������:
                classImage.sprite = �avalrySprite;
                break;
        }

    }

    private void DeleteCard()
    {
        onCharacterDelete?.Invoke(currentCharacterCard);
        Destroy(gameObject);
    }
}
