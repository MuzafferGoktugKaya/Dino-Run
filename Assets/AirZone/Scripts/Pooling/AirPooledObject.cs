using UnityEngine;

[DisallowMultipleComponent]
public class AirPooledObject : MonoBehaviour
{
    private AirObjectPool ownerPool;
    private GameObject sourcePrefab;

    public GameObject SourcePrefab => sourcePrefab;

    public void Initialize(AirObjectPool pool, GameObject prefab)
    {
        ownerPool = pool;
        sourcePrefab = prefab;
    }

    public void ReturnToPool()
    {
        if (ownerPool != null)
        {
            ownerPool.Return(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}