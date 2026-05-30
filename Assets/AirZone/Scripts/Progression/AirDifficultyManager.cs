using UnityEngine;

[DisallowMultipleComponent]
public class AirDifficultyManager : MonoBehaviour
{
    [System.Serializable]
    private class PhaseSettings
    {
        [Header("Phase")]
        public string phaseName = "Young";

        [Header("Obstacle Difficulty")]
        [Min(0.1f)] public float obstacleSpawnInterval = 1.5f;
        [Min(0f)] public float obstacleSpeedMultiplier = 1f;

        [Header("Meat Spawn")]
        [Min(0.1f)] public float meatSpawnInterval = 2.5f;

        [Header("Meat Weights")]
        [Min(0f)] public float alphaWeight = 20f;
        [Min(0f)] public float betaWeight = 65f;
        [Min(0f)] public float gammaWeight = 15f;
    }

    [Header("References")]
    [SerializeField] private AirGrowthController growthController;
    [SerializeField] private AirObstacleSpawner obstacleSpawner;
    [SerializeField] private AirMeatSpawner meatSpawner;

    [Header("Growth Stage Thresholds")]
    [SerializeField] private int adultStartsAtGrowthStage = 1;
    [SerializeField] private int apexStartsAtGrowthStage = 2;

    [Header("Young Phase")]
    [SerializeField] private PhaseSettings youngPhase = new PhaseSettings
    {
        phaseName = "Young",
        obstacleSpawnInterval = 1.5f,
        obstacleSpeedMultiplier = 1f,
        meatSpawnInterval = 2.5f,
        alphaWeight = 20f,
        betaWeight = 65f,
        gammaWeight = 15f
    };

    [Header("Adult Phase")]
    [SerializeField] private PhaseSettings adultPhase = new PhaseSettings
    {
        phaseName = "Adult",
        obstacleSpawnInterval = 1.25f,
        obstacleSpeedMultiplier = 1.15f,
        meatSpawnInterval = 2.4f,
        alphaWeight = 20f,
        betaWeight = 60f,
        gammaWeight = 20f
    };

    [Header("Apex Phase")]
    [SerializeField] private PhaseSettings apexPhase = new PhaseSettings
    {
        phaseName = "Apex",
        obstacleSpawnInterval = 1.05f,
        obstacleSpeedMultiplier = 1.35f,
        meatSpawnInterval = 2.3f,
        alphaWeight = 15f,
        betaWeight = 55f,
        gammaWeight = 30f
    };

    private string currentPhaseName;

    private void Awake()
    {
        FindMissingReferences();
    }

    private void OnEnable()
    {
        FindMissingReferences();

        if (growthController != null)
        {
            growthController.OnGrowthStageChanged += HandleGrowthStageChanged;
        }
    }

    private void OnDisable()
    {
        if (growthController != null)
        {
            growthController.OnGrowthStageChanged -= HandleGrowthStageChanged;
        }
    }

    private void Start()
    {
        if (growthController != null)
        {
            ApplyDifficultyForGrowthStage(growthController.CurrentGrowthStage);
        }
        else
        {
            ApplyDifficultyForGrowthStage(0);
        }
    }

    private void FindMissingReferences()
    {
        if (growthController == null)
        {
            growthController = FindFirstObjectByType<AirGrowthController>();
        }

        if (obstacleSpawner == null)
        {
            obstacleSpawner = FindFirstObjectByType<AirObstacleSpawner>();
        }

        if (meatSpawner == null)
        {
            meatSpawner = FindFirstObjectByType<AirMeatSpawner>();
        }
    }

    private void HandleGrowthStageChanged(int growthStage)
    {
        ApplyDifficultyForGrowthStage(growthStage);
    }

    private void ApplyDifficultyForGrowthStage(int growthStage)
    {
        PhaseSettings settings = GetSettingsForGrowthStage(growthStage);

        if (settings == null)
        {
            return;
        }

        if (currentPhaseName == settings.phaseName)
        {
            return;
        }

        ApplyPhaseSettings(settings);
    }

    private PhaseSettings GetSettingsForGrowthStage(int growthStage)
    {
        if (growthStage >= apexStartsAtGrowthStage)
        {
            return apexPhase;
        }

        if (growthStage >= adultStartsAtGrowthStage)
        {
            return adultPhase;
        }

        return youngPhase;
    }

    private void ApplyPhaseSettings(PhaseSettings settings)
    {
        currentPhaseName = settings.phaseName;

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnInterval(settings.obstacleSpawnInterval);
            obstacleSpawner.SetObstacleSpeedMultiplier(settings.obstacleSpeedMultiplier);
        }

        if (meatSpawner != null)
        {
            meatSpawner.SetSpawnInterval(settings.meatSpawnInterval);
            meatSpawner.SetMeatWeights(
                settings.alphaWeight,
                settings.betaWeight,
                settings.gammaWeight
            );
        }

        Debug.Log($"[AirDifficulty] Phase changed to {settings.phaseName}");
    }
}