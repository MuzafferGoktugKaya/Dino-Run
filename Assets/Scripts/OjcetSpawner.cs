using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject jumpObstaclePrefab;
    public GameObject slideObstaclePrefab;
    public GameObject coinPrefab;

    [Header("Spawn Settings")]
    public float startZ = 20f;
    public float spawnStep = 8f;
    public float spawnDistanceAhead = 120f;

    [Header("Lane Settings")]
    public float laneDistance = 3f;

    [Header("Spawn Chances")]
    [Range(0f, 1f)]
    public float coinChance = 0.35f;

    [Range(0f, 1f)]
    public float slideObstacleChance = 0.25f;

    private float currentZ;

    void Start()
    {
        currentZ = startZ;
        SpawnUntilAhead();
    }

    void Update()
    {
        SpawnUntilAhead();
    }

    void SpawnUntilAhead()
    {
        if (player == null) return;

        while (currentZ < player.position.z + spawnDistanceAhead)
        {
            SpawnNext();
            currentZ += spawnStep;
        }
    }

    void SpawnNext()
    {
        int randomLane = Random.Range(0, 3);
        float laneX = (randomLane - 1) * laneDistance;

        float randomValue = Random.value;

        if (randomValue < coinChance)
        {
            Vector3 coinPosition = new Vector3(laneX, 1f, currentZ);
            Instantiate(coinPrefab, coinPosition, Quaternion.identity);
        }
        else if (randomValue < coinChance + slideObstacleChance)
        {
            Vector3 slideObstaclePosition = new Vector3(laneX, 2.2f, currentZ);
            Instantiate(slideObstaclePrefab, slideObstaclePosition, Quaternion.identity);
        }
        else
        {
            Vector3 jumpObstaclePosition = new Vector3(laneX, 0.5f, currentZ);
            Instantiate(jumpObstaclePrefab, jumpObstaclePosition, Quaternion.identity);
        }
    }
}