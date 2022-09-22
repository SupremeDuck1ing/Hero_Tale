using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("HeroesTaleScene_v1.2");
    }

    public void QuitGame()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}
