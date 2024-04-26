using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLoader : MonoBehaviour, ILoadable
{
    [SerializeField] DbManager DB;

    private PlayerData playerData;

    public Action<string> OnPlayerDataRecieved;
    public Action<bool> OnPlayerDataChecked;
    public void Init()
    {
        playerData = DB.PlayerData;
        OnPlayerDataRecieved += LoadMenuScene;
    }
    private void OnDestroy()
    {
        OnPlayerDataRecieved -= LoadMenuScene;
    }

    public void CheckPlayerData()
    {
        string[] creditials = SaveSystem.LoadPlayer().Split(".");
        if (creditials.Length == 0)
        {
            OnPlayerDataChecked?.Invoke(false);
        }

        if (creditials[0] != "" && creditials[1] != "")
        {
            GetPlayerData(creditials[0], creditials[1]);
            OnPlayerDataChecked?.Invoke(true);
        }
        else
        {
            OnPlayerDataChecked?.Invoke(false);
        }
    }

    private void LoadMenuScene(string connectionAnwser)
    {
        if (connectionAnwser == "loginned")
        {
            SceneController.ToMenu();
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
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CreateNewPlayer(string Nick, string password)
    {
        if (!DB.IsConnected) { OnPlayerDataRecieved?.Invoke("notConnected"); return; };
        if (!DB.InsertToPlayers(Nick, password, 10000)) { OnPlayerDataRecieved?.Invoke("wrongCreditials"); return; };

        
        playerData.Name = Nick;
        playerData.Password = password;
        SaveSystem.SavePlayer(playerData.Name, playerData.Password);
        int id = DB.SelectIdPlayer(playerData.Name);

        playerData.money = 10000;
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

        OnPlayerDataRecieved?.Invoke("loginned");
    }

    public void GetPlayerData(string Nick, string password)
    {
        if (!DB.IsConnected) { OnPlayerDataRecieved?.Invoke("notConnected"); return; };
        if (!DB.IsPlayerExits(Nick, password)) { OnPlayerDataRecieved?.Invoke("wrongCreditials"); return; }; 

        playerData.Name = Nick;
        playerData.Password = password;
        SaveSystem.SavePlayer(playerData.Name, playerData.Password);

        playerData.PlayerId = DB.SelectIdPlayer(playerData.Name);
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
        List<CharacterCard> DeckCardOfPlayer = DB.SelectFromDeckCards(playerData);
        List<CardSupport> DeckCardSupportOfPlayer = DB.SelectFromDeckCardsSupport(playerData);

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
        playerData.deckUserCharCards.Clear();
        for (int i = 0; i < DeckCardOfPlayer.Count; i++)
        {
            if (DeckCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                DeckCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = Resources.Load<CharacterCard>($"cards/characters/{DeckCardOfPlayer[i].cardName}");
            playerData.deckUserCharCards.Add(card);
        }
        playerData.deckUserSupportCards.Clear();
        for (int i = 0; i < DeckCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = Resources.Load<CardSupport>($"cards/support/{DeckCardSupportOfPlayer[i].cardName}");
            playerData.deckUserSupportCards.Add(CardSupport);
        }

        OnPlayerDataRecieved?.Invoke("loginned");
    }
}