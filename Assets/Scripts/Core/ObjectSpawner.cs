using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public LevelData currentLevel;
    public Transform player;
    public float startZ = 20f;
    public float spawnStep = 15f;
    public float spawnDistanceAhead = 120f;
    public float laneDistance = 3f;

    private float currentZ;

    void Start()
    {
        currentZ = startZ;
    }

    void Update()
    {
        SpawnUntilAhead();
    }

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

        if (currentLevel.wormPrefab != null && Random.value < currentLevel.wormSpawnChance)
        {
            prefabToSpawn = currentLevel.wormPrefab;
            spawnY = 0f;
        }
        else if (randomValue < 0.20f)
        {
            prefabToSpawn = GetCoinPrefabForCurrentLevel();
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
            go.tag = "Obstacle";
        }
    }

    GameObject GetCoinPrefabForCurrentLevel()
    {
        if (currentLevel == null) return null;

        bool hasNegativeCoin =
            currentLevel.hellNegativeCoinChance > 0f &&
            currentLevel.hellNegativeCoinPrefab != null;

        if (hasNegativeCoin && Random.value < currentLevel.hellNegativeCoinChance)
        {
            return currentLevel.hellNegativeCoinPrefab;
        }

        bool hasHellSpecialCoins =
            currentLevel.hellSpecialCoinChance > 0f &&
            (currentLevel.hellSpeedCoinPrefab != null || currentLevel.hellJumpCoinPrefab != null);

        if (!hasHellSpecialCoins)
        {
            return currentLevel.coinPrefab;
        }

        if (Random.value > currentLevel.hellSpecialCoinChance)
        {
            return currentLevel.coinPrefab;
        }

        bool canSpawnSpeedCoin = currentLevel.hellSpeedCoinPrefab != null;
        bool canSpawnJumpCoin = currentLevel.hellJumpCoinPrefab != null;

        if (canSpawnSpeedCoin && canSpawnJumpCoin)
        {
            return Random.value < 0.5f
                ? currentLevel.hellSpeedCoinPrefab
                : currentLevel.hellJumpCoinPrefab;
        }

        if (canSpawnSpeedCoin)
        {
            return currentLevel.hellSpeedCoinPrefab;
        }

        if (canSpawnJumpCoin)
        {
            return currentLevel.hellJumpCoinPrefab;
        }

        return currentLevel.coinPrefab;
    }

    public void ClearExistingObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }

        if (player != null)
        {
            currentZ = player.position.z + 15f;
        }
    }
}