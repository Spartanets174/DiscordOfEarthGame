using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static enums;

public class DeckSupportCardDisplay : MonoBehaviour
{
    [Space, Header("Card Info")]
    [SerializeField]
    private Image supportImage;
    [SerializeField]
    private Image rarityImage;
    [SerializeField]
    private TextMeshProUGUI supportCardName;

    [Space]
    [SerializeField]
    private Button deleteButton;


    public event Action<CardSupport> onSupportCardDelete;

    private CardSupport currentCardSupport;
    private void Awake()
    {
        deleteButton.onClick.AddListener(DeleteCard);
    }
    private void OnDestroy()
    {
        deleteButton.onClick.RemoveListener(DeleteCard);
    }
    public void SetData(CardSupport cardSupport)
    {
        currentCardSupport = cardSupport;
        if (cardSupport.rarity.ToString() == "Обычная")
        {
            rarityImage.color = Color.gray;
        }
        else
        {
            rarityImage.color = new Color(126, 0, 255);
        }
        supportImage.sprite = cardSupport.image;
        supportCardName.text = cardSupport.name;
    }

    private void DeleteCard()
    {
        onSupportCardDelete?.Invoke(currentCardSupport);
        Destroy(gameObject);
    }
}
