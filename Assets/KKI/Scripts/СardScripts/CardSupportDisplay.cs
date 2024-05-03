
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSupportDisplay : OutlineClicableUI
{
    private CardSupport m_cardSupport;
    public CardSupport CardSupport => m_cardSupport;


    public TextMeshProUGUI cardSupportName;
    
    public TextMeshProUGUI ability;
    public TextMeshProUGUI rarity;
    public Image image;
    public Image rarityImage;
    public Color normalColor;
    public Color mythColor;

    public void SetValues(CardSupport cardSupport)
    {
        if (cardSupport == null) return; 
        IsEnabled = true;
        m_cardSupport = cardSupport;
        cardSupportName.text = m_cardSupport.cardName;
        image.sprite = m_cardSupport.image;
        ability.text = m_cardSupport.abilityText;
        rarity.text = m_cardSupport.rarity.ToString();
        rarity.color = Color.white;
        if (m_cardSupport.rarity.ToString() == "Обычная")
        {
            rarityImage.color = normalColor;
        }
        else
        {
            rarityImage.color = mythColor;
        }
    }
}
