using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] GameObject tutorialUI;
    [SerializeField] GameObject firstButton;
    [SerializeField] AudioSource levelMusic;

    void Start()
    {
        // Show tutorial and pause game immediately
        tutorialUI.SetActive(true);
        Time.timeScale = 0;

        // Pause level music if playing
        if (levelMusic != null && levelMusic.isPlaying)
        {
            levelMusic.Pause();
        }

        // Enable cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Highlight first button (controller support)
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void CloseTutorial()
    {
        // Resume game
        Time.timeScale = 1;
        tutorialUI.SetActive(false);

        // Resume music
        if (levelMusic != null)
        {
            levelMusic.UnPause();
        }

        // Lock cursor back to game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}