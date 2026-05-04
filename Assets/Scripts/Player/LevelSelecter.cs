using UnityEngine;
using UnityEngine.SceneManagement;

public class HubLevelSelector : MonoBehaviour
{
    public string level1Scene = "First Level";
    public string level2Scene = "Second Level";
    public string level3Scene = "Third Level";
    public string level4Scene = "Fourth Level";

    private bool isLoading = false;

    private void Update()
    {
        if (isLoading) return;

        if (PlayerMovement.levelCount >= 4)
        {
            isLoading = true;
            SceneManager.LoadScene("Win Screen");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;

        string tag = other.tag;

        if (tag == "Level1" && PlayerMovement.levelCount == 0 && !PlayerMovement.levelCompleted[0])
        {
            LoadLevel(level1Scene);
        }
        else if (tag == "Level2" && PlayerMovement.levelCount == 1 && !PlayerMovement.levelCompleted[1])
        {
            LoadLevel(level2Scene);
        }
        else if (tag == "Level3" && PlayerMovement.levelCount == 2 && !PlayerMovement.levelCompleted[2])
        {
            LoadLevel(level3Scene);
        }
        else if (tag == "Level4" && PlayerMovement.levelCount == 3 && !PlayerMovement.levelCompleted[3])
        {
            LoadLevel(level4Scene);
        }
        else
        {
            Debug.Log("Level locked or already completed!");
        }
    }

    private void LoadLevel(string sceneName)
    {
        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }
}