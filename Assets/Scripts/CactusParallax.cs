
using UnityEngine;

public class CactusParallax : MonoBehaviour
{
    [Header("Cactus Settings")]
    [Min(0f)]
    public float speedMultiplier = 0.7f;

    [Min(0f)]
    public float spacing = 5f;

    private Transform[] cacti = new Transform[3];
    private SpriteRenderer[] renderers = new SpriteRenderer[3];

    private Camera mainCamera;

    private void Start()
    {
        if (transform.childCount < 3)
        {
            Debug.LogError("CactusParallax needs three child cactus objects!");
            enabled = false;
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            cacti[i] = transform.GetChild(i);
            renderers[i] = cacti[i].GetComponent<SpriteRenderer>();

            if (renderers[i] == null)
            {
                Debug.LogError("Every cactus needs a SpriteRenderer!");
                enabled = false;
                return;
            }
        }

        mainCamera = Camera.main;

        if (mainCamera == null || !mainCamera.orthographic)
        {
            Debug.LogError("An orthographic Main Camera is required!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        float movement =
            GameManager.Instance.gameSpeed *
            speedMultiplier *
            Time.deltaTime;

        // Move all three cactuses to the left.
        for (int i = 0; i < 3; i++)
        {
            cacti[i].position += Vector3.left * movement;
        }

        // Find the leftmost and rightmost cactus.
        int leftIndex = 0;
        int rightIndex = 0;

        for (int i = 1; i < 3; i++)
        {
            if (renderers[i].bounds.min.x <
                renderers[leftIndex].bounds.min.x)
            {
                leftIndex = i;
            }

            if (renderers[i].bounds.max.x >
                renderers[rightIndex].bounds.max.x)
            {
                rightIndex = i;
            }
        }

        float cameraLeft =
            mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Reposition the leftmost cactus after it leaves the screen.
        if (renderers[leftIndex].bounds.max.x < cameraLeft)
        {
            float newLeftEdge =
                renderers[rightIndex].bounds.max.x + spacing;

            float newCenterX =
                newLeftEdge + renderers[leftIndex].bounds.extents.x;

            cacti[leftIndex].position = new Vector3(
                newCenterX,
                cacti[leftIndex].position.y,
                cacti[leftIndex].position.z
            );
        }
    }
}
