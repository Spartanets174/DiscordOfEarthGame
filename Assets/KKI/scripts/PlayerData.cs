using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player data")]
public class PlayerData : ScriptableObject
{
    public List<CharacterCard> allCharCards;
    public List<CardSupport> allSupportCards;
    public List<CharacterCard> allShopCharCards;
    public List<CardSupport> allShopSupportCards;
    public List<CharacterCard> allUserCharCards;
    public List<CardSupport> allUserSupportCards;
    public List<CharacterCard> deckUserCharCards;
    public List<CardSupport> deckUserSupportCards;
    public int money;
    public string Name;
    public int PlayerId;
}
