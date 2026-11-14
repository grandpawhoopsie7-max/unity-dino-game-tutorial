
using UnityEngine;

public class WaterBottle : MonoBehaviour
{
    private float leftEdge;

    private void Start()
    {
        if (Camera.main != null)
        {
            leftEdge = Camera.main.ScreenToWorldPoint(
                new Vector3(0f, 0f, 0f)
            ).x - 2f;
        }
        else
        {
            leftEdge = -15f;
        }
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;

        if (manager == null || !manager.IsRunActive)
            return;

        transform.position += Vector3.left *
            manager.gameSpeed *
            Time.deltaTime;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
