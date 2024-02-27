using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataLoader : MonoBehaviour, ILoadable
{
    [SerializeField] DbManager DB;

    private PlayerData playerData;
    public void Init()
    {
        playerData = DB.PlayerData;
        playerData.Name = saveSystem.LoadPlayer();
        playerData = DB.PlayerData;
        if (playerData.Name != "")
        {
            GetPlayerData();
        }
    }
    public bool IsNicknameInBase(string Nick)
    {
        bool hasName = false;
        List<string> nickList = DB.SelectFromPlayers();
        for (int i = 0; i < nickList.Count; i++)
        {
            if (nickList[i] == Nick)
            {
                hasName = true;
            }
        }
        if (nickList.Count == 0 || !hasName)
        {
            CreateNewPlayer(Nick);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateNewPlayer(string Nick)
    {
        int id = DB.InsertToPlayers(Nick, 1000);
        playerData.Name = Nick;
        saveSystem.savePlayer(playerData.Name);
        playerData.money = 1000;
        playerData.PlayerId = id;
        playerData.deckUserCharCards.Clear();
        playerData.deckUserSupportCards.Clear();
        List<CharacterCard> CardOfPlayer = DB.SelectFromChars();
        List<CardSupport> CardSupportOfPlayer = DB.SelectFromCardsSupport();

        playerData.allCharCards = playerData.allCharCards.OrderBy(x => x.cardName).ToList();
        CardOfPlayer = CardOfPlayer.OrderBy(x => x.cardName).ToList();
        playerData.allSupportCards = playerData.allSupportCards.OrderBy(x => x.cardName).ToList();
        CardSupportOfPlayer = CardSupportOfPlayer.OrderBy(x => x.cardName).ToList();

        for (int i = 0; i < playerData.allCharCards.Count; i++)
        {
            playerData.allCharCards[i].cardName = CardOfPlayer[i].cardName;
            playerData.allCharCards[i].race = CardOfPlayer[i].race;
            playerData.allCharCards[i].Class = CardOfPlayer[i].Class;
            playerData.allCharCards[i].rarity = CardOfPlayer[i].rarity;
            playerData.allCharCards[i].description = CardOfPlayer[i].description;
            playerData.allCharCards[i].health = CardOfPlayer[i].health;
            playerData.allCharCards[i].speed = CardOfPlayer[i].speed;
            playerData.allCharCards[i].physAttack = CardOfPlayer[i].physAttack;
            playerData.allCharCards[i].magAttack = CardOfPlayer[i].magAttack;
            playerData.allCharCards[i].range = CardOfPlayer[i].range;
            playerData.allCharCards[i].physDefence = CardOfPlayer[i].physDefence;
            playerData.allCharCards[i].magDefence = CardOfPlayer[i].magDefence;
            playerData.allCharCards[i].critNum = CardOfPlayer[i].critNum;
            playerData.allCharCards[i].passiveAbility = CardOfPlayer[i].passiveAbility;
            playerData.allCharCards[i].attackAbility = CardOfPlayer[i].attackAbility;
            playerData.allCharCards[i].defenceAbility = CardOfPlayer[i].defenceAbility;
            playerData.allCharCards[i].buffAbility = CardOfPlayer[i].buffAbility;
            playerData.allCharCards[i].image = CardOfPlayer[i].image;
            playerData.allCharCards[i].Price = CardOfPlayer[i].Price;
            playerData.allCharCards[i].id = CardOfPlayer[i].id;
        }

        
        for (int i = 0; i < playerData.allSupportCards.Count; i++)
        {
            playerData.allSupportCards[i].cardName = CardSupportOfPlayer[i].cardName;
            playerData.allSupportCards[i].race = CardSupportOfPlayer[i].race;
            playerData.allSupportCards[i].type = CardSupportOfPlayer[i].type;
            /*Debug.Log($"{playerData.allSupportCards[i].image},{CardSupportOfPlayer[i].image}");*/
            playerData.allSupportCards[i].image = CardSupportOfPlayer[i].image;
            playerData.allSupportCards[i].abilityText = CardSupportOfPlayer[i].abilityText;
            playerData.allSupportCards[i].rarity = CardSupportOfPlayer[i].rarity;
            playerData.allSupportCards[i].Price = CardSupportOfPlayer[i].Price;
            playerData.allSupportCards[i].id = CardSupportOfPlayer[i].id;
            /*Debug.Log($"{playerData.allSupportCards[i].image},{CardSupportOfPlayer[i].image}");*/
        }

        DB.InsertToCardsShopStart(playerData);
        DB.InsertToCardsSupportShopStart(playerData);
        DB.InsertToOwnCardStart(playerData);
        DB.InsertToOwnCardsSupportStart(playerData);

        List<CharacterCard> CardOfShopPlayer = DB.SelectFromCardsShop(playerData);
        List<CardSupport> CardSupportOfShopPlayer = DB.SelectFromCardsSupportShop(playerData);
        List<CharacterCard> OwnedCardOfPlayer = DB.SelectFromOwnCards(playerData);
        List<CardSupport> OwnedCardSupportOfPlayer = DB.SelectFromOwnCardsSupport(playerData);


        playerData.allShopCharCards.Clear();
        for (int i = 0; i < CardOfShopPlayer.Count; i++)
        {
            if (CardOfShopPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                CardOfShopPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = Resources.Load<CharacterCard>($"cards/characters/{CardOfShopPlayer[i].cardName}");
            playerData.allShopCharCards.Add(card);
        }
        playerData.allShopSupportCards.Clear();
        for (int i = 0; i < CardSupportOfShopPlayer.Count; i++)
        {
            CardSupport CardSupport = Resources.Load<CardSupport>($"cards/support/{CardSupportOfShopPlayer[i].cardName}");
            playerData.allShopSupportCards.Add(CardSupport);
        }

        playerData.allUserCharCards.Clear();
        for (int i = 0; i < OwnedCardOfPlayer.Count; i++)
        {
            if (OwnedCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                OwnedCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = Resources.Load<CharacterCard>($"cards/characters/{OwnedCardOfPlayer[i].cardName}");
            playerData.allUserCharCards.Add(card);
        }
        playerData.allUserSupportCards.Clear();
        for (int i = 0; i < OwnedCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = Resources.Load<CardSupport>($"cards/support/{OwnedCardSupportOfPlayer[i].cardName}");
            playerData.allUserSupportCards.Add(CardSupport);
        }
    }

    private void GetPlayerData()
    {
        playerData.money = DB.SelectBalancePlayer(playerData);
        playerData.PlayerId = DB.SelectIdPlayer(playerData.Name);
        playerData.deckUserCharCards.Clear();
        playerData.deckUserSupportCards.Clear();
        playerData.money = DB.SelectBalancePlayer(playerData);
        List<CharacterCard> CardOfPlayer = DB.SelectFromChars();
        List<CardSupport> CardSupportOfPlayer = DB.SelectFromCardsSupport();

        for (int i = 0; i < playerData.allCharCards.Count; i++)
        {
            playerData.allCharCards[i].cardName = CardOfPlayer[i].cardName;
            playerData.allCharCards[i].race = CardOfPlayer[i].race;
            playerData.allCharCards[i].Class = CardOfPlayer[i].Class;
            playerData.allCharCards[i].rarity = CardOfPlayer[i].rarity;
            playerData.allCharCards[i].description = CardOfPlayer[i].description;
            playerData.allCharCards[i].health = CardOfPlayer[i].health;
            playerData.allCharCards[i].speed = CardOfPlayer[i].speed;
            playerData.allCharCards[i].physAttack = CardOfPlayer[i].physAttack;
            playerData.allCharCards[i].magAttack = CardOfPlayer[i].magAttack;
            playerData.allCharCards[i].range = CardOfPlayer[i].range;
            playerData.allCharCards[i].physDefence = CardOfPlayer[i].physDefence;
            playerData.allCharCards[i].magDefence = CardOfPlayer[i].magDefence;
            playerData.allCharCards[i].critNum = CardOfPlayer[i].critNum;
            playerData.allCharCards[i].passiveAbility = CardOfPlayer[i].passiveAbility;
            playerData.allCharCards[i].attackAbility = CardOfPlayer[i].attackAbility;
            playerData.allCharCards[i].defenceAbility = CardOfPlayer[i].defenceAbility;
            playerData.allCharCards[i].buffAbility = CardOfPlayer[i].buffAbility;
            playerData.allCharCards[i].image = CardOfPlayer[i].image;
            playerData.allCharCards[i].Price = CardOfPlayer[i].Price;
            playerData.allCharCards[i].id = CardOfPlayer[i].id;
        }
        for (int i = 0; i < playerData.allSupportCards.Count; i++)
        {
            playerData.allSupportCards[i].cardName = CardSupportOfPlayer[i].cardName;
            playerData.allSupportCards[i].race = CardSupportOfPlayer[i].race;
            playerData.allSupportCards[i].type = CardSupportOfPlayer[i].type;
            playerData.allSupportCards[i].image = CardSupportOfPlayer[i].image;
            playerData.allSupportCards[i].abilityText = CardSupportOfPlayer[i].abilityText;
            playerData.allSupportCards[i].rarity = CardSupportOfPlayer[i].rarity;
            playerData.allSupportCards[i].Price = CardSupportOfPlayer[i].Price;
            playerData.allSupportCards[i].id = CardSupportOfPlayer[i].id;
        }
        List<CharacterCard> CardOfShopPlayer = DB.SelectFromCardsShop(playerData);
        List<CardSupport> CardSupportOfShopPlayer = DB.SelectFromCardsSupportShop(playerData);
        List<CharacterCard> OwnedCardOfPlayer = DB.SelectFromOwnCards(playerData);
        List<CardSupport> OwnedCardSupportOfPlayer = DB.SelectFromOwnCardsSupport(playerData);


        playerData.allShopCharCards.Clear();
        for (int i = 0; i < CardOfShopPlayer.Count; i++)
        {
            if (CardOfShopPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                CardOfShopPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = Resources.Load<CharacterCard>($"cards/characters/{CardOfShopPlayer[i].cardName}");
            playerData.allShopCharCards.Add(card);
        }
        playerData.allShopSupportCards.Clear();
        for (int i = 0; i < CardSupportOfShopPlayer.Count; i++)
        {
            CardSupport CardSupport = Resources.Load<CardSupport>($"cards/support/{CardSupportOfShopPlayer[i].cardName}");
            playerData.allShopSupportCards.Add(CardSupport);
        }

        playerData.allUserCharCards.Clear();
        for (int i = 0; i < OwnedCardOfPlayer.Count; i++)
        {
            if (OwnedCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                OwnedCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = Resources.Load<CharacterCard>($"cards/characters/{OwnedCardOfPlayer[i].cardName}");
            playerData.allUserCharCards.Add(card);
        }
        playerData.allUserSupportCards.Clear();
        for (int i = 0; i < OwnedCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = Resources.Load<CardSupport>($"cards/support/{OwnedCardSupportOfPlayer[i].cardName}");
            playerData.allUserSupportCards.Add(CardSupport);
        }

        SceneManager.LoadScene("menu");
    }

    private void testFunc()
    {
        DB.RemoveCardsSupportShop(playerData);
        DB.InsertToCardsSupportShop(playerData);

    }
}