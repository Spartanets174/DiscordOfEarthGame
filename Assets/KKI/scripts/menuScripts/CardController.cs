using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static enums;

public abstract class CardController : MonoBehaviour, ILoadable
{
    protected PlayerManager m_PlayerManager;


    private Races m_currentRace;
    public Races CurrentRace
    {
        get => m_currentRace;
    }

    private Classes m_currentClass;
    public Classes CurrentClass
    {
        get => m_currentClass;
    }

    private typeOfSupport m_currentTypeOfSupport;
    public typeOfSupport CurrentTypeOfSupport 
        {
        get => m_currentTypeOfSupport;
    }
    public void Init()
    {
        m_currentRace = Races.���;
        m_currentClass = Classes.���;
        m_currentTypeOfSupport = typeOfSupport.���;
        m_PlayerManager = FindAnyObjectByType<PlayerManager>();
    }

    public void SetCurrentClass(Classes cardClass)
    {
        m_currentClass = cardClass;
    }
    public void SetCurrentRace(Races cardRace)
    {
        m_currentRace = cardRace;
    }
    public void SetCurrentTypeOfSupport(typeOfSupport cardSupportType)
    {
        m_currentTypeOfSupport = cardSupportType;
    }

    public List<CharacterCard> FilterCharacterCards(List<CharacterCard> listToFilter)
    {
        List<CharacterCard> cards= listToFilter;
        //���� ������ ������ �����
        if (CurrentRace == Races.��� && CurrentClass != Classes.���)
        {
            cards = listToFilter.Where(x => x.Class == CurrentClass).ToList();
        }
        //���� ������� ������ ����
        if (CurrentRace != Races.��� && CurrentClass == Classes.���)
        {
            cards = listToFilter.Where(x => x.race == CurrentRace).ToList();
            
        }
        //���� ������� ��� ���������
        if (CurrentRace != Races.��� && CurrentClass != Classes.���)
        {
            cards = listToFilter.Where(x => x.race == CurrentRace && x.Class == CurrentClass).ToList();
        }
        return cards;
    }

    public List<CardSupport> FilterSupportCards(List<CardSupport> listToFilter)
    {
  
        List<CardSupport> cardsSupport = listToFilter;
        if (CurrentTypeOfSupport != typeOfSupport.���)
        {
             cardsSupport = listToFilter.Where(x => x.type == CurrentTypeOfSupport).ToList();
        }
        return cardsSupport;
    }
}
