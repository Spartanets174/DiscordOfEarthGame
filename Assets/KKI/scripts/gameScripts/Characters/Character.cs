using DG.Tweening;
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
    public float PhysAttack
    {
        get => m_physAttack;
        set => m_physAttack = value;
    }

    protected float m_magAttack;
    public float MagAttack
    {
        get => m_magAttack;
        set => m_magAttack = value;
    }

    protected int m_range;
    public int Range
    {
        get => m_range;
        set => m_range = value;
    }

    protected float m_physDefence;
    public float PhysDefence
    {
        get => m_physDefence;
        set => m_physDefence = value;
    }

    protected float m_magDefence;
    public float MagDefence
    {
        get => m_magDefence;
        set => m_magDefence = value;
    }

    protected float m_critChance;
    public float CritChance
    {
        get => m_critChance;
        set
        {         
            m_critChance = value;
        }
    }

    protected float m_critNum;
    public float CritNum
    {
        get => m_critNum;
        set => m_critNum = value;
    }

    protected int m_index=0;
    public int Index => m_index;

    protected float m_chanceToAvoidDamage = 0;
    public float ChanceToAvoidDamage
    {
        get => m_chanceToAvoidDamage;
        set => m_chanceToAvoidDamage = value;
    }

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

    private bool m_IsFreezed;
    public bool IsFreezed
    {
        get=> m_IsFreezed;
        set {
            m_IsFreezed = value;
            if (m_IsFreezed)
            {
                Speed = 0;
            }                      
        }
    }

    private bool m_canBeDamaged;
    public bool CanBeDamaged
    {
        get => m_canBeDamaged;
        set
        {
            m_canBeDamaged = value;
        }
    }

    protected bool m_isAttackedOnTheMove = false;
    public bool IsAttackedOnTheMove
    {
        get => m_isAttackedOnTheMove;
        set => m_isAttackedOnTheMove = value;
    }

    protected float m_lastDamageAmount;
    public float LastDamageAmount
    {
        get => m_lastDamageAmount;
        private set => m_lastDamageAmount = value;
    }
    protected Character m_lastAttackedCharacter;
    public Character LastAttackedCharacter
    {
        get => m_lastAttackedCharacter;
        private set => m_lastAttackedCharacter = value;
    }

    public event Action<Character> OnAttack;
    public event Action<Character> OnHeal;
    public event Action<Character> OnDeath;
    public event Action<Character, string, float> OnDamaged;
    public event Action<Character> OnAttackAbilityUsed;
    public event Action<Character> OnDefenceAbilityUsed;
    public event Action<Character> OnBuffAbilityUsed;
    public event Action<Character> OnPositionOnFieldChanged;

    public void ResetCharacter()
    {
        if (!IsFreezed)
        {
            m_speed = m_card.speed;
        }       
        m_isAttackedOnTheMove = false;
    }

    public void RemoveDebuffs()
    {
        Debug.Log("Stats is normal");
        IsFreezed = false;
        if (m_physAttack< m_card.physAttack) m_physAttack = m_card.physAttack;
        if (m_magAttack< m_card.magAttack) m_magAttack = m_card.magAttack;
        if (m_range < m_card.range) m_range = m_card.range;
        if (m_physDefence< m_card.physDefence) m_physDefence = m_card.physDefence;
        if (m_magDefence< m_card.magDefence) m_magDefence = m_card.magDefence;
        if (m_critChance< m_card.critChance) m_critChance = m_card.critChance;
        if (m_critNum< m_card.critNum) m_critNum = m_card.critNum;        
    }

    public virtual void SetData(CharacterCard card, Material material, int currentIndex)
    {
        m_card = card;
        m_characterName = m_card.cardName;
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
        CanBeDamaged = true;
    }
    public virtual bool Damage(Character chosenCharacter)
    {
        if (!CanBeDamaged)
        {
            OnDamaged?.Invoke(this, chosenCharacter.CharacterName, 0);
            return false;
        }

        if (IsDamageAvoided())
        {
            OnDamaged?.Invoke(this, chosenCharacter.CharacterName, 0);
            return false;
        }

        float crit = IsCrit(chosenCharacter.CritChance,chosenCharacter.CritNum);
        float finalPhysDamage = ((11 + chosenCharacter.PhysAttack) * chosenCharacter.PhysAttack * crit * (chosenCharacter.PhysAttack - PhysDefence + Card.health)) / 256;
        float finalMagDamage = ((11 + chosenCharacter.MagAttack) * chosenCharacter.MagAttack * crit * (chosenCharacter.MagAttack - MagDefence + Card.health)) / 256;
        float finalDamage = Math.Max(finalMagDamage, finalPhysDamage);

        if (finalDamage == 0)
        {
            finalDamage = UnityEngine.Random.Range(0.01f, 0.1f);
        }
        Health = Math.Max(0, Health - finalDamage);

        LastDamageAmount = finalDamage;
        LastAttackedCharacter = chosenCharacter;

        OnDamaged?.Invoke(this, chosenCharacter.CharacterName, finalDamage);
        if (Health == 0)
        {
            OnDeath?.Invoke(this);
        }
        
        return Health == 0;
    }

    public bool Damage(float damage, string nameObject)
    {
        if (!CanBeDamaged) return false;

        if (IsDamageAvoided()) return false;

        Health = Math.Max(0, Health - damage);

        OnDamaged?.Invoke(this, nameObject, damage);
        if (Health == 0)
        {
            OnDeath?.Invoke(this);
        }
        return Health == 0;
    }

    protected bool IsDamageAvoided()
    {
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance < ChanceToAvoidDamage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected float IsCrit(float critChance, float m_critNum)
    {
        float chance =  UnityEngine.Random.Range(0f,1f);
        if (chance < critChance)
        {
            return m_critNum;
        }
        else
        {
            return 1;
        }
    }

    public void Heal(int amount)
    {
        float temp = Health + amount;
        if (temp>Card.health)
        {
            Health = Card.health;
        }
        else
        {
            Health = temp;
        }
        OnHeal?.Invoke(this);
    }

    public void HealMoreThenMax(int amount)
    {
        Health += amount;
        OnHeal?.Invoke(this);
    }

    public void Move(int moveCost, Transform positionToMove)
    {
        Speed -= Convert.ToInt32(moveCost);

        Vector3 cellToMovePos = positionToMove.position;
        transform.DOMove(new Vector3(cellToMovePos.x, transform.position.y, cellToMovePos.z), 0.5f).OnComplete(() =>
        {
            transform.SetParent(positionToMove);
            transform.localPosition = new Vector3(0, 1, 0);           
            OnPositionOnFieldChanged?.Invoke(this);
        });      
    }

    public void UseAtackAbility()
    {
        OnAttackAbilityUsed?.Invoke(this);
    }
    public void UseDefenceAbility()
    {
        OnDefenceAbilityUsed?.Invoke(this);
    }
    public void UseBuffAbility()
    {
        OnBuffAbilityUsed?.Invoke(this);
    }

    protected void OnAttackInvoke()
    {
        m_isAttackedOnTheMove = true;
        OnAttack?.Invoke(this);
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
