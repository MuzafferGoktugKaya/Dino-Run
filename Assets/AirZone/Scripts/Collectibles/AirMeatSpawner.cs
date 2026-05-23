using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatSpawner : MonoBehaviour
{
    [Header("Meat")]
    [SerializeField] private GameObject meatPrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 2.5f;
    [SerializeField] private float initialDelay = 1f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnZ = 25f;
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float baseHeight = 2f;
    [SerializeField] private float heightStep = 1.5f;

    [Header("Random Range")]
    [SerializeField] private int minLane = -1;
    [SerializeField] private int maxLane = 1;
    [SerializeField] private int minHeightLevel = 0;
    [SerializeField] private int maxHeightLevel = 2;

    private float spawnTimer;

    private void Start()
    {
        spawnTimer = -initialDelay;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnMeat();
            spawnTimer = 0f;
        }
    }

    private void SpawnMeat()
    {
        if (meatPrefab == null)
        {
            Debug.LogWarning("[AirMeatSpawner] Meat prefab is not assigned.");
            return;
        }

        int lane = Random.Range(minLane, maxLane + 1);
        int heightLevel = Random.Range(minHeightLevel, maxHeightLevel + 1);

        float spawnX = lane * laneDistance;
        float spawnY = baseHeight + heightLevel * heightStep;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        Instantiate(meatPrefab, spawnPosition, Quaternion.identity);
    }
}