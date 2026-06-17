using UnityEngine;

public abstract class AirMeatEffectBase : MonoBehaviour, IAirMeatEffect
{
    public abstract void Apply(GameObject collector);

    protected bool TryGetCollectorComponent<T>(GameObject collector, out T component) where T : Component
    {
        component = null;

        if (collector == null)
        {
            return false;
        }

        if (collector.TryGetComponent(out component))
        {
            return true;
        }

        component = collector.GetComponentInParent<T>();
        if (component != null)
        {
            return true;
        }

        component = collector.GetComponentInChildren<T>();
        return component != null;
    }
}