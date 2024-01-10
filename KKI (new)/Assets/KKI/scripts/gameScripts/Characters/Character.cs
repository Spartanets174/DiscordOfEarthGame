using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected CharacterCard m_card;
    [SerializeField]
    protected HealthBar healthBar;
    public CharacterCard Card
    {
        get;
    }

    protected string m_characterName;
    public string CharacterName => m_characterName;

    protected enums.Races m_race;
    public enums.Races Race => m_race;

    protected enums.Classes m_Class;
    public enums.Classes Class => m_Class;

    protected enums.Rarity m_rarity;
    public enums.Rarity Кarity => m_rarity;

    protected float m_health;
    public float Health => m_health;

    protected int m_speed;
    public int Speed => m_speed;

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
    public int Index
    {
        get;
    }

    protected bool m_isAttackedOnTheMove = false;
    public bool IsAttackedOnTheMoved => m_isAttackedOnTheMove;

    public bool IsAttackAbilityUsed
    {
        get;
        private set;
    } = false;
    public bool IsDefenceAbilityUsed
    {
        get;
        private set;
    } = false;
    public bool IsBuffAbilityUsed
    {
        get;
        private set;
    } = false;

    public event Action<Character> OnAttack;
    public event Action<Character> OnHeal;
    public event Action<Character> OnDeath;
    public event Action<Character> OnDamaged;
    public event Action<Character> OnAttackAbilityUsed;
    public event Action<Character> OnDefenceAbilityUsed;
    public event Action<Character> OnBuffAbilityUsed;
    public event Action<Character> OnCharacterClicked;

    
    /*protected BattleSystem battleSystem;*/
    void Start()
    {
        m_characterName = m_card.name;
        m_race = m_card.race;
        m_Class = m_card.Class;
        m_rarity = m_card.rarity;
        m_health = m_card.health;
        m_speed = m_card.speed;
        m_physAttack = m_card.physAttack;
        m_magAttack = m_card.magAttack;
        m_range = m_card.range;
        m_physDefence = m_card.physDefence;
        m_magDefence = m_card.magDefence;
        m_critChance = m_card.critChance;
        m_critNum = m_card.critNum;

    }
   
    private void OnMouseDown()
    {
        OnCharacterClickedInvoke();
    }
    /*{
        if (!isEnemy&&!isStaticEnemy)
        {
            battleSystem.OnChooseCharacterButton(this.gameObject);
        }
        else
        {
            if (this.isChosen)
            {
                battleSystem.GetComponent<BattleSystem>().OnAttackButton(this);
            }
            *//*battleSystem.GetComponent<BattleSystem>().cahngeCardWindow(this.gameObject, true);*//*
        }
    }*/
    public bool Damage(Character chosenCharacter)
    {
        float crit = isCrit(chosenCharacter);
        float finalPhysDamage = ((11 + chosenCharacter.PhysAttack) * chosenCharacter.PhysAttack * crit * (chosenCharacter.PhysAttack - PhysDefence + Card.health)) / 256;
        float finalMagDamage = ((11 + chosenCharacter.MagAttack) * chosenCharacter.MagAttack * crit * (chosenCharacter.MagAttack - MagDefence + Card.health)) / 256;
        float finalDamage = Math.Max(finalMagDamage, finalPhysDamage);
        m_health = Math.Max(0, m_health - finalDamage);
        /*battleSystem.GameUIPresenter.SendMessage($"{chosenCharacter.name} наносит  юниту {name} {Mathf.RoundToInt(finalDamage * 100)} урона");
         if (!this.isStaticEnemy)
         {
             if (!this.isEnemy)
             {
                 for (int i = 0; i < battleSystem.charCardsUI.Count; i++)
                 {
                     if (this.name == battleSystem.charCardsUI[i].GetComponent<cardCharHolde>().card.name)
                     {
                         battleSystem.charCardsUI[i].GetComponent<cardCharHolde>().healthBar.SetHealth(health);
                     }
                 }
             }
             else
             {
                this.transform.GetChild(0).transform.GetChild(0).GetComponent<healthBar>().SetHealth(health);
             }
         }*/
        OnDamagedInvoke();
        if (m_health == 0)
        {
            OnDeathInvoke();
        }
        return m_health == 0;
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
        m_health += amount;
        OnHeal?.Invoke(this);
    }

    public void UseAtackAbility()
    {
        IsAttackAbilityUsed = true;
        OnAttackAbilityUsed?.Invoke(this);
    }
    public void UseDefenceAbility()
    {
        IsDefenceAbilityUsed = true;
        OnDefenceAbilityUsed?.Invoke(this);
    }
    public void UseBuffAbility()
    {
        IsBuffAbilityUsed = true;
        OnBuffAbilityUsed?.Invoke(this);
    }

    protected void OnAttackInvoke()
    {
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

    protected void OnCharacterClickedInvoke()
    {
        OnCharacterClicked?.Invoke(this);
    }
}
