using UnityEngine;

public class AirObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnZ = 25f;

    [Header("Lane Settings")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private int minLane = -1;
    [SerializeField] private int maxLane = 1;

    [Header("Height Settings")]
    [SerializeField] private float baseHeight = 2f;
    [SerializeField] private float heightStep = 1.5f;
    [SerializeField] private int minHeightLevel = 0;
    [SerializeField] private int maxHeightLevel = 2;

    private float spawnTimer;
    private float obstacleSpeedMultiplier = 1f;

    private void Update()
    {
        // Counts time between obstacle spawns.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.1f, interval);
    }

    public void SetObstacleSpeedMultiplier(float multiplier)
    {
        obstacleSpeedMultiplier = Mathf.Max(0f, multiplier);
    }

    private void SpawnObstacle()
    {
        // Selects a random lane and height level for the new obstacle.
        int randomLane = Random.Range(minLane, maxLane + 1);
        int randomHeightLevel = Random.Range(minHeightLevel, maxHeightLevel + 1);

        // Converts lane and height level values into world position.
        float spawnX = randomLane * laneDistance;
        float spawnY = baseHeight + randomHeightLevel * heightStep;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        // Creates the obstacle prefab at the calculated position.
        GameObject obstacleInstance = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

        if (obstacleInstance.TryGetComponent(out AirObstacle obstacle))
        {
            obstacle.SetDifficultySpeedMultiplier(obstacleSpeedMultiplier);
        }
    }
}