using UnityEngine;
using UnityEngine.SceneManagement;

public class HubLevelSelector : MonoBehaviour
{
    public string level1Scene = "First Level";
    public string level2Scene = "Second Level";
    public string level3Scene = "Third Level";
    public string level4Scene = "Fourth Level";

    private bool isLoading = false;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (isLoading) return;
        if (playerMovement == null) return;

        if (playerMovement.levelCount >= 4)
        {
            isLoading = true;
            SceneManager.LoadScene("Win Screen");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;

        string sceneToLoad = GetSceneFromTag(other.tag);

        if (string.IsNullOrEmpty(sceneToLoad)) return;

        isLoading = true;
        SceneManager.LoadScene(sceneToLoad);
    }

    private string GetSceneFromTag(string tag)
    {
        switch (tag)
        {
            case "Level1": return level1Scene;
            case "Level2": return level2Scene;
            case "Level3": return level3Scene;
            case "Level4": return level4Scene;
            default: return "";
        }
    }
}