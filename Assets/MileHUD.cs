
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MileHUD : MonoBehaviour
{
    [Header("HUD Text")]
    public TMP_Text levelText;
    public TMP_Text timerText;
    public TMP_Text staminaText;

    [Header("Stamina Bar")]
    public Image staminaFill;

    [Header("Gameplay Settings")]
    public bool showTimer = true;

    private void Start()
    {
        showTimer = PlayerPrefs.GetInt("ShowTimer", 1) == 1;
        UpdateTimerVisibility();
    }

    public void SetLevel(string mileName)
    {
        if (levelText != null)
            levelText.text = mileName;
    }

    public void SetTimer(float timeRemaining)
    {
        if (timerText == null)
            return;

        if (!showTimer)
            return;

        timeRemaining = Mathf.Max(0f, timeRemaining);

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SetStamina(float currentStamina, float maxStamina)
    {
        float percentage = maxStamina > 0f
            ? Mathf.Clamp01(currentStamina / maxStamina)
            : 0f;

        if (staminaText != null)
        {
            staminaText.text =
                Mathf.RoundToInt(percentage * 100f) + "%";
        }

        if (staminaFill != null)
        {
            staminaFill.fillAmount = percentage;
        }
    }

    public void SetTimerVisibility(bool visible)
    {
        showTimer = visible;
        UpdateTimerVisibility();
    }

    private void UpdateTimerVisibility()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(showTimer);
    }
}
