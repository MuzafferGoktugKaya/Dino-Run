using UnityEngine;

[DisallowMultipleComponent]
public class AirRestoreStaminaMeatEffect : AirMeatEffectBase
{
    [Header("Stamina Reward")]
    [SerializeField] private float restoreAmount = 25f;

    public override void Apply(GameObject collector)
    {
        if (restoreAmount <= 0f)
        {
            return;
        }

        if (!TryGetCollectorComponent(collector, out AirStaminaController staminaController))
        {
            Debug.LogWarning("[AirRestoreStaminaMeatEffect] No AirStaminaController found on collector.");
            return;
        }

        staminaController.Restore(restoreAmount);
        Debug.Log($"[AirRestoreStaminaMeatEffect] Restored {restoreAmount} stamina.");
    }
}