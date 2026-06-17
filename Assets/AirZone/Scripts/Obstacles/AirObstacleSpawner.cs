using UnityEngine;

public class AirObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Object Pool")]
    [SerializeField] private AirObjectPool objectPool;
    [SerializeField] private bool useObjectPool = true;

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

    private void Awake()
    {
        if (objectPool == null)
        {
            objectPool = FindFirstObjectByType<AirObjectPool>();
        }
    }

    private void Update()
    {
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
        int randomLane = Random.Range(minLane, maxLane + 1);
        int randomHeightLevel = Random.Range(minHeightLevel, maxHeightLevel + 1);

        float spawnX = randomLane * laneDistance;
        float spawnY = baseHeight + randomHeightLevel * heightStep;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        GameObject obstacleInstance;

        if (useObjectPool && objectPool != null)
        {
            obstacleInstance = objectPool.Get(obstaclePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            obstacleInstance = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        }

        if (obstacleInstance != null &&
            obstacleInstance.TryGetComponent(out AirObstacle obstacle))
        {
            obstacle.SetDifficultySpeedMultiplier(obstacleSpeedMultiplier);
        }
    }
}