using UnityEngine;

[DisallowMultipleComponent]
public class AirGrowthMeatEffect : AirMeatEffectBase
{
    [Header("Growth Reward")]
    [SerializeField] private int meatAmount = 1;

    public override void Apply(GameObject collector)
    {
        if (meatAmount <= 0)
        {
            return;
        }

        if (!TryGetMeatConsumer(collector, out IAirMeatConsumer meatConsumer))
        {
            Debug.LogWarning("[AirGrowthMeatEffect] No IAirMeatConsumer found on collector.");
            return;
        }

        meatConsumer.AddMeat(meatAmount);
        Debug.Log($"[AirGrowthMeatEffect] Added {meatAmount} meat.");
    }

    private bool TryGetMeatConsumer(GameObject collector, out IAirMeatConsumer meatConsumer)
    {
        meatConsumer = null;

        if (collector == null)
        {
            return false;
        }

        meatConsumer = collector.GetComponentInParent<IAirMeatConsumer>();

        if (meatConsumer != null)
        {
            return true;
        }

        meatConsumer = collector.GetComponentInChildren<IAirMeatConsumer>();
        return meatConsumer != null;
    }
}