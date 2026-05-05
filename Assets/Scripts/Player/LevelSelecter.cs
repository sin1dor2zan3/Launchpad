using UnityEngine;
using UnityEngine.SceneManagement;

public class HubLevelSelector : MonoBehaviour
{
    [Header("Scenes")]
    public string level1Scene = "First Level";
    public string level2Scene = "Second Level";
    public string level3Scene = "Third Level";
    public string level4Scene = "Fourth Level";

    [Header("Level Objects (assign in inspector)")]
    public Renderer level1Renderer;
    public Renderer level2Renderer;
    public Renderer level3Renderer;
    public Renderer level4Renderer;

    [Header("Materials")]
    public Material levelLockedMat;
    public Material levelUnlockedMat;
    public Material levelCompletedMat;

    private bool isLoading = false;

    private void Start()
    {
        UpdateLevelMaterials();
    }

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

        if (tag == "Level1" && IsUnlocked(0))
        {
            LoadLevel(level1Scene);
        }
        else if (tag == "Level2" && IsUnlocked(1))
        {
            LoadLevel(level2Scene);
        }
        else if (tag == "Level3" && IsUnlocked(2))
        {
            LoadLevel(level3Scene);
        }
        else if (tag == "Level4" && IsUnlocked(3))
        {
            LoadLevel(level4Scene);
        }
        else
        {
            Debug.Log("Level locked or already completed!");
        }
    }

    private bool IsUnlocked(int index)
    {
        return PlayerMovement.levelCount == index && !PlayerMovement.levelCompleted[index];
    }

    private void LoadLevel(string sceneName)
    {
        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    // =========================
    // MATERIAL LOGIC
    // =========================
    private void UpdateLevelMaterials()
    {
        SetMaterial(level1Renderer, 0);
        SetMaterial(level2Renderer, 1);
        SetMaterial(level3Renderer, 2);
        SetMaterial(level4Renderer, 3);
    }

    private void SetMaterial(Renderer rend, int index)
    {
        if (rend == null) return;

        if (PlayerMovement.levelCompleted[index])
        {
            rend.material = levelCompletedMat;
        }
        else if (PlayerMovement.levelCount == index)
        {
            rend.material = levelUnlockedMat;
        }
        else
        {
            rend.material = levelLockedMat;
        }
    }
}