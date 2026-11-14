
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;

        [Range(0f, 1f)]
        public float spawnChance;

        public float verticalOffset;
    }

    [Header("Individual Objects")]
    public SpawnableObject[] objects;

    [Header("Combined Spawn")]
    public GameObject cactusPrefab;
    public GameObject waterBottlePrefab;

    [Range(0f, 1f)]
    public float combinedSpawnChance = 0.10f;

    public float bottleHeightAboveCactus = 1.5f;

    [Header("Spawn Timing")]
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;

    private void OnEnable()
    {
        ScheduleNextSpawn();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void SetSpawnRates(float minimum, float maximum)
    {
        minSpawnRate = Mathf.Max(0.1f, minimum);
        maxSpawnRate = Mathf.Max(minSpawnRate, maximum);

        if (isActiveAndEnabled)
        {
            CancelInvoke(nameof(Spawn));
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void Spawn()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsRunActive)
            return;

        float roll = Random.value;

        // Special event: spawn cactus and bottle together.
        if (roll < combinedSpawnChance)
        {
            SpawnCombined();
        }
        else
        {
            SpawnIndividual(roll - combinedSpawnChance);
        }

        ScheduleNextSpawn();
    }

    private void SpawnCombined()
    {
        if (cactusPrefab == null || waterBottlePrefab == null)
            return;

        Vector3 cactusPosition = transform.position;

        Instantiate(
            cactusPrefab,
            cactusPosition,
            Quaternion.identity
        );

        Vector3 bottlePosition = cactusPosition +
            Vector3.up * bottleHeightAboveCactus;

        Instantiate(
            waterBottlePrefab,
            bottlePosition,
            Quaternion.identity
        );
    }

    private void SpawnIndividual(float roll)
    {
        foreach (SpawnableObject obj in objects)
        {
            if (roll < obj.spawnChance)
            {
                if (obj.prefab != null)
                {
                    Vector3 position = transform.position +
                        Vector3.up * obj.verticalOffset;

                    Instantiate(
                        obj.prefab,
                        position,
                        Quaternion.identity
                    );
                }

                return;
            }

            roll -= obj.spawnChance;
        }
    }
}
