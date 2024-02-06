using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CardDisplay : OutlineClicableUI
{
    
    private CharacterCard m_card;
    public CharacterCard Card=> m_card;

    public TextMeshProUGUI cardName; 
    public TextMeshProUGUI health;
    public TextMeshProUGUI physAttack;
    public TextMeshProUGUI magAttack;
    public TextMeshProUGUI physDefence;
    public TextMeshProUGUI magDefence;
    public Image rarity;
    public Image image;

    public void SetValues(CharacterCard card)
    {
        IsEnabled = true;
        m_card = card;
        cardName.text = m_card.characterName;
        if (m_card.rarity.ToString() == "Обычная")
        {
            rarity.color = Color.gray;
        }
        else
        {
            rarity.color = new Color(126, 0, 255);
        }
        health.text = Convert.ToString(m_card.health * 100);
        physAttack.text = Convert.ToString(m_card.physAttack * 100);
        magAttack.text = Convert.ToString(m_card.magAttack * 100);
        physDefence.text = Convert.ToString(m_card.physDefence * 100);
        magDefence.text = Convert.ToString(m_card.magDefence * 100);
        image.sprite = m_card.image;
    }
}