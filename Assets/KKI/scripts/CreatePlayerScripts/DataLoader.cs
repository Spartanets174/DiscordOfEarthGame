using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLoader : MonoBehaviour, ILoadable
{
    [SerializeField] DbManager DB;

    private PlayerData playerData;

    public event Action<string> OnPlayerDataRecieved;
    public event Action<bool> OnPlayerDataChecked;
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
           StartCoroutine(GetPlayerData(creditials[0], creditials[1]));
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

    public IEnumerator CreateNewPlayer(string Nick, string password)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (!DB.IsConnected) { OnPlayerDataRecieved?.Invoke("notConnected"); yield break; };
        if (!DB.InsertToPlayers(Nick, password, 10000)) { OnPlayerDataRecieved?.Invoke("wrongCreditials"); yield break; };

        
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
        playerData.allShopSupportCards.Clear();
        playerData.allUserCharCards.Clear();
        playerData.allUserSupportCards.Clear();

        List<CharacterCard> CharacterCards = Resources.LoadAll<CharacterCard>($"cards/characters").OrderBy(x => x.cardName).ToList();
        List<CardSupport> CardsSupport = Resources.LoadAll<CardSupport>($"cards/support").OrderBy(x => x.cardName).ToList();

        for (int i = 0; i < CardOfShopPlayer.Count; i++)
        {
            if (CardOfShopPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                CardOfShopPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = CharacterCards.Where(x => x.id == CardOfShopPlayer[i].id).FirstOrDefault();
            playerData.allShopCharCards.Add(card);
        }
        
        for (int i = 0; i < CardSupportOfShopPlayer.Count; i++)
        {
            CardSupport CardSupport = CardsSupport.Where(x => x.id == CardSupportOfShopPlayer[i].id).FirstOrDefault();
            playerData.allShopSupportCards.Add(CardSupport);
        }

        
        for (int i = 0; i < OwnedCardOfPlayer.Count; i++)
        {
            if (OwnedCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                OwnedCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = CharacterCards.Where(x => x.id == OwnedCardOfPlayer[i].id).FirstOrDefault();
            playerData.allUserCharCards.Add(card);
        }
        
        for (int i = 0; i < OwnedCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = CardsSupport.Where(x => x.id == OwnedCardSupportOfPlayer[i].id).FirstOrDefault();
            playerData.allUserSupportCards.Add(CardSupport);
        }

        OnPlayerDataRecieved?.Invoke("loginned");
        yield break;
    }

    public IEnumerator GetPlayerData(string Nick, string password)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (!DB.IsConnected) { OnPlayerDataRecieved?.Invoke("notConnected"); yield break; };
        if (!DB.IsPlayerExits(Nick, password)) { OnPlayerDataRecieved?.Invoke("wrongCreditials"); yield break; }; 

        playerData.Name = Nick;
        playerData.Password = password;
        SaveSystem.SavePlayer(playerData.Name, playerData.Password);

        playerData.PlayerId = DB.SelectIdPlayer(playerData.Name);
        playerData.money = DB.SelectBalancePlayer(playerData);

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

        List<CharacterCard> CharacterCards = Resources.LoadAll<CharacterCard>($"cards/characters").OrderBy(x => x.cardName).ToList();
        List<CardSupport> CardsSupport = Resources.LoadAll<CardSupport>($"cards/support").OrderBy(x => x.cardName).ToList();

        playerData.allShopCharCards.Clear();
        playerData.allShopSupportCards.Clear();
        playerData.allUserCharCards.Clear();
        playerData.allUserSupportCards.Clear();
        playerData.deckUserCharCards.Clear();
        playerData.deckUserSupportCards.Clear();

        for (int i = 0; i < CardOfShopPlayer.Count; i++)
        {
            if (CardOfShopPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                CardOfShopPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = CharacterCards.Where(x => x.id == CardOfShopPlayer[i].id).FirstOrDefault();
            playerData.allShopCharCards.Add(card);
        }
       
        for (int i = 0; i < CardSupportOfShopPlayer.Count; i++)
        {
            CardSupport CardSupport = CardsSupport.Where(x => x.id == CardSupportOfShopPlayer[i].id).FirstOrDefault();
            playerData.allShopSupportCards.Add(CardSupport);
        }
        
        for (int i = 0; i < OwnedCardOfPlayer.Count; i++)
        {
            if (OwnedCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                OwnedCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = CharacterCards.Where(x => x.id == OwnedCardOfPlayer[i].id).FirstOrDefault();
            playerData.allUserCharCards.Add(card);
        }
        
        for (int i = 0; i < OwnedCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = CardsSupport.Where(x => x.id == OwnedCardSupportOfPlayer[i].id).FirstOrDefault();
            playerData.allUserSupportCards.Add(CardSupport);
        }
       
        for (int i = 0; i < DeckCardOfPlayer.Count; i++)
        {
            if (DeckCardOfPlayer[i].cardName == "Бесстрашный \"Страж\"")
            {
                DeckCardOfPlayer[i].cardName = "Бесстрашный Страж";
            }
            CharacterCard card = CharacterCards.Where(x => x.id == DeckCardOfPlayer[i].id).FirstOrDefault();
            playerData.deckUserCharCards.Add(card);
        }
        
        for (int i = 0; i < DeckCardSupportOfPlayer.Count; i++)
        {
            CardSupport CardSupport = CardsSupport.Where(x => x.id == DeckCardSupportOfPlayer[i].id).FirstOrDefault();
            playerData.deckUserSupportCards.Add(CardSupport);
        }

        OnPlayerDataRecieved?.Invoke("loginned");
        yield break;
    }
}