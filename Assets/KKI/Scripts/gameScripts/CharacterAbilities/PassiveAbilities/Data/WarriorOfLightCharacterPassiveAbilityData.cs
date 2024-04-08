using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WarriorOfLightCharacterPassiveAbilityData : BasePassiveCharacterAbilityData
{
    [Range(0, 3),Tooltip("�������� ���������� ������, �� �������� ������������ ����� ��������� (���� ������, �� ���������� � ������ ��� ������)")]
    public float selectMagDefenceAmount;

    [Range(0, 3), Tooltip("�������� ���������� ������, �� ������� ����������� �������� ��������� ��������")]
    public float buffMagDefenceAmount;
}
