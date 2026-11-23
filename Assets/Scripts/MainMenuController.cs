using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Scene Settings")]
    public string gameSceneName = "Game";

    private void Start()
    {
        // Make sure secondary panels are closed.
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("Exit button pressed.");
    }
}
