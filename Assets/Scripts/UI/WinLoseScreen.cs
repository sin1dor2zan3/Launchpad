using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    public void PlayAgainButton() 
    {
        SceneManager.LoadSceneAsync(1);
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
