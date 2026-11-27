using UnityEngine;

public class MenuParallaxLayer : MonoBehaviour
{
    [Header("Two repeating sprites")]
    public Transform firstTile;
    public Transform secondTile;

    [Header("Movement")]
    public float speed = 0.2f;

    private Camera mainCamera;
    private float tileWidth;

    private void Start()
    {
        mainCamera = Camera.main;

        SpriteRenderer firstRenderer = firstTile.GetComponent<SpriteRenderer>();

        if (mainCamera == null || firstRenderer == null)
        {
            Debug.LogError("MenuParallaxLayer needs a Main Camera and SpriteRenderer.");
            enabled = false;
            return;
        }

        tileWidth = firstRenderer.bounds.size.x;
    }

    private void Update()
    {
        if (mainCamera == null || tileWidth <= 0f)
            return;

        float movement = speed * Time.deltaTime;

        firstTile.position += Vector3.left * movement;
        secondTile.position += Vector3.left * movement;

        float cameraLeft = mainCamera.transform.position.x
            - mainCamera.orthographicSize * mainCamera.aspect;

        if (firstTile.GetComponent<SpriteRenderer>().bounds.max.x < cameraLeft)
        {
            float rightEdge = secondTile.position.x + tileWidth;
            firstTile.position = new Vector3(
                rightEdge,
                firstTile.position.y,
                firstTile.position.z
            );
        }

        if (secondTile.GetComponent<SpriteRenderer>().bounds.max.x < cameraLeft)
        {
            float rightEdge = firstTile.position.x + tileWidth;
            secondTile.position = new Vector3(
                rightEdge,
                secondTile.position.y,
                secondTile.position.z
            );
        }
    }
}