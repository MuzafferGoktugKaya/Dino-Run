using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public LevelData currentLevel;
    public Transform player;
    public float startZ = 20f;
    public float spawnStep = 15f;
    public float spawnDistanceAhead = 120f;
    public float laneDistance = 3f;

    public GameObject blackCoinPrefab;

    private float currentZ;

    void Start() { currentZ = startZ; }
    void Update() { SpawnUntilAhead(); }

    void SpawnUntilAhead()
    {
        if (player == null || currentLevel == null) return;
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

        GameObject prefabToSpawn = null;
        float spawnY = 0f;
        bool isBlackCoin = false;

        if (currentLevel.wormPrefab != null && Random.value < currentLevel.wormSpawnChance)
        {
            prefabToSpawn = currentLevel.wormPrefab;
            spawnY = 0f;
        }
        else if (blackCoinPrefab != null && Random.value < 0.05f) // %5 şansla senin siyah coin
        {
            prefabToSpawn = blackCoinPrefab;
            spawnY = currentLevel.coinY;
            isBlackCoin = true;
        }
        else if (randomValue < 0.20f)
        {
            prefabToSpawn = currentLevel.coinPrefab;
            spawnY = currentLevel.coinY;
        }
        else if (randomValue < 0.40f)
        {
            prefabToSpawn = currentLevel.ObstaclePrefab1;
            spawnY = currentLevel.obstacle1Y;
        }
        else if (randomValue < 0.60f)
        {
            prefabToSpawn = currentLevel.ObstaclePrefab2;
            spawnY = currentLevel.obstacle2Y;
        }

        if (prefabToSpawn != null)
        {
            GameObject go = Instantiate(prefabToSpawn, new Vector3(laneX, spawnY, currentZ), Quaternion.identity);

            if (isBlackCoin)
            {
                go.tag = "BlackCoin";
            }
            else
            {
                go.tag = "Obstacle";
            }
        }
    }

    public void ClearExistingObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in obstacles) Destroy(obj);
        currentZ = player.position.z + 15f;
    }
}