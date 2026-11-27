
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreakFreeQTE : MonoBehaviour
{
    [Header("QTE UI")]
    public TMP_Text promptText;
    public TMP_Text instructionsText;
    public Slider progressSlider;

    [Header("QTE Settings")]
    public int requiredPresses = 6;
    public float timeLimit = 3f;
    public float exhaustionStaminaRestore = 35f;

    [Header("Gameplay Settings")]
    public bool showInstructions = true;

    [Header("References")]
    public Player player;

    private int currentPresses;
    private float timeRemaining;
    private bool qteRunning;
    private bool causedByExhaustion;

    public void StartQTE(bool exhaustion)
    {
        if (qteRunning)
            return;
        showInstructions =
    PlayerPrefs.GetInt("ShowQTEInstructions", 1) == 1;

        causedByExhaustion = exhaustion;
        currentPresses = 0;
        timeRemaining = timeLimit;
        qteRunning = true;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = requiredPresses;
            progressSlider.value = 0;
        }

        if (promptText != null)
            promptText.text = "YOU'VE FALLEN!";

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

            if (currentPresses >= requiredPresses)
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
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(showInstructions);

            if (showInstructions)
            {
                instructionsText.text =
                    "MASH SPACE TO GET UP! " +
                    currentPresses + "/" + requiredPresses;
            }
        }
    }

    public void SetInstructionsVisibility(bool visible)
    {
        showInstructions = visible;
        UpdateInstructions();
    }

    private void FinishQTE(bool succeeded)
    {
        qteRunning = false;
        Time.timeScale = 1f;
        gameObject.SetActive(false);

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
