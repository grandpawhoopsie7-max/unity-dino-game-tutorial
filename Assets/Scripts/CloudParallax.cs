
using UnityEngine;

public class CloudParallax : MonoBehaviour
{
    [Header("Cloud Settings")]
    [Min(0f)]
    public float speedMultiplier = 0.08f;

    [Min(0f)]
    public float spacing = 8f;

    private Transform firstCloud;
    private Transform secondCloud;

    private SpriteRenderer firstRenderer;
    private SpriteRenderer secondRenderer;

    private Camera mainCamera;

    private float cloudWidth;

    private void Start()
    {
        if (transform.childCount < 2)
        {
            Debug.LogError("CloudParallax needs two child sprites!");
            enabled = false;
            return;
        }

        firstCloud = transform.GetChild(0);
        secondCloud = transform.GetChild(1);

        firstRenderer = firstCloud.GetComponent<SpriteRenderer>();
        secondRenderer = secondCloud.GetComponent<SpriteRenderer>();

        if (firstRenderer == null || secondRenderer == null)
        {
            Debug.LogError("Both cloud objects need SpriteRenderers!");
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

        cloudWidth = firstRenderer.bounds.size.x;
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        float movement =
            GameManager.Instance.gameSpeed *
            speedMultiplier *
            Time.deltaTime;

        firstCloud.position += Vector3.left * movement;
        secondCloud.position += Vector3.left * movement;

        Transform leftCloud;
        Transform rightCloud;
        SpriteRenderer leftRenderer;

        if (firstCloud.position.x < secondCloud.position.x)
        {
            leftCloud = firstCloud;
            rightCloud = secondCloud;
            leftRenderer = firstRenderer;
        }
        else
        {
            leftCloud = secondCloud;
            rightCloud = firstCloud;
            leftRenderer = secondRenderer;
        }

        float cameraLeft =
            mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        if (leftRenderer.bounds.max.x < cameraLeft)
        {
            float newX =
                rightCloud.position.x + cloudWidth + spacing;

            leftCloud.position = new Vector3(
                newX,
                leftCloud.position.y,
                leftCloud.position.z
            );
        }
    }
}
