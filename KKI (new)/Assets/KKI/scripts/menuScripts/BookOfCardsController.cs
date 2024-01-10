using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookOfCardsController : CardController
{
    public List<CharacterCard> CharacterUserCards => m_PlayerManager.allUserCharCards;

    public List<CardSupport> SupportUserCards => m_PlayerManager.allUserSupportCards;

    public List<CharacterCard> CharacterDeckUserCards => m_PlayerManager.deckUserCharCards;

    public List<CardSupport> SupportDeckUserCards => m_PlayerManager.deckUserSupportCards;


    public bool CanEquipCharacterCard(CharacterCard card)
    {
        int count = 0;
        int max=5;
        for (int j = 0; j < m_PlayerManager.deckUserCharCards.Count; j++)
        {
            if (m_PlayerManager.deckUserCharCards[j].Class == card.Class)
            {
                count++;
            }
        }
        if (card.Class == enums.Classes.Маг || card.Class == enums.Classes.Лучник)
        {
            max = 2;
        }
        if (card.Class == enums.Classes.Кавалерия)
        {
            max = 1;
        }
        if (count < max&& m_PlayerManager.deckUserCharCards.Count<5)
        {
            m_PlayerManager.deckUserCharCards.Add(card);
            m_PlayerManager.allUserCharCards.Remove(card);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanEquipSupportCard(CardSupport card)
    {
        if (m_PlayerManager.deckUserSupportCards.Count < 7)
        {
            m_PlayerManager.deckUserSupportCards.Add(card);
            m_PlayerManager.allUserSupportCards.Remove(card);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (card is CharacterCard)
        {
            m_PlayerManager.deckUserCharCards.Remove((CharacterCard)card);
            m_PlayerManager.allUserCharCards.Add((CharacterCard)card);
        }
        if (card is CardSupport)
        {
            m_PlayerManager.deckUserSupportCards.Remove((CardSupport)card);
            m_PlayerManager.allUserSupportCards.Add((CardSupport)card);
        }
    }
}
