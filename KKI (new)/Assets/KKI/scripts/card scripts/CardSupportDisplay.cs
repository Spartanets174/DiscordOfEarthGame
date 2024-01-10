
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(OutlineClicableUI))]
public class CardSupportDisplay : MonoBehaviour
{
    private CardSupport m_cardSupport;
    public CardSupport CardSupport => m_cardSupport;

    [SerializeField]
    private OutlineClicableUI m_clicable;
    public OutlineClicableUI Clicable => m_clicable;

    public TextMeshProUGUI cardSupportName;
    
    public TextMeshProUGUI ability;
    public TextMeshProUGUI rarity;
    public Image image;
    public Image rarityImage;

    public void SetValues(CardSupport cardSupport)
    {
        m_cardSupport = cardSupport;
        cardSupportName.text = m_cardSupport.name;
        image.sprite = m_cardSupport.image;
        ability.text = m_cardSupport.abilityText;
        rarity.text = m_cardSupport.rarity.ToString();
        rarity.color = Color.white;
        if (m_cardSupport.rarity.ToString() == "Обычная")
        {
            rarityImage.color = Color.gray;
        }
        else
        {
            rarityImage.color = new Color(126, 0, 255);
        }
    }
}
