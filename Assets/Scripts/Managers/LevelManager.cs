using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data List")]
    public List<LevelData> levels = new List<LevelData>();
    
    [Header("Transition Settings")]
    public int scoreStep = 5; // Her 5 skorda bir geçiş
    public Animator fadeAnimator; 

    private int currentLevelIndex = 0;
    private int totalLevelsPassed = 0; // Kaç kez seviye atladığımızı tutar
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

        // Bir sonraki eşik: (geçilen toplam seviye + 1) * scoreStep
        int nextLevelThreshold = (totalLevelsPassed + 1) * scoreStep;
        
        if (GameManager.Instance.score >= nextLevelThreshold)
        {
            totalLevelsPassed++;
            
            // MODULO kullanarak index'i döndür: 0, 1, 0, 1...
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