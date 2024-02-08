using System;
using UnityEngine;

public abstract class Character : OutlineInteractableObject
{
    [SerializeField]
    protected CharacterCard m_card;
    public CharacterCard Card => m_card;

    [SerializeField]
    protected HealthBar healthBar;
    

    protected string m_characterName;
    public string CharacterName => m_characterName;

    protected enums.Races m_race;
    public enums.Races Race => m_race;

    protected enums.Classes m_Class;
    public enums.Classes Class => m_Class;

    protected enums.Rarity m_rarity;
    public enums.Rarity Êarity => m_rarity;

    protected float m_health;
    public float Health
    {
        get => m_health;
        private set 
        { 
            m_health = value;
            healthBar.SetHealth(m_health);
        }
    }

    protected int m_speed;
    public int Speed
    {
        get => m_speed;
        set => m_speed = value;
    }

    protected float m_physAttack;
    public float PhysAttack => m_physAttack;

    protected float m_magAttack;
    public float MagAttack => m_magAttack;

    protected int m_range;
    public int Range => m_range;

    protected float m_physDefence;
    public float PhysDefence => m_physDefence;

    protected float m_magDefence;
    public float MagDefence => m_magDefence;

    protected float m_critChance;
    public float CritChance => m_critChance;

    protected float m_critNum;
    public float CritNum => m_critNum;

    protected int m_index;
    public int Index => m_index;

    public Vector2 PositionOnField
    {
        get
        {
            return transform.GetComponentInParent<Cell>().CellIndex;
        }
    }

    public Cell ParentCell
    {
        get
        {
            return transform.GetComponentInParent<Cell>();
        }
    }

    protected bool m_isChosen = false;
    public bool IsChosen
    {
        get
        {
            return m_isChosen;
        }
        set
        {
            m_isChosen = value;
            SetOutlineState(IsChosen);
        }
    }


    protected bool m_isAttackedOnTheMove = false;
    public bool IsAttackedOnTheMove
    {
        get => m_isAttackedOnTheMove;
        set => m_isAttackedOnTheMove = value;
    }

    protected bool m_isAttackAbilityUsed = false;
    public bool IsAttackAbilityUsed => m_isAttackAbilityUsed;

    protected bool m_isDefenceAbilityUsed = false;
    public bool IsDefenceAbilityUsed => m_isDefenceAbilityUsed;

    protected bool m_isBuffAbilityUsed = false;
    public bool IsBuffAbilityUsed => m_isBuffAbilityUsed;

    public event Action<Character> OnAttack;
    public event Action<Character> OnHeal;
    public event Action<Character> OnDeath;
    public event Action<Character> OnDamaged;
    public event Action<Character> OnAttackAbilityUsed;
    public event Action<Character> OnDefenceAbilityUsed;
    public event Action<Character> OnBuffAbilityUsed;

    void Start()
    {
        m_characterName = m_card.characterName;
        m_race = m_card.race;
        m_Class = m_card.Class;
        m_rarity = m_card.rarity;
        Health = m_card.health;
        m_speed = m_card.speed;
        m_physAttack = m_card.physAttack;
        m_magAttack = m_card.magAttack;
        m_range = m_card.range;
        m_physDefence = m_card.physDefence;
        m_magDefence = m_card.magDefence;
        m_critChance = m_card.critChance;
        m_critNum = m_card.critNum;

        OnClick += OnCharacterClickedInvoke;
        IsChosen = false;
        
    }

    public void ResetCharacter()
    {
        m_speed = m_card.speed;
        m_isAttackedOnTheMove = false;
        m_isAttackAbilityUsed = false;
        m_isDefenceAbilityUsed = false;
        m_isBuffAbilityUsed = false;
    }
    public abstract void SetData(CharacterCard card, Material material, int currentIndex);
    public float Damage(Character chosenCharacter)
    {
        float crit = isCrit(chosenCharacter);
        float finalPhysDamage = ((11 + chosenCharacter.PhysAttack) * chosenCharacter.PhysAttack * crit * (chosenCharacter.PhysAttack - PhysDefence + Card.health)) / 256;
        float finalMagDamage = ((11 + chosenCharacter.MagAttack) * chosenCharacter.MagAttack * crit * (chosenCharacter.MagAttack - MagDefence + Card.health)) / 256;
        float finalDamage = Math.Max(finalMagDamage, finalPhysDamage);
        Health = Math.Max(0, Health - finalDamage);

        OnDamagedInvoke();
        if (Health == 0)
        {
            OnDeathInvoke();
        }
        return finalDamage;
    }
    protected float isCrit(Character chosenCharacter)
    {
        float chance =  UnityEngine.Random.Range(0f,1f);
        if (chance < chosenCharacter.m_critChance)
        {
            return chosenCharacter.m_critNum;
        }
        else
        {
            return 1;
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        OnHeal?.Invoke(this);
    }

    public void UseAtackAbility()
    {
        m_isAttackAbilityUsed = true;
        OnAttackAbilityUsed?.Invoke(this);
    }
    public void UseDefenceAbility()
    {
        m_isDefenceAbilityUsed = true;
        OnDefenceAbilityUsed?.Invoke(this);
    }
    public void UseBuffAbility()
    {
        m_isBuffAbilityUsed = true;
        OnBuffAbilityUsed?.Invoke(this);
    }

    protected void OnAttackInvoke()
    {
        m_isAttackedOnTheMove = true;
        OnAttack?.Invoke(this);
    }

    protected void OnDeathInvoke()
    {
        OnDeath?.Invoke(this);
    }

    protected void OnDamagedInvoke()
    {
        OnDamaged?.Invoke(this);
    }

    protected override void OnMouseExit()
    {
        if (IsEnabled&&!IsChosen)
        {
            OnHoverExitInvoke();
        }
    }

    protected void OnCharacterClickedInvoke(GameObject gameObject)
    {
        IsChosen = true;
    }
}
