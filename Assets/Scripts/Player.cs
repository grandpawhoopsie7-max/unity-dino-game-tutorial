
using UnityEngine;

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

    private float currentStamina;
    private bool isTripping;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
        isTripping = false;
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        isTripping = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateStaminaHud(
                currentStamina, maxStamina
            );
        }
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;

        if (manager == null || !manager.IsRunActive || isTripping)
            return;

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
    }

    private void BeginTrip(bool exhaustion)
    {
        if (isTripping)
            return;

        isTripping = true;
        direction = Vector3.zero;

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

    public void RecoverFromTrip(float staminaRestore)
    {
        currentStamina = Mathf.Clamp(
            currentStamina + staminaRestore,
            0f,
            maxStamina
        );

        isTripping = false;
        direction = Vector3.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateStaminaHud(
                currentStamina, maxStamina
            );
        }
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
                    currentStamina, maxStamina
                );
            }

            return;
        }

        if (other.CompareTag("Obstacle"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }
}
