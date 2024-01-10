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
        charStatsText.text = $"��: {characterCard.health*100}" + "\n" +
                    $"��: {characterCard.physAttack * 100}" + "\n" +
                    $"��: {characterCard.magAttack * 100}" + "\n" +
                    $"��: {characterCard.physDefence * 100}" + "\n" +
                    $"��: {characterCard.magDefence * 100}" + "\n" +
                    $"��: {characterCard.critChance * 100}";
        charDescription.text = characterCard.description;
        charAbilities.text = $"��������� �����������: {characterCard.attackAbility}" + "\n" + "\n" +
                    $"�������� �����������: {characterCard.defenceAbility}" + "\n" + "\n" +
                    $"����������� �����������: {characterCard.buffAbility}" + "\n" + "\n" +
                    $"��������� �����������: {characterCard.passiveAbility}";
    }
}
