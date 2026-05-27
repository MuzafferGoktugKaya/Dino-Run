using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AirStaminaController : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private bool startFull = true;

    [Header("Usage")]
    [SerializeField] private float climbStaminaCost = 20f;

    [Header("Regeneration")]
    [SerializeField] private float regenPerSecond = 18f;
    [SerializeField] private float regenDelayAfterUse = 0.6f;

    private float currentStamina;
    private float lastSpendTime = float.NegativeInfinity;

    public event Action<float, float> OnStaminaChanged;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float NormalizedStamina => maxStamina <= 0f ? 0f : currentStamina / maxStamina;

    private void Awake()
    {
        maxStamina = Mathf.Max(1f, maxStamina);
        currentStamina = startFull ? maxStamina : Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void Start()
    {
        NotifyStaminaChanged();
    }

    private void Update()
    {
        RegenerateStamina();
    }

    public bool TrySpendForClimb()
    {
        return TrySpend(climbStaminaCost);
    }

    public bool TrySpend(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        if (currentStamina < amount)
        {
            Debug.Log("[AirStamina] Not enough stamina to climb.");
            return false;
        }

        currentStamina -= amount;
        lastSpendTime = Time.time;

        NotifyStaminaChanged();
        return true;
    }

    public void Restore(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        NotifyStaminaChanged();

        Debug.Log($"[AirStamina] Restored {amount} stamina. Current: {currentStamina:F0}/{maxStamina:F0}");
    }

    public void Drain(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentStamina = Mathf.Max(0f, currentStamina - amount);
        NotifyStaminaChanged();

        Debug.Log($"[AirStamina] Drained {amount} stamina. Current: {currentStamina:F0}/{maxStamina:F0}");
    }

    private void RegenerateStamina()
    {
        if (Time.time < lastSpendTime + regenDelayAfterUse)
        {
            return;
        }

        if (currentStamina >= maxStamina)
        {
            return;
        }

        currentStamina = Mathf.Min(
            maxStamina,
            currentStamina + regenPerSecond * Time.deltaTime
        );

        NotifyStaminaChanged();
    }

    private void NotifyStaminaChanged()
    {
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
}