using System.Collections.Generic;
using UnityEngine;

public class PlayerManager:MonoBehaviour, ILoadable
{
    public PlayerData playerData;
    public List<CharacterCard> allCharCards;
    public List<CardSupport> allSupportCards;
    public List<CharacterCard> allShopCharCards;
    public List<CardSupport> allShopSupportCards;
    public List<CharacterCard> allUserCharCards;
    public List<CardSupport> allUserSupportCards;
    public List<CharacterCard> deckUserCharCards;
    public List<CardSupport> deckUserSupportCards;
    public int money = 100000;
    public string Name;

    public void Init()
    {
        this.allShopCharCards = playerData.allShopCharCards;
        this.allShopSupportCards = playerData.allShopSupportCards;
        this.allUserCharCards = playerData.allUserCharCards;
        this.allUserSupportCards = playerData.allUserSupportCards;
        this.deckUserCharCards = playerData.deckUserCharCards;
        this.deckUserSupportCards = playerData.deckUserSupportCards;
        this.money = playerData.money;
        this.Name = playerData.Name;
        //playerData.allCharCards = this.allCharCards;
        //playerData.allSupportCards = this.allSupportCards;
        //playerData.allUserCharCards = this.allUserCharCards;
        //playerData.allUserSupportCards = this.allUserSupportCards;
        //playerData.deckUserCharCards = this.deckUserCharCards;
        //playerData.deckUserSupportCards = this.deckUserSupportCards;
        //playerData.money = this.money;
    }
    private void OnDestroy()
    {
        playerData.allShopCharCards = this.allShopCharCards;
        playerData.allShopSupportCards = this.allShopSupportCards;
        playerData.allUserCharCards = this.allUserCharCards;
        playerData.allUserSupportCards = this.allUserSupportCards;
        playerData.deckUserCharCards = this.deckUserCharCards;
        playerData.deckUserSupportCards = this.deckUserSupportCards;
        playerData.money = this.money;
    }

}