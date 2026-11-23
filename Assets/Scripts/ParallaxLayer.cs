
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Min(0f)]
    public float speedMultiplier = 0.35f;

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
            Debug.LogError("ParallaxLayer needs two child sprites!");
            enabled = false;
            return;
        }

        firstTile = transform.GetChild(0);
        secondTile = transform.GetChild(1);

        firstRenderer = firstTile.GetComponent<SpriteRenderer>();
        secondRenderer = secondTile.GetComponent<SpriteRenderer>();

        if (firstRenderer == null || secondRenderer == null)
        {
            Debug.LogError("Both mountain tiles need SpriteRenderers!");
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

        // Find which tile is currently on the left.
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

        // Find the camera's left boundary.
        float cameraLeft =
            mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Only wrap after the entire left tile leaves the camera.
        if (leftRenderer.bounds.max.x < cameraLeft)
        {
            leftTile.position = new Vector3(
                rightTile.position.x + tileWidth,
                leftTile.position.y,
                leftTile.position.z
            );
        }
    }
}
