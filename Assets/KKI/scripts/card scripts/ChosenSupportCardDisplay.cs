using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChosenSupportCardDisplay : MonoBehaviour
{
    [SerializeField]
    private Image supportCardImage;
    [SerializeField]
    private Image supportCardRarity;
    [SerializeField]
    private TextMeshProUGUI supportCardRarityText;
    [SerializeField]
    private TextMeshProUGUI supportCardAbility;
    [SerializeField]
    private TextMeshProUGUI supportCardName;
    

    private CardSupport m_chosenCardSupport;
    public CardSupport ChosenCardSupport=>m_chosenCardSupport;

    public void SetSupportCardData(GameObject cardSupportObject)
    {
        CardSupport cardSupport = cardSupportObject.GetComponent<CardSupportDisplay>().CardSupport;
        m_chosenCardSupport = cardSupport;
        supportCardImage.sprite = cardSupport.image;
        if (cardSupport.rarity==Enums.Rarity.Мифическая)
        {
            supportCardRarity.color = new Color(126, 0, 255);
        }
        else
        {
            supportCardRarity.color = Color.gray;
        }
        supportCardRarityText.text = cardSupport.rarity.ToString();
        supportCardAbility.text = $"Способность: {cardSupport.abilityText}";
        supportCardName.text = cardSupport.cardName;
    }
}
