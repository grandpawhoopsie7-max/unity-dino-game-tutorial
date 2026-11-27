
using UnityEngine;

public class OptionsTabController : MonoBehaviour
{
    [Header("Settings Panels")]
    public GameObject audioSettingsPanel;
    public GameObject displaySettingsPanel;
    public GameObject gameplaySettingsPanel;

    private void Start()
    {
        ShowAudioSettings();
    }

    public void ShowAudioSettings()
    {
        audioSettingsPanel.SetActive(true);
        displaySettingsPanel.SetActive(false);
        gameplaySettingsPanel.SetActive(false);
    }

    public void ShowDisplaySettings()
    {
        audioSettingsPanel.SetActive(false);
        displaySettingsPanel.SetActive(true);
        gameplaySettingsPanel.SetActive(false);
    }

    public void ShowGameplaySettings()
    {
        audioSettingsPanel.SetActive(false);
        displaySettingsPanel.SetActive(false);
        gameplaySettingsPanel.SetActive(true);
    }
}
