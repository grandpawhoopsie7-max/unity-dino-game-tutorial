
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject tutorialPanel;

    [Header("Scene Settings")]
    public string gameSceneName = "Game";

    private void Start()
    {
        // Make sure all secondary panels are closed.
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        optionsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("Exit button pressed.");
    }
}
