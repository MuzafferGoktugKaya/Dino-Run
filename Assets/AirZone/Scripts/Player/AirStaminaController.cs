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
    private float lastSpendTime = -999f;

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