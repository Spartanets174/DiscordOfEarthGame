using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void Exit()
    {
        Debug.Log("out");
        Application.Quit();
    }
    public void ToGame()
    {
        SceneManager.LoadScene("game");

    }
    public void ToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
