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

    private float currentStamina;

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
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;

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

        if (manager == null || !manager.IsRunActive)
            return;

        currentStamina = Mathf.Max(
            0f,
            currentStamina - runningDrainPerSecond * Time.deltaTime
        );

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


    private void OnTriggerEnter(Collider other)
    {
        // Water bottles restore stamina.
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

            return;
        }

        // Obstacles cause Game Over.
        if (other.CompareTag("Obstacle"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}