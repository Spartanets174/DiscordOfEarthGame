using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Скрипт для изменения полоски здоровья в интерфейсе
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(float health)
    {
        DOTween.To(()=> { return slider.value; }, SetSliderValue, health,1f);
    }

    private void SetSliderValue(float x)
    {
        slider.value = x;
    }
}
