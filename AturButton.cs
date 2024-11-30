using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AturButton : MonoBehaviour
{

    public void RestartGame()
    {
        Debug.Log("Restart button clicked.");
        //Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        Debug.Log("Main Menu button clicked.");
        SceneManager.LoadScene("MainMenu");
    }

    public void HowTo()
    {
        Debug.Log("How");
        SceneManager.LoadScene("ApaNih");
    }

}
