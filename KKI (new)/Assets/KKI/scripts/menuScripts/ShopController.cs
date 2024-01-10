using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController :CardController
{
    public List<CharacterCard> CharacterShopCards=> m_PlayerManager.allShopCharCards;

    public List<CardSupport> SupportShopCards => m_PlayerManager.allShopSupportCards;

    public bool CanBuyCharacterCard(CharacterCard card)
    {
        if (m_PlayerManager.money >= card.Price)
        {
            m_PlayerManager.allUserCharCards.Add(card);
            m_PlayerManager.allShopCharCards.Remove(card);

            m_PlayerManager.money -= card.Price;
            return true;
        }
        else
        {
            return false;
        }   
    }

    public bool CanBuySupportCard(CardSupport card)
    {
        if (m_PlayerManager.money >= card.Price)
        {
            m_PlayerManager.allUserSupportCards.Add(card);
            m_PlayerManager.allShopSupportCards.Remove(card);
            m_PlayerManager.money -= card.Price;
            return true;
        }
        else
        {
            return false;
        }
    }

}
