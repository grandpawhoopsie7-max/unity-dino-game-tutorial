
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingsManager : MonoBehaviour
{
    [Header("Gameplay Toggles")]
    public Toggle environmentalEffectsToggle;
    public Toggle showTimerToggle;
    public Toggle qteInstructionsToggle;

    private const string ENVIRONMENT_KEY = "EnvironmentalEffectsEnabled";
    private const string TIMER_KEY = "ShowTimer";
    private const string QTE_KEY = "ShowQTEInstructions";

    private void Start()
    {
        // Load saved preferences. All settings default to ON.
        bool environmentalEffects = PlayerPrefs.GetInt(ENVIRONMENT_KEY, 1) == 1;
        bool showTimer = PlayerPrefs.GetInt(TIMER_KEY, 1) == 1;
        bool showQTEInstructions = PlayerPrefs.GetInt(QTE_KEY, 1) == 1;

        // Set initial toggle states before adding listeners.
        environmentalEffectsToggle.isOn = environmentalEffects;
        showTimerToggle.isOn = showTimer;
        qteInstructionsToggle.isOn = showQTEInstructions;

        // Listen for changes.
        environmentalEffectsToggle.onValueChanged.AddListener(SetEnvironmentalEffects);
        showTimerToggle.onValueChanged.AddListener(SetShowTimer);
        qteInstructionsToggle.onValueChanged.AddListener(SetQTEInstructions);
    }

    public void SetEnvironmentalEffects(bool enabled)
    {
        PlayerPrefs.SetInt(ENVIRONMENT_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetShowTimer(bool enabled)
    {
        PlayerPrefs.SetInt(TIMER_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQTEInstructions(bool enabled)
    {
        PlayerPrefs.SetInt(QTE_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetGameplayToDefaults()
    {
        environmentalEffectsToggle.isOn = true;
        showTimerToggle.isOn = true;
        qteInstructionsToggle.isOn = true;

        PlayerPrefs.SetInt(ENVIRONMENT_KEY, 1);
        PlayerPrefs.SetInt(TIMER_KEY, 1);
        PlayerPrefs.SetInt(QTE_KEY, 1);

        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (environmentalEffectsToggle != null)
            environmentalEffectsToggle.onValueChanged.RemoveListener(SetEnvironmentalEffects);

        if (showTimerToggle != null)
            showTimerToggle.onValueChanged.RemoveListener(SetShowTimer);

        if (qteInstructionsToggle != null)
            qteInstructionsToggle.onValueChanged.RemoveListener(SetQTEInstructions);
    }
}
