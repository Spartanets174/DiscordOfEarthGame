using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCharacterCardDisplay : MonoBehaviour
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
    private Sprite ÒavalrySprite;

    
   
    public event Action<CharacterCard> onCharacterDelete;

    private CharacterCard currentCharacterCard;
    private void Awake()
    {
        deleteButton.onClick.AddListener(DeleteCard);
    }
    private void OnDestroy()
    {
        deleteButton.onClick.RemoveListener(DeleteCard);
    }
    public void SetData(CharacterCard characterCard)
    {
        currentCharacterCard = characterCard;
        if (characterCard.rarity.ToString() == "Œ·˚˜Ì‡ˇ")
        {
            rarityImage.color = Color.gray;
        }
        else
        {
            rarityImage.color = new Color(126, 0, 255);
        }
        characterImage.sprite = characterCard.image;
        characterName.text = characterCard.cardName;
        switch (characterCard.Class)
        {
            case enums.Classes.œ‡Î‡‰ËÌ:
                classImage.sprite = warriorSprite;
                break;
            case enums.Classes.ÀÛ˜ÌËÍ:
                classImage.sprite = archerSprite;
                break;
            case enums.Classes.Ã‡„:
                classImage.sprite = wizardSprite;
                break;
            case enums.Classes. ‡‚‡ÎÂËˇ:
                classImage.sprite = ÒavalrySprite;
                break;
        }

    }

    private void DeleteCard()
    {
        onCharacterDelete?.Invoke(currentCharacterCard);
        Destroy(gameObject);
    }
}
