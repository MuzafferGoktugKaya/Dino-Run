using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AirGrowthController : MonoBehaviour, IAirMeatConsumer
{
    [Header("Growth State")]
    [SerializeField] private int currentMeatCount = 0;
    [SerializeField] private int currentGrowthStage = 0;

    [Header("Growth Rules")]
    [SerializeField] private int meatNeededPerStage = 3;
    [SerializeField] private int maxGrowthStage = 3;

    public event Action<int, int> OnMeatChanged;
    public event Action<int> OnGrowthStageChanged;

    public int CurrentMeatCount => currentMeatCount;
    public int CurrentGrowthStage => currentGrowthStage;
    public int MeatNeededPerStage => meatNeededPerStage;
    public int MaxGrowthStage => maxGrowthStage;

    private void Start()
    {
        NotifyState();
    }

    public void AddMeat(int amount)
    {
        if (amount <= 0)
            return;

        currentMeatCount += amount;

        Debug.Log($"[AirGrowth] Meat collected: {currentMeatCount}");

        TryIncreaseGrowthStage();

        OnMeatChanged?.Invoke(currentMeatCount, GetMeatNeededForNextStage());
    }

    private void TryIncreaseGrowthStage()
    {
        while (currentMeatCount >= meatNeededPerStage && currentGrowthStage < maxGrowthStage)
        {
            currentMeatCount -= meatNeededPerStage;
            currentGrowthStage++;

            Debug.Log($"[AirGrowth] Stage increased: {currentGrowthStage}");

            OnGrowthStageChanged?.Invoke(currentGrowthStage);
        }
    }

    public int GetMeatNeededForNextStage()
    {
        if (currentGrowthStage >= maxGrowthStage)
            return 0;

        return meatNeededPerStage;
    }

    private void NotifyState()
    {
        OnMeatChanged?.Invoke(currentMeatCount, GetMeatNeededForNextStage());
        OnGrowthStageChanged?.Invoke(currentGrowthStage);
    }
}