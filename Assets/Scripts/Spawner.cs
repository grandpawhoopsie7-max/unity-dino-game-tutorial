
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

    [Header("Rare Emergency Recovery Pickup")]
    public GameObject emergencyRecoveryPrefab;

    [Range(0f, 1f)]
    public float emergencyRecoverySpawnChance = 0.01f;

    public float emergencyRecoveryVerticalOffset = 0f;

    [Header("Spawn Timing")]
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;

    [Header("Water Bottle Fairness")]
    public float minimumBottleSpacing = 8f;

    public float mile1BottleInterval = 20f;
    public float mile2BottleInterval = 25f;
    public float mile3BottleInterval = 30f;

    private float timeSinceLastBottle;

    private void OnEnable()
    {
        timeSinceLastBottle = 0f;
        ScheduleNextSpawn();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsRunActive)
            return;

        timeSinceLastBottle += Time.deltaTime;
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

    private float GetGuaranteedBottleInterval()
    {
        if (GameManager.Instance == null)
            return mile1BottleInterval;

        switch (GameManager.Instance.CurrentMile)
        {
            case 1:
                return mile1BottleInterval;

            case 2:
                return mile2BottleInterval;

            case 3:
                return mile3BottleInterval;

            default:
                return mile3BottleInterval;
        }
    }

    private void ScheduleNextSpawn()
    {
        Invoke(
            nameof(Spawn),
            Random.Range(minSpawnRate, maxSpawnRate)
        );
    }

    private void Spawn()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsRunActive)
            return;

        // Guarantee a bottle if enough time has passed.
        if (timeSinceLastBottle >= GetGuaranteedBottleInterval())
        {
            SpawnWaterBottle();
            ScheduleNextSpawn();
            return;
        }

        // Rare emergency recovery pickup.
        if (emergencyRecoveryPrefab != null &&
            Random.value < emergencyRecoverySpawnChance)
        {
            SpawnEmergencyRecovery();
            ScheduleNextSpawn();
            return;
        }

        float roll = Random.value;

        // Special event: cactus and water bottle together.
        if (roll < combinedSpawnChance)
        {
            if (timeSinceLastBottle >= minimumBottleSpacing)
            {
                SpawnCombined();
            }
            else
            {
                SpawnIndividualEvent();
            }
        }
        else
        {
            SpawnIndividualEvent();
        }

        ScheduleNextSpawn();
    }

    private void SpawnEmergencyRecovery()
    {
        Vector3 position = transform.position +
            Vector3.up * emergencyRecoveryVerticalOffset;

        Instantiate(
            emergencyRecoveryPrefab,
            position,
            Quaternion.identity
        );

        Debug.Log("Rare Emergency Recovery pickup spawned!");
    }

    private void SpawnIndividualEvent()
    {
        float individualRoll = Random.value;
        SpawnIndividual(individualRoll);
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

        timeSinceLastBottle = 0f;
    }

    private void SpawnWaterBottle()
    {
        if (waterBottlePrefab == null)
            return;

        Vector3 position = transform.position;

        foreach (SpawnableObject obj in objects)
        {
            if (obj.prefab == waterBottlePrefab)
            {
                position += Vector3.up * obj.verticalOffset;
                break;
            }
        }

        Instantiate(
            waterBottlePrefab,
            position,
            Quaternion.identity
        );

        timeSinceLastBottle = 0f;
    }

    private void SpawnIndividual(float roll)
    {
        foreach (SpawnableObject obj in objects)
        {
            if (roll < obj.spawnChance)
            {
                if (obj.prefab != null)
                {
                    if (obj.prefab == waterBottlePrefab &&
                        timeSinceLastBottle < minimumBottleSpacing)
                    {
                        return;
                    }

                    Vector3 position = transform.position +
                        Vector3.up * obj.verticalOffset;

                    Instantiate(
                        obj.prefab,
                        position,
                        Quaternion.identity
                    );

                    if (obj.prefab == waterBottlePrefab)
                    {
                        timeSinceLastBottle = 0f;
                    }
                }

                return;
            }

            roll -= obj.spawnChance;
        }
    }
}
