using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChosenCharacterCardDisplay : MonoBehaviour
{
    [SerializeField]
    private Image charImage;
    [SerializeField]
    private TextMeshProUGUI charStatsText;
    [SerializeField]
    private TextMeshProUGUI charDescription;
    [SerializeField]
    private TextMeshProUGUI charAbilities;

   private CharacterCard m_chosenCharCard;
    public CharacterCard ChosenCharCard => m_chosenCharCard;

    public void SetCharacterData(GameObject characterCardObject)
    {
        CharacterCard characterCard = characterCardObject.GetComponent<CardDisplay>().Card;
        m_chosenCharCard = characterCard;
        charImage.sprite = characterCard.image;
        charStatsText.text = $"ЗД: {characterCard.health*100}" + "\n" +
                    $"ФА: {characterCard.physAttack * 100}" + "\n" +
                    $"МА: {characterCard.magAttack * 100}" + "\n" +
                    $"ФЗ: {characterCard.physDefence * 100}" + "\n" +
                    $"МЗ: {characterCard.magDefence * 100}" + "\n" +
                    $"ВК: {characterCard.critChance * 100}";
        charDescription.text = characterCard.description;
        charAbilities.text = $"Атакующая способность: {characterCard.attackAbility}" + "\n" + "\n" +
                    $"Защитная способность: {characterCard.defenceAbility}" + "\n" + "\n" +
                    $"Усиливающая способность: {characterCard.buffAbility}" + "\n" + "\n" +
                    $"Пассивная способность: {characterCard.passiveAbility}";
    }
}
