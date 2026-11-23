
using UnityEngine;

public class FloorParallax : MonoBehaviour
{
    [Header("Floor Settings")]
    [Min(0f)]
    public float speedMultiplier = 1f;

    [Header("Seam Settings")]
    [Min(0f)]
    public float overlap = 0.5f;

    private Transform firstTile;
    private Transform secondTile;

    private SpriteRenderer firstRenderer;
    private SpriteRenderer secondRenderer;

    private Camera mainCamera;

    private float tileWidth;

    private void Start()
    {
        if (transform.childCount < 2)
        {
            Debug.LogError("FloorParallax needs two child floor tiles!");
            enabled = false;
            return;
        }

        firstTile = transform.GetChild(0);
        secondTile = transform.GetChild(1);

        firstRenderer = firstTile.GetComponent<SpriteRenderer>();
        secondRenderer = secondTile.GetComponent<SpriteRenderer>();

        if (firstRenderer == null || secondRenderer == null)
        {
            Debug.LogError("Both floor tiles need SpriteRenderers!");
            enabled = false;
            return;
        }

        mainCamera = Camera.main;

        if (mainCamera == null || !mainCamera.orthographic)
        {
            Debug.LogError("An orthographic Main Camera is required!");
            enabled = false;
            return;
        }

        tileWidth = firstRenderer.bounds.size.x;

        // Position the second tile with a small overlap.
        secondTile.position = new Vector3(
            firstTile.position.x + tileWidth - overlap,
            firstTile.position.y,
            firstTile.position.z
        );
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        float movement =
            GameManager.Instance.gameSpeed *
            speedMultiplier *
            Time.deltaTime;

        firstTile.position += Vector3.left * movement;
        secondTile.position += Vector3.left * movement;

        Transform leftTile;
        Transform rightTile;

        SpriteRenderer leftRenderer;
        SpriteRenderer rightRenderer;

        if (firstTile.position.x < secondTile.position.x)
        {
            leftTile = firstTile;
            rightTile = secondTile;

            leftRenderer = firstRenderer;
            rightRenderer = secondRenderer;
        }
        else
        {
            leftTile = secondTile;
            rightTile = firstTile;

            leftRenderer = secondRenderer;
            rightRenderer = firstRenderer;
        }

        float cameraLeft =
            mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Recycle the tile after it leaves the camera.
        if (leftRenderer.bounds.max.x < cameraLeft)
        {
            leftTile.position = new Vector3(
                rightRenderer.bounds.max.x +
                leftRenderer.bounds.extents.x -
                overlap,
                leftTile.position.y,
                leftTile.position.z
            );
        }
    }
}
