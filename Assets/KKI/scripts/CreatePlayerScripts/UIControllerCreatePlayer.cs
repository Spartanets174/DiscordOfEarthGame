using DG.Tweening;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerCreatePlayer : MonoBehaviour, ILoadable
{
    [Header("Action UI")]
    [SerializeField] private InputField Nick;
    [SerializeField] private InputField Password;
    [SerializeField] private Button submitRegistration;
    [SerializeField] private Button submitLogin;
    [SerializeField] private Button toRegistration;
    [SerializeField] private Button toLogin;

    [Header("Panels")]
    [SerializeField] private GameObject registrationPanel;
    [SerializeField] private GameObject loginPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI warningTextNick;
    [SerializeField] private TextMeshProUGUI warningTextPassword;

    [Header("Scripts")]
    [SerializeField] private DataLoader dataLoader;

    private Sequence sequenceName;
    private Sequence sequencePassword;
    private Regex validateGuidRegex;
    public void Init()
    {
        validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[#?!@$%^&*-]).{4,}$");
        submitLogin.onClick.AddListener(CheckInputsLogin);
        submitRegistration.onClick.AddListener(CheckInputs);
        toRegistration.onClick.AddListener(SwapPanels);
        toLogin.onClick.AddListener(SwapPanels);
        Nick.onValueChanged.AddListener(TurnOffWarningTextName);
        Password.onValueChanged.AddListener(TurnOffWarningTextPassword);
    }

    private void OnDestroy()
    {
        submitLogin.onClick.RemoveListener(CheckInputsLogin);
        submitRegistration.onClick.RemoveListener(CheckInputs);
        toRegistration.onClick.RemoveListener(SwapPanels);
        toLogin.onClick.RemoveListener(SwapPanels);
        Nick.onValueChanged.RemoveListener(TurnOffWarningTextName);
        Password.onValueChanged.RemoveListener(TurnOffWarningTextPassword);
        StopAllCoroutines();
    }
    private void CheckInputsLogin()
    {
        Nick.text = Nick.text.Trim();
        Password.text = Password.text.Trim();
        StopAllCoroutines();

        bool isNameValid = CheckNameLogin();
        bool isPasswordValid = CheckPasswordLogin();

        if (isNameValid && isPasswordValid)
        {
            string connectionAnwser = dataLoader.GetPlayerData(Nick.text, Password.text);
            if (connectionAnwser== "loginned")
            {
               SceneController.ToMenu();
            }
            if (connectionAnwser == "notConnected")
            {
                warningTextPassword.text = "Отсутсвует подключение к серверу!";
                OnWarningTextPassword();
            }
            if (connectionAnwser == "wrongCreditials")
            {
                warningTextPassword.text = "Неправильный логин или пароль!";
                OnWarningTextPassword();
            }
        }
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
            bool isConnected = dataLoader.CreateNewPlayer(Nick.text, Password.text);
            if (isConnected)
            {
                SceneController.ToMenu();
            }
            else
            {
                warningTextPassword.text = "Отсутсвует подключение к серверу!";
                OnWarningTextPassword();
            }        
        }
    }

    private void SwapPanels()
    {
        registrationPanel.SetActive(!registrationPanel.activeSelf);
        loginPanel.SetActive(!loginPanel.activeSelf);
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
    private bool CheckPasswordLogin()
    {
        if (Password.text == "")
        {
            warningTextPassword.text = "Вы ничего не ввели!";
            OnWarningTextPassword();
            return false;
        }

        return true;
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
        if (!validateGuidRegex.IsMatch(Password.text))
        {
            warningTextPassword.text = "Пароль должен соответсвовать требованиям: \nХотя бы одна заглавная английская буква,хотя бы один специальный символ";
            OnWarningTextPassword();
            return false;
        }
        return true;
    }

    private bool CheckNameLogin()
    {
        if (Nick.text == "")
        {
            warningTextNick.text = "Вы ничего не ввели!";
            OnWarningTextName();
            return false;
        }

        if (dataLoader.IsNicknameInBase(Nick.text))
        {
            warningTextNick.text = "Данного имени не существует!";
            OnWarningTextName();
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

