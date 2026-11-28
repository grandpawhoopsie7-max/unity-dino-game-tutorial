
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreakFreeQTE : MonoBehaviour
{
    [Header("QTE UI")]
    public TMP_Text promptText;
    public TMP_Text instructionsText;
    public Slider progressSlider;

    [Header("Break Free Settings")]
    public int requiredPresses = 6;
    public float timeLimit = 3f;
    public float exhaustionStaminaRestore = 35f;

    [Header("Emergency Recovery Settings")]
    public int emergencyRequiredPresses = 10;
    public float emergencyTimeLimit = 4f;
    public float emergencyStaminaRestore = 25f;

    [Header("Gameplay Settings")]
    public bool showInstructions = true;

    [Header("References")]
    public Player player;

    private int currentPresses;
    private int activeRequiredPresses;
    private float timeRemaining;
    private bool qteRunning;
    private bool causedByExhaustion;
    private bool isEmergencyQTE;

    public void StartQTE(bool exhaustion)
    {
        if (qteRunning)
            return;

        showInstructions =
            PlayerPrefs.GetInt("ShowQTEInstructions", 1) == 1;

        isEmergencyQTE = false;
        causedByExhaustion = exhaustion;

        activeRequiredPresses = requiredPresses;
        timeRemaining = timeLimit;

        BeginQTE();
    }

    public void StartEmergencyQTE()
    {
        if (qteRunning)
            return;

        showInstructions =
            PlayerPrefs.GetInt("ShowQTEInstructions", 1) == 1;

        isEmergencyQTE = true;
        causedByExhaustion = false;

        activeRequiredPresses = emergencyRequiredPresses;
        timeRemaining = emergencyTimeLimit;

        BeginQTE();
    }

    private void BeginQTE()
    {
        currentPresses = 0;
        qteRunning = true;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = activeRequiredPresses;
            progressSlider.value = 0;
        }

        if (promptText != null)
        {
            promptText.text = isEmergencyQTE
                ? "FIGHT THROUGH EXHAUSTION!"
                : "YOU'VE FALLEN!";
        }

        UpdateInstructions();

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!qteRunning)
            return;

        timeRemaining -= Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPresses++;

            if (progressSlider != null)
                progressSlider.value = currentPresses;

            UpdateInstructions();

            if (currentPresses >= activeRequiredPresses)
            {
                FinishQTE(true);
                return;
            }
        }

        if (timeRemaining <= 0f)
        {
            FinishQTE(false);
        }
    }

    private void UpdateInstructions()
    {
        if (instructionsText == null)
            return;

        instructionsText.gameObject.SetActive(showInstructions);

        if (!showInstructions)
            return;

        if (isEmergencyQTE)
        {
            instructionsText.text =
                "MASH SPACE TO RECOVER! " +
                currentPresses + "/" + activeRequiredPresses;
        }
        else
        {
            instructionsText.text =
                "MASH SPACE TO GET UP! " +
                currentPresses + "/" + activeRequiredPresses;
        }
    }

    public void SetInstructionsVisibility(bool visible)
    {
        showInstructions = visible;
        UpdateInstructions();
    }

    private void FinishQTE(bool succeeded)
    {
        bool wasEmergency = isEmergencyQTE;

        qteRunning = false;
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        if (wasEmergency)
        {
            if (player != null)
                player.FinishEmergencyRecovery(succeeded);

            return;
        }

        if (succeeded)
        {
            float staminaRestore =
                causedByExhaustion ? exhaustionStaminaRestore : 0f;

            if (player != null)
                player.RecoverFromTrip(staminaRestore);
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }

    private void OnDisable()
    {
        if (qteRunning)
        {
            qteRunning = false;
            Time.timeScale = 1f;
        }
    }
}
