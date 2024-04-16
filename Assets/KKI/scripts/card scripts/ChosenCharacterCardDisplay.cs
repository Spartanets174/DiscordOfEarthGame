using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField]
    private Color attackColor;
    [SerializeField]
    private Color defenceColor;
    [SerializeField]
    private Color buffColor;
    [SerializeField]
    private Color passiveColor;

    private CharacterCard m_chosenCharCard;
    public CharacterCard ChosenCharCard => m_chosenCharCard;

    public void SetCharacterData(GameObject characterCardObject)
    {
        CharacterCard characterCard = characterCardObject.GetComponent<CardDisplay>().Card;
        m_chosenCharCard = characterCard;
        charImage.sprite = characterCard.image;
        charImage.DOFade(1,0);
        charStatsText.text = $"��: {characterCard.health*100}" + "\n" +
                    $"��: {characterCard.physAttack * 100}" + "\n" +
                    $"��: {characterCard.magAttack * 100}" + "\n" +
                    $"��: {characterCard.physDefence * 100}" + "\n" +
                    $"��: {characterCard.magDefence * 100}" + "\n" +
                    $"��: {characterCard.critChance * 100}";
        charDescription.text = characterCard.description;
        charAbilities.text = $"<color=#{attackColor.ToHexString()}>��������� �����������</color>: {characterCard.attackAbility}" + "\n" + "\n" +
                    $"<color=#{defenceColor.ToHexString()}>�������� �����������</color>: {characterCard.defenceAbility}" + "\n" + "\n" +
                    $"<color=#{buffColor.ToHexString()}>����������� �����������</color>: {characterCard.buffAbility}" + "\n" + "\n" +
                    $"<color=#{passiveColor.ToHexString()}>��������� �����������</color> {characterCard.passiveAbility}";

    }
}
