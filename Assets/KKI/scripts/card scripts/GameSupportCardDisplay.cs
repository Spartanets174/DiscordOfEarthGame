using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSupportCardDisplay : OutlineClicableUI
{
    [SerializeField]
    private Image supportImage;
    [SerializeField]
    private Image supportCardRarity;
    [SerializeField]
    private TextMeshProUGUI supportAbility;
    [SerializeField]
    private TextMeshProUGUI supportName;

    private CardSupport m_currentCardSupport;
    public CardSupport CurrentCardSupport => m_currentCardSupport;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetData(CardSupport cardSupport)
    {
        m_currentCardSupport = cardSupport;
        supportImage.sprite = cardSupport.image;
        if (cardSupport.rarity == enums.Rarity.Ìèôè÷åñêàÿ) 
        { 
            supportCardRarity.color = new Color(126, 0, 255);
        }
        else
        {
            supportCardRarity.color = Color.gray;
        }
        supportAbility.text = cardSupport.abilityText;
        supportName.name = cardSupport.characterName;
/*        cardSupport.ability.onCardSupportUsed += OnCardSupportUsed;
*/    }

    private void OnCardSupportUsed(GameSupportÑardAbility gameSupportÑard)
    {
        image.DOFade(0.5f,1);
    }
}
