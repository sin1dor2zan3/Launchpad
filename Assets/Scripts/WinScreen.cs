using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void MenuButton() 
    {
        SceneManager.LoadSceneAsync(0);
    }

}
