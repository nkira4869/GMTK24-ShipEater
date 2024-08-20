using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the Pause Menu UI panel
    public GameObject creditsPanelUI; // Reference to the Credits panel UI

    private bool isPaused = false;

    void Update()
    {
        // Toggle pause when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game time
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze the game time
        isPaused = true;
    }

    public void ShowCredits()
    {
        creditsPanelUI.SetActive(true); // Show the credits panel
    }

    public void HideCredits()
    {
        creditsPanelUI.SetActive(false); // Hide the credits panel
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Reset the time scale to normal before loading the main menu
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the name of your actual main menu scene
    }
}