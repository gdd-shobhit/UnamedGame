using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (sceneName == "Exit") {
            Application.Quit();
            Debug.Log(sceneName);
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    
}