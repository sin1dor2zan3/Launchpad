using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    public void PlayAgainButton() 
    {
        SceneManager.LoadSceneAsync(1);
        PlayerMovement.levelCount = 0;
        for (int i = 0; i < PlayerMovement.levelCompleted.Length; i++)        
        {
            PlayerMovement.levelCompleted[i] = false;
        }
    }
    
    public void MenuButton() 
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitButton() 
    {
        Application.Quit();
    }

}
