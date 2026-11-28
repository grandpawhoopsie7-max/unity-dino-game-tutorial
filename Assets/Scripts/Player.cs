using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;

    [Header("Movement")]
    public float jumpForce = 8f;
    public float gravity = 9.81f * 2f;

    [Header("Stamina")]
    [Min(0f)] public float maxStamina = 100f;
    [Min(0f)] public float runningDrainPerSecond = 0.08f;
    [Min(0f)] public float jumpStaminaCost = 6f;
    [Min(0f)] public float waterBottleRestore = 25f;

    [Header("Break Free QTE")]
    public BreakFreeQTE breakFreeQTE;

    [Header("Emergency Recovery")]
    [Range(0.01f, 1f)]
    public float emergencyActivationThreshold = 0.25f;

    [Min(0f)]
    public float emergencyStaminaRestore = 25f;

    public KeyCode emergencyRecoveryKey = KeyCode.E;

    [Tooltip("The full panel shown when Emergency Recovery is ready.")]
    public GameObject emergencyPromptPanel;

    [Tooltip("The TMP text inside the Emergency Recovery panel.")]
    public TMP_Text emergencyPromptText;

    [Header("Emergency Charge Marker")]
    public UnityEngine.UI.Image emergencyChargeMarker;

    private float currentStamina;
    private bool isTripping;
    private bool emergencyChargeAvailable = true;
    private bool emergencyQTEActive;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool HasEmergencyCharge => emergencyChargeAvailable;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
        isTripping = false;
        emergencyQTEActive = false;

        UpdateEmergencyPrompt();
        UpdateEmergencyChargeMarker();
    }

    private void OnDisable()
    {
        if (emergencyPromptPanel != null)
            emergencyPromptPanel.SetActive(false);
        else if (emergencyPromptText != null)
            emergencyPromptText.gameObject.SetActive(false);
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        isTripping = false;
        emergencyQTEActive = false;
        emergencyChargeAvailable = true;

        UpdateEmergencyPrompt();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateStaminaHud(
                currentStamina,
                maxStamina
            );
        }
        UpdateEmergencyChargeMarker();
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;

        if (manager == null || !manager.IsRunActive ||
            isTripping || emergencyQTEActive)
        {
            return;
        }

        UpdateEmergencyPrompt();

        // Activate Emergency Recovery when stamina is low enough.
        if (emergencyChargeAvailable &&
            currentStamina <= maxStamina * emergencyActivationThreshold &&
            Input.GetKeyDown(emergencyRecoveryKey))
        {
            BeginEmergencyRecovery();
          
            return;
        }

        currentStamina = Mathf.Max(
            0f,
            currentStamina - runningDrainPerSecond * Time.deltaTime
        );

        if (currentStamina <= 0f)
        {
            BeginTrip(true);
            return;
        }

        direction += gravity * Time.deltaTime * Vector3.down;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            if (Input.GetButtonDown("Jump") &&
                currentStamina >= jumpStaminaCost)
            {
                currentStamina -= jumpStaminaCost;
                direction = Vector3.up * jumpForce;
            }
        }

        character.Move(direction * Time.deltaTime);

        manager.UpdateStaminaHud(currentStamina, maxStamina);
        UpdateEmergencyPrompt();
    }

    private void BeginTrip(bool exhaustion)
    {
        if (isTripping)
            return;

        isTripping = true;
        direction = Vector3.zero;

        UpdateEmergencyPrompt();

        if (breakFreeQTE != null)
        {
            breakFreeQTE.StartQTE(exhaustion);
        }
        else
        {
            Debug.LogError(
                "Break Free QTE is not assigned on the Player!"
            );

            isTripping = false;
        }
    }

    private void BeginEmergencyRecovery()
    {
        if (!emergencyChargeAvailable || breakFreeQTE == null)
            return;

        // Consume the charge when the challenge begins.
        emergencyChargeAvailable = false;
        emergencyQTEActive = true;
        direction = Vector3.zero;

        UpdateEmergencyPrompt();
        UpdateEmergencyChargeMarker();

        breakFreeQTE.StartEmergencyQTE();
    }

    public void FinishEmergencyRecovery(bool succeeded)
    {
        emergencyQTEActive = false;

        if (succeeded)
        {
            RecoverFromTrip(emergencyStaminaRestore);
        }
        else
        {
            direction = Vector3.zero;
            UpdateEmergencyPrompt();
        }
    }

    public void RechargeEmergencyRecovery()
    {
        if (emergencyChargeAvailable)
            return;

        emergencyChargeAvailable = true;
        UpdateEmergencyPrompt();
        UpdateEmergencyChargeMarker();

        Debug.Log("Emergency Recovery recharged!");

    }

    private void UpdateEmergencyPrompt()
    {
        bool canActivate =
            emergencyChargeAvailable &&
            currentStamina <= maxStamina * emergencyActivationThreshold &&
            !isTripping &&
            !emergencyQTEActive;

        if (emergencyPromptPanel != null)
            emergencyPromptPanel.SetActive(canActivate);
        else if (emergencyPromptText != null)
            emergencyPromptText.gameObject.SetActive(canActivate);

        if (emergencyPromptText != null && canActivate)
        {
            emergencyPromptText.text =
                "EMERGENCY RECOVERY READY\n[E] PRESS TO ACTIVATE";
        }
    }

    public void RecoverFromTrip(float staminaRestore)
    {
        currentStamina = Mathf.Clamp(
            currentStamina + staminaRestore,
            0f,
            maxStamina
        );

        isTripping = false;
        emergencyQTEActive = false;
        direction = Vector3.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateStaminaHud(
                currentStamina,
                maxStamina
            );
        }

        UpdateEmergencyPrompt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterBottle"))
        {
            currentStamina = Mathf.Min(
                maxStamina,
                currentStamina + waterBottleRestore
            );

            Destroy(other.gameObject);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateStaminaHud(
                    currentStamina,
                    maxStamina
                );
            }

            UpdateEmergencyPrompt();
            return;
        }

        EmergencyRecoveryPickup recoveryPickup =
            other.GetComponent<EmergencyRecoveryPickup>();

        if (recoveryPickup != null)
        {
            RechargeEmergencyRecovery();
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Obstacle"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }
    private void UpdateEmergencyChargeMarker()
    {
        if (emergencyChargeMarker == null)
            return;

        emergencyChargeMarker.color = emergencyChargeAvailable
            ? Color.white
            : Color.gray;
    }
}