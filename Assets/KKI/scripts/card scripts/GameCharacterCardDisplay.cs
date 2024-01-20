using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class GameCharacterCardDisplay : MonoBehaviour
{
    [SerializeField]
    private Image charImage;
    [SerializeField]
    private Image charAttackAbilityImage;
    [SerializeField]
    private Image charDefenceAbilityImage;
    [SerializeField]
    private Image charBuffAbilityImage;
    [SerializeField]
    private HealthBar m_charealthBar;

    [SerializeField]
    private OutlineClicableUI m_outlineClicableUI;
    public OutlineClicableUI OutlineClicableUI => m_outlineClicableUI;

    private CharacterCard m_currentCharacterCard;
    public CharacterCard CurrentCharacterCard => m_currentCharacterCard;

    private PlayerCharacter m_currentCharacter;
    public PlayerCharacter CurrentCharacter => m_currentCharacter;

    private Image image;

    private bool m_isCharacterSpawned;
    public bool IsCharacterSpawned
    {
        get { return m_isCharacterSpawned; }
        set
        {
            m_isCharacterSpawned = value;
            OutlineClicableUI.IsEnabled = !value;
        }
    }

    private bool m_isChosen;
    public bool IsChosen
    {
        get { return m_isChosen; }
        set
        {
            m_isChosen = value;
            OutlineClicableUI.IsEnabled = value;
        }
    }

    private void Start()
    {
        m_isCharacterSpawned = false;
        image = GetComponent<Image>();
    }

    public void SetData(CharacterCard characterCard )
    {
        m_currentCharacterCard = characterCard;
        charImage.sprite = m_currentCharacterCard.image;
        m_charealthBar.SetMaxHealth(m_currentCharacterCard.health);
        m_charealthBar.SetHealth(m_currentCharacterCard.health);      
    }

    public void SetCharacter(PlayerCharacter character)
    {
        m_currentCharacter = character;

        character.OnAttackAbilityUsed += OnAttackAbilityUsed;
        character.OnDefenceAbilityUsed += OnDefenceAbilityUsed;
        character.OnBuffAbilityUsed += OnBuffAbilityUsed;
        character.OnDamaged += SetHealth;
        character.OnHeal += SetHealth;
        character.OnDeath += OnDeath;
    }

    public void OnAttackAbilityUsed(Character character)
    {
        charAttackAbilityImage.DOFade(0.5f,1);
    }
    public void OnDefenceAbilityUsed(Character character)
    {
        charDefenceAbilityImage.DOFade(0.5f, 1);
    }
    public void OnBuffAbilityUsed(Character character)
    {
        charBuffAbilityImage.DOFade(0.5f, 1);
    }

    private void SetHealth(Character character)
    {
        m_charealthBar.SetHealth(character.Health);
    }

    private void OnDeath(Character character)
    {
        image.DOFade(0.5f, 1);
    }
}
