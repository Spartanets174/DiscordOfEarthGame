using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSupportCardDisplay : OutlineClicableUI
{
    [Space, Header("Card Info")]
    [SerializeField]
    private Image supportImage;
    [SerializeField]
    private Image rarityImage;
    [SerializeField]
    private TextMeshProUGUI supportCardName;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color mythColor;


    [Space]
    [SerializeField]
    private Button deleteButton;


    public event Action<CardSupport> onSupportCardDelete;

    private CardSupport currentCardSupport;
    public CardSupport CurrentCardSupport=> currentCardSupport;
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
    public void SetData(CardSupport cardSupport)
    {
        currentCardSupport = cardSupport;
        if (cardSupport.rarity.ToString() == "Обычная")
        {
            rarityImage.color = normalColor;
        }
        else
        {
            rarityImage.color = mythColor;
        }
        supportImage.sprite = cardSupport.image;
        supportCardName.text = cardSupport.cardName;
    }

    private void DeleteCard()
    {
        onSupportCardDelete?.Invoke(currentCardSupport);
        Destroy(gameObject);
    }
}
