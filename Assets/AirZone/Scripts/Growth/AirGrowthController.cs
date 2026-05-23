using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AirGrowthController : MonoBehaviour, IAirMeatConsumer
{
    [Header("Growth State")]
    [SerializeField] private int currentMeatCount = 0;
    [SerializeField] private int totalMeatCollected = 0;
    [SerializeField] private int currentGrowthStage = 0;

    [Header("Growth Rules")]
    [SerializeField] private int meatNeededPerStage = 3;
    [SerializeField] private int maxGrowthStage = 3;

    [Header("Visual Growth")]
    [SerializeField] private Transform scaleTarget;
    [SerializeField] private float scaleIncreasePerStage = 0.15f;

    private Vector3 initialScale;

    public event Action<int, int> OnMeatChanged;
    public event Action<int> OnGrowthStageChanged;

    public int CurrentMeatCount => currentMeatCount;
    public int TotalMeatCollected => totalMeatCollected;
    public int CurrentGrowthStage => currentGrowthStage;
    public int MeatNeededPerStage => meatNeededPerStage;
    public int MaxGrowthStage => maxGrowthStage;

    private void Awake()
    {
        if (scaleTarget == null)
            scaleTarget = transform;

        initialScale = scaleTarget.localScale;
    }

    private void Start()
    {
        ApplyGrowthScale();
        NotifyState();
    }

    public void AddMeat(int amount)
    {
        if (amount <= 0)
            return;

        currentMeatCount += amount;
        totalMeatCollected += amount;

        TryIncreaseGrowthStage();

        Debug.Log($"[AirGrowth] Collected Total: {totalMeatCollected} | Progress: {GetProgressText()} | Stage: {currentGrowthStage}");

        OnMeatChanged?.Invoke(currentMeatCount, GetMeatNeededForNextStage());
    }

    private void TryIncreaseGrowthStage()
    {
        while (currentMeatCount >= meatNeededPerStage && currentGrowthStage < maxGrowthStage)
        {
            currentMeatCount -= meatNeededPerStage;
            currentGrowthStage++;

            ApplyGrowthScale();

            Debug.Log($"[AirGrowth] Stage increased: {currentGrowthStage}");

            OnGrowthStageChanged?.Invoke(currentGrowthStage);
        }
    }

    private void ApplyGrowthScale()
    {
        if (scaleTarget == null)
            return;

        float scaleMultiplier = 1f + currentGrowthStage * scaleIncreasePerStage;
        scaleTarget.localScale = initialScale * scaleMultiplier;
    }

    public int GetMeatNeededForNextStage()
    {
        if (currentGrowthStage >= maxGrowthStage)
            return 0;

        return meatNeededPerStage;
    }

    private string GetProgressText()
    {
        if (currentGrowthStage >= maxGrowthStage)
            return "MAX";

        return $"{currentMeatCount} / {meatNeededPerStage}";
    }

    private void NotifyState()
    {
        OnMeatChanged?.Invoke(currentMeatCount, GetMeatNeededForNextStage());
        OnGrowthStageChanged?.Invoke(currentGrowthStage);
    }
}