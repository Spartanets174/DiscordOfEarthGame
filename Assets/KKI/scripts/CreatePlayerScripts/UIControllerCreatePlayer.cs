using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerCreatePlayer : MonoBehaviour, ILoadable
{
    [SerializeField] private InputField Nick;
    [SerializeField] private InputField Password;
    [SerializeField] private TextMeshProUGUI warningTextNick;
    [SerializeField] private TextMeshProUGUI warningTextPassword;
    [SerializeField] private Button submit;
    [SerializeField] private DataLoader dataLoader;

    private Sequence sequenceName;
    private Sequence sequencePassword;
    public void Init()
    {
        submit.onClick.AddListener(CheckInputs);
        Nick.onValueChanged.AddListener(TurnOffWarningTextName);
        Password.onValueChanged.AddListener(TurnOffWarningTextPassword);
    }
    private void OnDestroy()
    {
        submit.onClick.RemoveListener(CheckInputs);
        Nick.onValueChanged.RemoveListener(TurnOffWarningTextName);
        Password.onValueChanged.RemoveListener(TurnOffWarningTextPassword);
        StopAllCoroutines();
    }
    private void TurnOffWarningTextName(string args)
    {
        warningTextNick.gameObject.SetActive(false);
    }
    private void TurnOnWarningTextName()
    {
        warningTextNick.alpha = 1;
        warningTextNick.gameObject.SetActive(true);
    }
    private void TurnOffWarningTextPassword(string args)
    {
        warningTextPassword.gameObject.SetActive(false);
    }
    private void TurnOnWarningTextPassword()
    {
        warningTextPassword.alpha = 1;
        warningTextPassword.gameObject.SetActive(true);
    }
    public void CheckInputs()
    {
        Nick.text = Nick.text.Trim();
        Password.text = Password.text.Trim();
        StopAllCoroutines();

        bool isNameValid = CheckName();
        bool isPasswordValid = CheckPassword();

        if (isNameValid && isPasswordValid)
        {
            dataLoader.CreateNewPlayer(Nick.text, Password.text);
            SceneManager.LoadScene("menu");
        }
    }

    private bool CheckPassword()
    {
        if (Password.text == "")
        {
            warningTextPassword.text = "Вы ничего не ввели!";
            OnWarningTextPassword();
            return false;
        }
        if (Password.text.Length < 4)
        {
            warningTextPassword.text = "Пароль слишком маленький (минимум 4 символа)!";
            OnWarningTextPassword();
            return false;
        }

        return true;
    }

    private bool CheckName()
    {
        if (Nick.text == "")
        {
            warningTextNick.text = "Вы ничего не ввели!";
            OnWarningTextName();
            return false;        }

        if (Nick.text.Length > 15)
        {
            warningTextNick.text = "Имя слишком большое (максимум 15 символов)!";
            OnWarningTextName();
            return false;
        }
        if (Nick.text.Length < 4)
        {

            warningTextNick.text = "Имя слишком маленькое (минимум 4 символа)!";
            OnWarningTextName();
            return false;
        }

        if (!dataLoader.IsNicknameInBase(Nick.text))
        {
            warningTextNick.text = "Данное имя уже существует!";
            OnWarningTextName();
            return false;
        }


        return true;
    }

    private void OnWarningTextName()
    {
        sequenceName.Kill();
        TurnOnWarningTextName();
        sequenceName = DOTween.Sequence();
        sequenceName.AppendInterval(2).Append(warningTextNick.DOFade(0, 2f))
            .OnComplete(() =>
            {
                warningTextNick.color = Color.red;
                TurnOffWarningTextName("args");
                sequenceName.Kill();
            });
        sequenceName.Play();
    }

    private void OnWarningTextPassword()
    {
        sequencePassword.Kill();
        TurnOnWarningTextPassword();
        sequencePassword = DOTween.Sequence();
        sequencePassword.AppendInterval(2).Append(warningTextPassword.DOFade(0, 2f))
            .OnComplete(() =>
            {
                warningTextPassword.color = Color.red;
                TurnOffWarningTextPassword("args");
                sequencePassword.Kill();
            });
        sequencePassword.Play();
    }
}

