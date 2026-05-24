using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data List")]
    public List<LevelData> levels = new List<LevelData>();
    
    [Header("Transition Settings")]
    public int scoreStep = 5; 
    public Animator fadeAnimator; 

    private int currentLevelIndex = 0;
    private int totalLevelsPassed = 0; 
    private ObjectSpawner spawner;
    private GroundLooper groundLooper;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        spawner = Object.FindFirstObjectByType<ObjectSpawner>();
        groundLooper = Object.FindFirstObjectByType<GroundLooper>();
        if (levels.Count > 0) UpdateSystems(levels[0]);
    }

    void Update()
    {
        if (GameManager.Instance == null || levels.Count == 0) return;

        int nextLevelThreshold = (totalLevelsPassed + 1) * scoreStep;
        
        if (GameManager.Instance.score >= nextLevelThreshold)
        {
            totalLevelsPassed++;
            currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
            StartNextLevelTransition();
        }
    }

    void StartNextLevelTransition()
    {
        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeTrigger");
        UpdateSystems(levels[currentLevelIndex]);
    }

    void UpdateSystems(LevelData data)
    {
        if (data == null) return;

        // >>> YENİ: RUNTIME SKYBOX DEĞİŞTİRME MEKANİZMASI <<<
        if (data.skyboxMaterial != null)
        {
            // Unity'nin global skybox'ını kodla değiştiriyoruz
            RenderSettings.skybox = data.skyboxMaterial;
            
            // Işıklandırma ve yansımaların yeni gökyüzüne göre anında tazelenmesini sağlar
            DynamicGI.UpdateEnvironment(); 
        }
        // >>> ---------------------------------------- <<<

        if (spawner != null)
        {
            spawner.currentLevel = data;
            spawner.ClearExistingObstacles(); 
        }
        
        if (groundLooper != null)
        {
            groundLooper.currentLevel = data;
            groundLooper.ApplyLevelMaterial(); 
        }
    }

    public LevelData GetCurrentLevelData()
    {
        if (levels != null && currentLevelIndex < levels.Count)
        {
            return levels[currentLevelIndex];
        }
        return null;
    }
}