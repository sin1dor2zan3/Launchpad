using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Timer Settings")]
    public float timeLimit = 60f;

    private float currentTime;
    private bool gameEnded = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private void Awake()
    {
        Instance = this;
        currentTime = timeLimit;
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;

        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            LoseGame();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        float timeToDisplay = Mathf.Max(0, currentTime);

        int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60f);

        timerText.text = $"Time Remaining: {minutes:00}:{seconds:00}";
    }

    public float GetTimeRemaining()
    {
        return currentTime;
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        SceneManager.LoadScene("Lose Screen");
    }
}