using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatSpawner : MonoBehaviour
{
    [System.Serializable]
    private class MeatSpawnOption
    {
        public GameObject prefab;

        [Min(0f)]
        public float weight = 1f;
    }

    [Header("Meat Variants")]
    [SerializeField] private MeatSpawnOption[] meatOptions;

    [Header("Object Pool")]
    [SerializeField] private AirObjectPool objectPool;
    [SerializeField] private bool useObjectPool = true;

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

    private void Awake()
    {
        if (objectPool == null)
        {
            objectPool = FindFirstObjectByType<AirObjectPool>();
        }
    }

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
        GameObject selectedPrefab = SelectMeatPrefab();

        if (selectedPrefab == null)
        {
            return;
        }

        int lane = UnityEngine.Random.Range(minLane, maxLane + 1);
        int heightLevel = UnityEngine.Random.Range(minHeightLevel, maxHeightLevel + 1);

        float spawnX = lane * laneDistance;
        float spawnY = baseHeight + heightLevel * heightStep;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        if (useObjectPool && objectPool != null)
        {
            objectPool.Get(selectedPrefab, spawnPosition, Quaternion.identity);
            return;
        }

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    private GameObject SelectMeatPrefab()
    {
        if (meatOptions == null || meatOptions.Length == 0)
        {
            Debug.LogWarning("[AirMeatSpawner] No meat spawn options assigned.");
            return null;
        }

        float totalWeight = 0f;

        foreach (MeatSpawnOption option in meatOptions)
        {
            if (option == null || option.prefab == null || option.weight <= 0f)
            {
                continue;
            }

            totalWeight += option.weight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("[AirMeatSpawner] Total meat spawn weight must be greater than zero.");
            return null;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (MeatSpawnOption option in meatOptions)
        {
            if (option == null || option.prefab == null || option.weight <= 0f)
            {
                continue;
            }

            currentWeight += option.weight;

            if (randomValue <= currentWeight)
            {
                return option.prefab;
            }
        }

        return null;
    }
}