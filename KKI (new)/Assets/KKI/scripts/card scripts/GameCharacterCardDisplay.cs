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

    private CharacterCard m_currentCharacterCard;
    public CharacterCard CurrentCharacterCard => m_currentCharacterCard;

    private Character m_currentCharacter;
    public Character CurrentCharacter => m_currentCharacter;

    private Image image;
    private bool IsCharacterSpawned
    {
        get;
        set;
    } = false;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetData(CharacterCard characterCard )
    {
        m_currentCharacterCard = characterCard;
        charImage.sprite = m_currentCharacterCard.image;
        m_charealthBar.SetMaxHealth(m_currentCharacterCard.health);
        m_charealthBar.SetHealth(m_currentCharacterCard.health);      
    }

    public void SetCharacter(Character character)
    {
        m_currentCharacter = character;

        character.OnAttackAbilityUsed += OnAttackAbilityUsed;
        character.OnDefenceAbilityUsed += OnDefenceAbilityUsed;
        character.OnBuffAbilityUsed += OnBuffAbilityUsed;
        character.OnDamaged += SetHealth;
        character.OnHeal += SetHealth;
        character.OnDeath += OnDeath;
    }
/*    private void OnDestroy()
    {
        m_currentCharacter.OnAttackAbilityUsed -= OnAttackAbilityUsed;
        m_currentCharacter.OnDefenceAbilityUsed -= OnDefenceAbilityUsed;
        m_currentCharacter.OnBuffAbilityUsed -= OnBuffAbilityUsed;
        m_currentCharacter.OnDamaged -= SetHealth;
        m_currentCharacter.OnHeal -= SetHealth;
        m_currentCharacter.OnDeath -= OnDeath;
    }*/

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
