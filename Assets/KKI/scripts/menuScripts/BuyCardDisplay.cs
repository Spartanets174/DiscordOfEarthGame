
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyCardDisplay : MonoBehaviour
{
    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private Image cardSprite;
    [SerializeField]
    private TextMeshProUGUI cardAbilities;
    [SerializeField]
    private TextMeshProUGUI cardPrice;
    [SerializeField]
    private TextMeshProUGUI moneyOfPlayer;
    [SerializeField]
    private TextMeshProUGUI NotEnoughtCaption;
    [SerializeField]
    private TextMeshProUGUI cardName;


    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button m_buyButton;
    public Button BuyButton => m_buyButton;

    private Card m_chosenCard;
    public Card ChosenCard => m_chosenCard;
    Sequence currentSequence;


    private void Start()
    {
        m_buyButton.onClick.AddListener(UpdateMoneyText);
        closeButton.onClick.AddListener(KillSequence);
    }

    private void OnDestroy()
    {
        m_buyButton.onClick.RemoveListener(UpdateMoneyText);
        closeButton.onClick.RemoveListener(KillSequence);
    }
    private void UpdateMoneyText()
    {
        moneyOfPlayer.text = "У вас денег: " + playerManager.Money.ToString() + "$";
    }

    private void KillSequence()
    {
        NotEnoughtCaption.gameObject.SetActive(false);
    }

    public IEnumerator TurnOffNotEnoughtCaption()
    {
        NotEnoughtCaption.gameObject.SetActive(true);
        NotEnoughtCaption.text = $"Не хватает: {m_chosenCard.Price - playerManager.Money}$";
        yield return new WaitForSecondsRealtime(2);
        currentSequence = DOTween.Sequence();
        currentSequence.id = 1;
        currentSequence.Append(NotEnoughtCaption.DOFade(0,2f))
        .OnComplete(() => {
            NotEnoughtCaption.color = Color.red;
            NotEnoughtCaption.gameObject.SetActive(false);
            currentSequence.Kill();
        });
        currentSequence.Play();
        
    }

    public void SetCardInfo(Card card)
    {
        cardName.text = card.characterName;
        m_chosenCard = card;
        cardSprite.sprite = card.image;
        cardPrice.text = card.Price.ToString()+"$";
        moneyOfPlayer.text = "У вас денег: " + playerManager.Money.ToString() + "$";

        if (card is CharacterCard)
        {
            CharacterCard characterCard = (CharacterCard)card;
            cardAbilities.text = $"Атакующая способность: {characterCard.attackAbility}" + "\n" + "\n" +
                    $"Защитная способность: {characterCard.defenceAbility}" + "\n" + "\n" +
                    $"Усиливающая способность: {characterCard.buffAbility}" + "\n" + "\n" +
                    $"Пассивная способность: {characterCard.passiveAbility}";

        }
        if (card is CardSupport)
        {
            CardSupport cardSupport = (CardSupport)card;
            cardAbilities.text = $"Способность: {cardSupport.abilityText}";
        }
    }
}
