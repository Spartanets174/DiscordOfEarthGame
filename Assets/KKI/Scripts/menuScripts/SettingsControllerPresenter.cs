using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsControllerPresenter : MonoBehaviour, ILoadable
{
    [Header("Game objects")]
    [SerializeField]
    private GameObject sureToExitWindow;

    [Header("Action UI")]
    [SerializeField]
    private Button closeSureToExitWindowButton;
    [SerializeField]
    private Button noSureToExitWindowButton;
    [SerializeField]
    private Button yesSureToExitWindowButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button exitAccountButton;

    private SettingsController settingsController;
    public void Init()
    {
        settingsController = FindObjectOfType<SettingsController>();

        closeSureToExitWindowButton.onClick.AddListener(CloseSureToExitWindow);
        noSureToExitWindowButton.onClick.AddListener(CloseSureToExitWindow);
        yesSureToExitWindowButton.onClick.AddListener(ExitAccount);
        closeButton.onClick.AddListener(CloseSettings);
        exitAccountButton.onClick.AddListener(OpenSureToExitWindow);
    }

    private void OpenSureToExitWindow()
    {
        sureToExitWindow.SetActive(true);
    }

    private void CloseSureToExitWindow()
    {
        sureToExitWindow.SetActive(false);
    }

    private void CloseSettings()
    {
        gameObject.SetActive(false);
        foreach (var outlineInteractableObject in settingsController.OutlineInteractableObjects)
        {
            outlineInteractableObject.IsEnabled = true;
        }
    }

    private void ExitAccount()
    {
        settingsController.ExitAccount();
    }
}
