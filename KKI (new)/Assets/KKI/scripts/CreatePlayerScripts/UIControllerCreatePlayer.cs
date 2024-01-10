using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerCreatePlayer : MonoBehaviour, ILoadable
{
    [SerializeField] private InputField Nick;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button submit;
    [SerializeField] private DataLoader dataLoader;
    public void Init()
    {
        submit.onClick.AddListener(CheckNick);
        Nick.onValueChanged.AddListener(TurnOffWarningText);
    }
    private void OnDestroy()
    {
        submit.onClick.RemoveListener(CheckNick);
        Nick.onValueChanged.RemoveListener(TurnOffWarningText);
        StopAllCoroutines();
    }
    private void TurnOffWarningText(string args)
    {
        warningText.gameObject.SetActive(false);
    }
    private void TurnOnWarningText()
    {
        warningText.gameObject.SetActive(true);
    }
    public void CheckNick()
    {
        Nick.text = Nick.text.Trim();
        if (Nick.text == "")
        {
            warningText.text = "Вы ничего не ввели!";
            StartCoroutine(OnWarningText());
        }
        else
        {
            if (Nick.text.Length <= 15)
            {
                if (Nick.text.Length >= 4)
                {
                    if (dataLoader.IsNicknameInBase(Nick.text))
                    {
                        SceneManager.LoadScene("menu");
                    }
                    else
                    {
                        warningText.text = "Данное имя уже существует!";
                        StartCoroutine(OnWarningText());
                        
                    }
                }
                else
                {
                    warningText.text = "Имя слишком маленькое (минимум 4 символа)!";
                    StartCoroutine(OnWarningText());
                }
            }
            else
            {
                warningText.text = "Имя слишком большое (максимум 15 символов)!";
                StartCoroutine(OnWarningText());
            }
        }
    }
    IEnumerator OnWarningText()
    {
        TurnOnWarningText();
        yield return new WaitForSecondsRealtime(2);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(warningText.DOFade(0, 2f))
            .OnComplete(() => {
                warningText.color = Color.red;
                TurnOffWarningText("args");
                mySequence.Kill();
            });
            mySequence.Play();
    }
}

