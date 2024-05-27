using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
public class GameCharacterCardDisplay : OutlineClicableUI
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

    private PlayerCharacter m_currentCharacter;
    public PlayerCharacter CurrentCharacter => m_currentCharacter;

    private bool m_isCharacterSpawned;
    public bool IsCharacterSpawned
    {
        get { return m_isCharacterSpawned; }
        set
        {
            m_isCharacterSpawned = value;
            IsEnabled = !value;
        }
    }

    private bool m_isChosen;
    public bool IsChosen
    {
        get { return m_isChosen; }
        set
        {
            m_isChosen = value;
            IsEnabled = value;
        }
    }

    public event Action<GameObject> OnCharacterSetted;
    private void Start()
    {
        m_isCharacterSpawned = false;
    }

    public void SetData(CharacterCard characterCard )
    {
        m_currentCharacterCard = characterCard;
        charImage.sprite = m_currentCharacterCard.image;
        m_charealthBar.SetMaxHealth(m_currentCharacterCard.health);
        m_charealthBar.SetHealth(m_currentCharacterCard.health, 1);      
    }

    public void SetCharacter(PlayerCharacter character)
    {
        m_currentCharacter = character;

        character.AttackCharacterAbility.OnCardAbilityUsed += OnAttackAbilityUsed;
        character.DefenceCharacterAbility.OnCardAbilityUsed += OnDefenceAbilityUsed;
        character.BuffCharacterAbility.OnCardAbilityUsed += OnBuffAbilityUsed;
        character.OnDamaged += (x,y,z)=> SetHealth(x);
        character.OnHeal += (x, y, z)=> SetHealth(x);
        character.OnDeath += OnDeath;
        OnCharacterSetted?.Invoke(gameObject);
    }

    public void OnAttackAbilityUsed(BaseCharacterAbility baseCharacterAbility)
    {
        charAttackAbilityImage.DOFade(0,1);
    }
    public void OnDefenceAbilityUsed(BaseCharacterAbility baseCharacterAbility)
    {
        charDefenceAbilityImage.DOFade(0, 1);
    }
    public void OnBuffAbilityUsed(BaseCharacterAbility baseCharacterAbility)
    {
        charBuffAbilityImage.DOFade(0, 1);
    }

    private void SetHealth(Character character)
    {
        m_charealthBar.SetHealth(character.Health, 1);
    }

    private void OnDeath(Character character)
    {
        IsEnabled = false;
    }
}
