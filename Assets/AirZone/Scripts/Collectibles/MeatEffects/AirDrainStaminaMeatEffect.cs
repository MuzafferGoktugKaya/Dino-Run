using UnityEngine;

[DisallowMultipleComponent]
public class AirDrainStaminaMeatEffect : AirMeatEffectBase
{
    [Header("Stamina Debuff")]
    [SerializeField] private float drainAmount = 30f;

    public override void Apply(GameObject collector)
    {
        if (drainAmount <= 0f)
        {
            return;
        }

        if (!TryGetCollectorComponent(collector, out AirStaminaController staminaController))
        {
            Debug.LogWarning("[AirDrainStaminaMeatEffect] No AirStaminaController found on collector.");
            return;
        }

        staminaController.Drain(drainAmount);
        Debug.Log($"[AirDrainStaminaMeatEffect] Drained {drainAmount} stamina.");
    }
}