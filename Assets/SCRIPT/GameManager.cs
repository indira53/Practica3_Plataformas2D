using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void DeathScene()
    {
        SceneManager.LoadScene("DeathScene");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("GameplayScene");
    }

    public void ExitButton()
    {
        Debug.Log("quitting");
        Application.Quit();
        Debug.Log("Not quitting");
    }
}
