using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatCollectible : MonoBehaviour
{
    [Header("Composite Effects")]
    [SerializeField] private bool destroyAfterCollect = true;

    private AirMeatEffectBase[] meatEffects;
    private bool isCollected;

    private void Awake()
    {
        meatEffects = GetComponents<AirMeatEffectBase>();

        if (meatEffects.Length == 0)
        {
            Debug.LogWarning($"[AirMeatCollectible] No meat effects found on {gameObject.name}.");
        }
    }

    private void OnEnable()
    {
        isCollected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
        {
            return;
        }

        if (!CanCollectorReceiveMeat(other))
        {
            return;
        }

        isCollected = true;
        ApplyEffects(other.gameObject);

        if (destroyAfterCollect)
        {
            ReturnToPoolOrDestroy();
        }
    }

    private void ApplyEffects(GameObject collector)
    {
        if (meatEffects == null || meatEffects.Length == 0)
        {
            Debug.LogWarning($"[AirMeatCollectible] {gameObject.name} was collected but has no effects.");
            return;
        }

        foreach (AirMeatEffectBase meatEffect in meatEffects)
        {
            if (meatEffect == null || !meatEffect.isActiveAndEnabled)
            {
                continue;
            }

            meatEffect.Apply(collector);
        }
    }

    private void ReturnToPoolOrDestroy()
    {
        if (TryGetComponent(out AirPooledObject pooledObject))
        {
            pooledObject.ReturnToPool();
            return;
        }

        Destroy(gameObject);
    }

    private bool CanCollectorReceiveMeat(Collider collectorCollider)
    {
        if (collectorCollider == null)
        {
            return false;
        }

        if (collectorCollider.GetComponentInParent<IAirMeatConsumer>() != null)
        {
            return true;
        }

        if (collectorCollider.GetComponentInParent<AirStaminaController>() != null)
        {
            return true;
        }

        return false;
    }
}