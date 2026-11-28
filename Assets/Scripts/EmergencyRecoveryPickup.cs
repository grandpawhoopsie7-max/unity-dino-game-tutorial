
using UnityEngine;

public class EmergencyRecoveryPickup : MonoBehaviour
{
    [Header("Movement")]
    public float speedMultiplier = 1f;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsRunActive)
            return;

        transform.position += Vector3.left *
            GameManager.Instance.gameSpeed *
            speedMultiplier *
            Time.deltaTime;

        if (mainCamera != null && spriteRenderer != null)
        {
            float cameraLeft =
                mainCamera.transform.position.x -
                mainCamera.orthographicSize * mainCamera.aspect;

            if (spriteRenderer.bounds.max.x < cameraLeft - 2f)
            {
                Destroy(gameObject);
            }
        }
    }
}
