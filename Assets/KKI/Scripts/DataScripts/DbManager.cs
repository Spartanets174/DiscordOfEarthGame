using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConnectionInfo
{
    public static ConnectionData ConnectionData { get; private set; } = new ConnectionData();

    public static void ChangeConncetionInfo(ConnectionData connectionData)
    {
        ConnectionData = connectionData;
    }
}
[Serializable]
public class ConnectionData
{
    public string ip = "127.0.0.1";
    public string port = "3306";
    public string uid = "root";
    public string pwd = "12345";
    public string database = "gamedb";

    public ConnectionData()
    {

    }

    public ConnectionData(string ip, string port, string uid, string pwd, string database)
    {
        this.ip = ip;
        this.port = port;
        this.uid = uid;
        this.pwd = pwd;
        this.database = database;

    }
}
public static class Hashing
{
    public static string ToSHA256(string s)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
        var sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        return sb.ToString();
    }
}

public class DbManager : MonoBehaviour
{
    public static string connectionString;

    public static MySqlConnection con;

    [SerializeField] private PlayerData playerData;
    [SerializeField] private ConnectionData connectionData;
    public PlayerData PlayerData => playerData;

    private bool m_isConnected;
    public bool IsConnected => m_isConnected;
    public void Awake()
    {
        OpenCon();
    }

    public void OpenCon()
    {
        ConnectionInfo.ChangeConncetionInfo(connectionData);
        connectionString = $"server = {ConnectionInfo.ConnectionData.ip}; port = {ConnectionInfo.ConnectionData.port}; uid = {ConnectionInfo.ConnectionData.uid}; pwd = {ConnectionInfo.ConnectionData.pwd}; Database = {ConnectionInfo.ConnectionData.database};";
        m_isConnected = true;
        con = new MySqlConnection(connectionString);
        try
        {
            con.Open();
            Debug.Log("opened");
        }
        catch (System.Exception ex)
        {
            m_isConnected = false;
            Debug.LogError(ex.Message);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayer();

        CloseCon();
    }

    public void SavePlayer()
    {
        if (SceneManager.GetActiveScene().name == "CreatePlayer") return;
        if (playerData != null)
        {
            UpdatePlayerBalance(playerData);

            RemoveCardsSupportShop(playerData);
            InsertToCardsSupportShop(playerData);

            RemoveCardsShop(playerData);
            InsertToCardsShop(playerData);

            RemoveCardsSupportOwn(playerData);
            InsertToCardsSupportOwn(playerData);

            RemoveCardsOwn(playerData);
            InsertToCardsOwn(playerData);

            RemoveCardsSupportDeck(playerData);
            InsertToCardsSupportDeck(playerData);

            RemoveCardsDeck(playerData);
            InsertToCardDeck(playerData);
        }
    }
    public void CloseCon()
    {
        con.Close();
        Debug.Log("closed");
    }

    #region Player
    public bool InsertToPlayers(string Name, string password, int balance)
    {
        //'{Name}',{balance}, '{password}'
        password = Hashing.ToSHA256(password);
        string query = $"insert into gamedb.players (p_name, balance, p_password) values (@name, @balance, @password)";

        var command = new MySqlCommand(query, con);
        command.Prepare();

        try
        {
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@balance", balance);
            command.Parameters.AddWithValue("@password", password);

            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            command.Dispose();
            return false;
        }
        command.Dispose();
        return true;
    }

    public bool IsPlayerExits(string nick, string password)
    {
        string query = $"select* from gamedb.players where p_name = @name and p_password = @password";
        MySqlCommand command = new MySqlCommand(query, con);
        command.Prepare();
        password = Hashing.ToSHA256(password);
        try
        {
            command.Parameters.AddWithValue("@name", nick);
            command.Parameters.AddWithValue("@password", password);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                PlayerCredentialsData data = new(reader.GetString("p_name"), reader.GetString("p_password"));
                command.Dispose();
                if (data.PlayerName != "" && data.PlayerPassword != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                command.Dispose();
                return false;
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
            return false;
        }
    }

    public List<string> SelectFromPlayers()
    {
        string query = $"select p_name from gamedb.players";
        List<string> nickList = new List<string>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    nickList.Add(reader.GetString("p_name"));
                }
                /* reader.Read();*/
                /*Debug.Log(reader.GetString("name"));*/
                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return nickList;
    }

    public int UpdatePlayerBalance(PlayerData playerData)
    {
        string query = $"UPDATE gamedb.players SET balance = {playerData.money} where p_name='{playerData.Name}'";

        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();
        return Convert.ToInt32(command.LastInsertedId);
    }

    public int SelectBalancePlayer(PlayerData playerData)
    {
        int balance = -1;
        string query = $"select balance from gamedb.players where p_name='{playerData.Name}'";
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                balance = reader.GetInt32("balance");
                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return balance;
    }

    public int SelectIdPlayer(string playerName)
    {
        string query = $"select idPlayers from gamedb.players where gamedb.players.p_name = '{playerName}'";
        MySqlCommand command = new MySqlCommand(query, con);
        int id = -1;
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                id = reader.GetInt32("idPlayers");
                reader.Read();
                /* Debug.Log(reader.GetInt32("idPlayers"));*/
                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return id;
    }
    #endregion

    #region AllChars
    public void IsertIntoChars(PlayerData playerData)
    {

        for (int i = 0; i < playerData.allCharCards.Count; i++)
        {

            string critChance = playerData.allCharCards[i].critChance.ToString();
            string crit = playerData.allCharCards[i].critNum.ToString();
            if (crit != "0")
            {
                crit = crit.Replace(",", ".");
            }
            if (critChance != "0")
            {
                critChance = critChance.Replace(",", ".");
            }
            string query = $"insert into gamedb.characters(char_name, race, class, rarity, desctiption, health, speed ,p_attack,m_attack,char_range,p_defence,m_defence,crit_possibility,crit,passive,special_1,special_2,special_3,path) values('{playerData.allCharCards[i].cardName}','{playerData.allCharCards[i].race}','{playerData.allCharCards[i].Class}','{playerData.allCharCards[i].rarity}','{playerData.allCharCards[i].description}',{playerData.allCharCards[i].health},{playerData.allCharCards[i].speed},{playerData.allCharCards[i].physAttack},{playerData.allCharCards[i].magAttack},{playerData.allCharCards[i].range},{playerData.allCharCards[i].physDefence},{playerData.allCharCards[i].magDefence},{critChance},{crit},'{playerData.allCharCards[i].passiveAbility}','{playerData.allCharCards[i].attackAbility}','{playerData.allCharCards[i].defenceAbility}','{playerData.allCharCards[i].buffAbility}','{playerData.allCharCards[i].image.name}')";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }

    public List<CharacterCard> SelectFromChars()
    {
        string query = $"SELECT * FROM gamedb.characters order by char_name";
        List<CharacterCard> cardList = new List<CharacterCard>();
        MySqlCommand command = new MySqlCommand(query, con);

        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CharacterCard item = CreateCharacterCard(reader);
                    cardList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return cardList;
    }
    #endregion

    #region AllSupport
    public void InsertIntoCardsSupport(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allSupportCards.Count; i++)
        {
            string query = $"insert into gamedb.cards(race,name,effect,type,rarity,path, price) values('{playerData.allSupportCards[i].race}','{playerData.allSupportCards[i].cardName}','{playerData.allSupportCards[i].abilityText}','{playerData.allSupportCards[i].type}','{playerData.allSupportCards[i].rarity}','{playerData.allSupportCards[i].cardName}',{playerData.allSupportCards[i].Price})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public List<CardSupport> SelectFromCardsSupport()
    {
        string query = $"select * from gamedb.cards ORDER BY name";
        List<CardSupport> CardSupportList = new List<CardSupport>();
        MySqlCommand command = new MySqlCommand(query, con);

        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CardSupport item = CreateCardSupport(reader);
                    CardSupportList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return CardSupportList;
    }



    #endregion

    #region SupportShop

    public void InsertToCardsSupportShop(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allShopSupportCards.Count; i++)
        {
            if (playerData.allShopSupportCards[i] == null) continue;
            string query = $"insert into gamedb.cards_shop(idCards_Shop,cost,id_player) values({playerData.allShopSupportCards[i].id},{playerData.allShopSupportCards[i].Price},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsSupportShop(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.cards_shop WHERE id_player = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }


    public void InsertToCardsSupportShopStart(PlayerData playerData)
    {

        for (int i = 0; i < playerData.allSupportCards.Count; i++)
        {
            if (!playerData.startUserSupportCards.Contains(playerData.allSupportCards[i]))
            {
                string query = $"insert into gamedb.cards_shop(idCards_Shop,cost,id_player) values({playerData.allSupportCards[i].id},{playerData.allSupportCards[i].Price},{playerData.PlayerId})";
                var command = new MySqlCommand(query, con);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                command.Dispose();
            }
        }
    }

    public List<CardSupport> SelectFromCardsSupportShop(PlayerData playerData)
    {
        string query = $"SELECT idCards FROM gamedb.cards as c inner join gamedb.cards_shop as cs on(c.idCards = cs.idCards_Shop) where cs.id_player = {playerData.PlayerId}";
        List<CardSupport> CardSupportList = new List<CardSupport>();
        /*List<int> CardSupportids = new();*/
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CardSupport item = CreateCardSupport(reader);
                    CardSupportList.Add(item);
                    /*CardSupportids.Add(reader.GetInt32("idCards"));*/
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return CardSupportList;

    }
    #endregion

    #region CharShop

    public void InsertToCardsShop(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allShopCharCards.Count; i++)
        {
            string query = $"insert into gamedb.characters_shop(idCharacters_Shop,cost,id_payer) values({playerData.allShopCharCards[i].id},{playerData.allShopCharCards[i].Price},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsShop(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.characters_shop WHERE id_payer = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }

    public void InsertToCardsShopStart(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allCharCards.Count; i++)
        {
            if (!playerData.startUserCharCards.Contains(playerData.allCharCards[i]))
            {
                string query = $"insert into gamedb.characters_shop(idCharacters_Shop,cost,id_payer) values({playerData.allCharCards[i].id},{playerData.allCharCards[i].Price},{playerData.PlayerId})";
                var command = new MySqlCommand(query, con);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                command.Dispose();
            }
        }
    }

    public List<CharacterCard> SelectFromCardsShop(PlayerData playerData)
    {
        string query = $"SELECT * FROM gamedb.characters as c inner join gamedb.characters_shop as cs on(c.idCharacters = cs.idCharacters_Shop) where cs.id_payer = {playerData.PlayerId}";
        List<CharacterCard> cardList = new List<CharacterCard>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CharacterCard item = CreateCharacterCard(reader);
                    cardList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return cardList;

    }
    #endregion

    #region SupportOwn


    public void InsertToCardsSupportOwn(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allUserSupportCards.Count; i++)
        {
            string query = $"insert into gamedb.owned_cards(card_id,player_id) values({playerData.allUserSupportCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsSupportOwn(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.owned_cards WHERE player_id = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }



    public void InsertToOwnCardsSupportStart(PlayerData playerData)
    {
        for (int i = 0; i < playerData.startUserSupportCards.Count; i++)
        {
            string query = $"insert into gamedb.owned_cards(card_id,player_id) values({playerData.startUserSupportCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }

    public List<CardSupport> SelectFromOwnCardsSupport(PlayerData playerData)
    {
        string query = $"SELECT * FROM gamedb.cards as c inner join gamedb.owned_cards as cs on(c.idCards = cs.card_id) where cs.player_id = {playerData.PlayerId}";
        List<CardSupport> CardSupportList = new List<CardSupport>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CardSupport item = CreateCardSupport(reader);
                    CardSupportList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return CardSupportList;

    }

    #endregion

    #region CharOwn


    public void InsertToCardsOwn(PlayerData playerData)
    {
        for (int i = 0; i < playerData.allUserCharCards.Count; i++)
        {
            string query = $"insert into gamedb.owned_characters(character_id,playerId) values({playerData.allUserCharCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsOwn(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.owned_characters WHERE playerId = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }




    public void InsertToOwnCardStart(PlayerData playerData)
    {
        for (int i = 0; i < playerData.startUserCharCards.Count; i++)
        {
            string query = $"insert into gamedb.owned_characters(character_id,playerId) values({playerData.startUserCharCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }

    public List<CharacterCard> SelectFromOwnCards(PlayerData playerData)
    {
        string query = $"SELECT * FROM gamedb.characters as c inner join gamedb.owned_characters as cs on(c.idCharacters = cs.character_id) where cs.playerId = {playerData.PlayerId}";
        List<CharacterCard> cardList = new List<CharacterCard>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CharacterCard item = CreateCharacterCard(reader);
                    cardList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return cardList;

    }

    #endregion

    #region SupportDeck
    public void InsertToCardsSupportDeck(PlayerData playerData)
    {
        for (int i = 0; i < playerData.deckUserSupportCards.Count; i++)
        {
            string query = $"insert into gamedb.deck_cards(IdCard,IdPlayer) values({playerData.deckUserSupportCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsSupportDeck(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.deck_cards WHERE IdPlayer = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }

    public List<CardSupport> SelectFromDeckCardsSupport(PlayerData playerData)
    {
        string query = $"SELECT * FROM gamedb.cards as c inner join gamedb.deck_cards as cs on(c.idCards = cs.IdCard) where cs.IdPlayer = {playerData.PlayerId}";
        List<CardSupport> CardSupportList = new List<CardSupport>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CardSupport item = CreateCardSupport(reader);
                    CardSupportList.Add(item);
                }

                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return CardSupportList;

    }
    #endregion

    #region CharDeck
    public void InsertToCardDeck(PlayerData playerData)
    {
        for (int i = 0; i < playerData.deckUserCharCards.Count; i++)
        {
            string query = $"insert into gamedb.deck_characters(IdCharacter,IdPlayer) values({playerData.deckUserCharCards[i].id},{playerData.PlayerId})";
            var command = new MySqlCommand(query, con);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            command.Dispose();
        }
    }
    public void RemoveCardsDeck(PlayerData playerData)
    {
        string query = $"DELETE FROM gamedb.deck_characters WHERE IdPlayer = {playerData.PlayerId}";
        var command = new MySqlCommand(query, con);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        command.Dispose();

    }

    public List<CharacterCard> SelectFromDeckCards(PlayerData playerData)
    {
        string query = $"SELECT * FROM gamedb.characters as c inner join gamedb.deck_characters as cs on(c.idCharacters = cs.IdCharacter) where cs.IdPlayer = {playerData.PlayerId}";
        List<CharacterCard> cardList = new List<CharacterCard>();
        MySqlCommand command = new MySqlCommand(query, con);
        try
        {
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CharacterCard item = CreateCharacterCard(reader);
                    cardList.Add(item);
                }
                command.Dispose();
            }
            else
            {
                command.Dispose();
            }
        }
        catch (System.Exception ex)
        {
            command.Dispose();
            Debug.LogError(ex.Message);
        }
        return cardList;

    }
    #endregion

    #region CreatingCards
    private CharacterCard CreateCharacterCard(MySqlDataReader reader)
    {
        CharacterCard item = new CharacterCard();
        item.cardName = reader.GetString("char_name");

        switch (reader.GetString("race"))
        {
            case "����":
                item.race = Enums.Races.����;
                break;
            case "�����":
                item.race = Enums.Races.�����;
                break;
            case "�����":
                item.race = Enums.Races.�����;
                break;
            case "Ҹ���������":
                item.race = Enums.Races.Ҹ���������;
                break;
            case "������������������":
                item.race = Enums.Races.������������������;
                break;
        }

        switch (reader.GetString("class"))
        {
            case "�������":
                item.Class = Enums.Classes.�������;
                break;
            case "������":
                item.Class = Enums.Classes.������;
                break;
            case "���������":
                item.Class = Enums.Classes.���������;
                break;
            case "���":
                item.Class = Enums.Classes.���;
                break;
        }

        switch (reader.GetString("rarity"))
        {
            case "�������":
                item.rarity = Enums.Rarity.�������;
                break;
            case "����������":
                item.rarity = Enums.Rarity.����������;
                break;
        }

        item.description = reader.GetString("desctiption");

        item.health = Convert.ToInt32(reader.GetFloat("health"));
        item.speed = Convert.ToInt32(reader.GetFloat("speed"));
        item.physAttack = reader.GetFloat("p_attack");
        item.magAttack = reader.GetFloat("m_attack");
        item.range = Convert.ToInt32(reader.GetFloat("char_range"));
        item.physDefence = reader.GetFloat("p_defence");
        item.magDefence = reader.GetFloat("m_defence");
        item.critChance = reader.GetFloat("crit_possibility");
        item.critNum = reader.GetFloat("crit");

        item.passiveAbility = reader.GetString("passive");
        item.attackAbility = reader.GetString("special_1");
        item.defenceAbility = reader.GetString("special_2");
        item.buffAbility = reader.GetString("special_3");

        item.Price = reader.GetInt32("price");

        item.image = Resources.Load<Sprite>($"Card images/card of char/{reader.GetString("path")}");
        item.id = reader.GetInt32("idCharacters");

        /*                    Debug.Log($"name {item.name} ({reader.GetString("char_name")})");
                                       Debug.Log($"���� {item.race} ({reader.GetString("race")})");
                                       Debug.Log($"class {item.Class} ({reader.GetString("class")})");
                                       Debug.Log($"rarity {item.rarity} ({reader.GetString("rarity")})");
                                       Debug.Log($"desctiption {item.description} ({reader.GetString("desctiption")})");
                                       Debug.Log($"health {item.health} ({reader.GetFloat("health")})");
                                       Debug.Log($"speed {item.speed} ({reader.GetFloat("speed")})");
                                       Debug.Log($"physAttack {item.physAttack} ({reader.GetFloat("p_attack")})");
                                       Debug.Log($"magAttack {item.magAttack} ({reader.GetFloat("m_attack")})");
                                       Debug.Log($"range {item.range} ({reader.GetFloat("char_range")})");
                                       Debug.Log($"physDefence {item.physDefence} ({reader.GetFloat("p_defence")})");
                                       Debug.Log($"magDefence {item.magDefence} ({reader.GetFloat("m_defence")})");
                                       Debug.Log($"critChance {item.critChance} ({reader.GetDouble("crit_possibility")})");
                                       Debug.Log($"critNum {item.critNum} ({reader.GetDouble("crit")})");
                                       Debug.Log($"passiveAbility {item.passiveAbility} ({reader.GetString("passive")})");
                                       Debug.Log($"attackAbility {item.attackAbility} ({reader.GetString("special_1")})");
                                       Debug.Log($"defenceAbility {item.defenceAbility} ({reader.GetString("special_2")})");
                                       Debug.Log($"buffAbility {item.buffAbility} ({reader.GetString("special_3")})");
                                       Debug.Log($"name {item.image}");    
                    */

        return item;
    }

    private CardSupport CreateCardSupport(MySqlDataReader reader)
    {
        CardSupport item = new CardSupport();
        item.cardName = reader.GetString("name");
        switch (reader.GetString("race"))
        {
            case "����":
                item.race = Enums.Races.����;
                break;
            case "�����":
                item.race = Enums.Races.�����;
                break;
            case "�����":
                item.race = Enums.Races.�����;
                break;
            case "Ҹ���������":
                item.race = Enums.Races.Ҹ���������;
                break;
            case "������������������":
                item.race = Enums.Races.������������������;
                break;
        }
        switch (reader.GetString("type"))
        {
            case "���������":
                item.type = Enums.TypeOfSupport.���������;
                break;
            case "��������":
                item.type = Enums.TypeOfSupport.��������;
                break;
            case "�����������":
                item.type = Enums.TypeOfSupport.�����������;
                break;
        }
        item.abilityText = reader.GetString("effect");
        switch (reader.GetString("rarity"))
        {
            case "�������":
                item.rarity = Enums.Rarity.�������;
                break;
            case "����������":
                item.rarity = Enums.Rarity.����������;
                break;
        }
        item.Price = reader.GetInt32("price");
        item.id = reader.GetInt32("idCards");
        /*item.image = Resources.Load<Sprite>($"Card images/cards of support/{reader.GetString("path")}");*/

        return item;
        /*                    Debug.Log($"��� {item.name} ({reader.GetString("name")}), ���� {item.race} ({reader.GetString("race")}), ����������� {item.ability} ({reader.GetString("effect")}), ��� {item.type} ({reader.GetString("type")}), �������� {item.rarity} ({reader.GetString("rarity")})");
        */
    }
    #endregion
}




