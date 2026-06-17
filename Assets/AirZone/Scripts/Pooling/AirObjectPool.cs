using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AirObjectPool : MonoBehaviour
{
    [Serializable]
    private class PoolEntry
    {
        public GameObject prefab;

        [Min(0)]
        public int prewarmCount = 5;
    }

    [Header("Pool Setup")]
    [SerializeField] private PoolEntry[] poolEntries;
    [SerializeField] private Transform pooledParent;

    private readonly Dictionary<GameObject, Queue<GameObject>> poolsByPrefab =
        new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (pooledParent == null)
        {
            pooledParent = transform;
        }

        PrewarmPools();
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[AirObjectPool] Cannot get pooled object because prefab is null.");
            return null;
        }

        EnsurePoolExists(prefab);

        Queue<GameObject> pool = poolsByPrefab[prefab];

        GameObject instance = pool.Count > 0
            ? pool.Dequeue()
            : CreateInstance(prefab);

        instance.transform.SetParent(null);
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);

        return instance;
    }

    public void Return(GameObject instance)
    {
        if (instance == null)
        {
            return;
        }

        AirPooledObject pooledObject = instance.GetComponent<AirPooledObject>();

        if (pooledObject == null || pooledObject.SourcePrefab == null)
        {
            Destroy(instance);
            return;
        }

        GameObject sourcePrefab = pooledObject.SourcePrefab;

        EnsurePoolExists(sourcePrefab);

        instance.SetActive(false);
        instance.transform.SetParent(pooledParent);

        poolsByPrefab[sourcePrefab].Enqueue(instance);
    }

    private void PrewarmPools()
    {
        if (poolEntries == null)
        {
            return;
        }

        foreach (PoolEntry entry in poolEntries)
        {
            if (entry == null || entry.prefab == null)
            {
                continue;
            }

            EnsurePoolExists(entry.prefab);

            for (int i = 0; i < entry.prewarmCount; i++)
            {
                GameObject instance = CreateInstance(entry.prefab);
                poolsByPrefab[entry.prefab].Enqueue(instance);
            }
        }
    }

    private void EnsurePoolExists(GameObject prefab)
    {
        if (!poolsByPrefab.ContainsKey(prefab))
        {
            poolsByPrefab[prefab] = new Queue<GameObject>();
        }
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, pooledParent);

        AirPooledObject pooledObject = instance.GetComponent<AirPooledObject>();

        if (pooledObject == null)
        {
            pooledObject = instance.AddComponent<AirPooledObject>();
        }

        pooledObject.Initialize(this, prefab);

        instance.SetActive(false);
        return instance;
    }
}