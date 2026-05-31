using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data List")]
    [Tooltip("0. Eleman her zaman LAND zone olmalıdır!")]
    public List<LevelData> levels = new List<LevelData>();
    
[Header("Transition Settings")]
public int minScoreStep = 10;
public int maxScoreStep = 15;

private int currentScoreStep;
private int nextLevelThreshold;

public float transitionPauseDuration = 0.75f;

    private int currentLevelIndex = 0;
    private int totalLevelsPassed = 0; 
    private ObjectSpawner spawner;
    private GroundLooper groundLooper;
    private bool isTransitioning = false;

    // ÇÖZÜM: GameManager'ın hata vermeden "currentLevel" verisini okuyabilmesi için kısa bir Property ekledik
    public LevelData currentLevel
    {
        get
        {
            if (levels != null && currentLevelIndex < levels.Count)
            {
                return levels[currentLevelIndex];
            }
            return null;
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        spawner = Object.FindFirstObjectByType<ObjectSpawner>();
        groundLooper = Object.FindFirstObjectByType<GroundLooper>();
        
        // İlk seviye her zaman listenin ilk elemanıdır (Land Zone)
currentLevelIndex = 0;

currentScoreStep = Random.Range(minScoreStep, maxScoreStep + 1);
nextLevelThreshold = currentScoreStep;

if (levels.Count > 0)
    UpdateSystems(levels[currentLevelIndex]);
    }

void Update()
{
    if (GameManager.Instance == null ||
        !GameManager.Instance.isGameStarted ||
        GameManager.Instance.isGameOver ||
        levels.Count == 0 ||
        isTransitioning)
    {
        return;
    }

    if (GameManager.Instance.score >= nextLevelThreshold)
    {
        totalLevelsPassed++;

        // Sonraki geçiş için yeni random skor aralığı
        currentScoreStep = Random.Range(minScoreStep, maxScoreStep + 1);
        nextLevelThreshold += currentScoreStep;

        Debug.Log($"Next zone change at score: {nextLevelThreshold}");

        SelectNextRandomLevelIndex();
        StartCoroutine(NextLevelTransitionRoutine());
    }
}

    void SelectNextRandomLevelIndex()
    {
        if (levels.Count <= 1) return;

        int nextIndex = currentLevelIndex;
        while (nextIndex == currentLevelIndex)
        {
            nextIndex = Random.Range(0, levels.Count);
        }
        currentLevelIndex = nextIndex;
    }

    // --- ENTEGRE SKOR FADE DESTEKLİ GEÇİŞ ROUTINE'I ---
    IEnumerator NextLevelTransitionRoutine()
    {
        isTransitioning = true;

        // 1. Oyunu durdur
        Time.timeScale = 0f;

        if (GameManager.Instance != null && GameManager.Instance.fadePanel != null)
        {
            GameManager.Instance.fadePanel.gameObject.SetActive(true);
            GameManager.Instance.fadePanel.enabled = true;
            GameManager.Instance.fadePanel.raycastTarget = true; 

            float fadeDuration = 0.4f;
            float timer = 0f;

            // EKRAN KARARIRKEN SKORU (HUD) DA YUMUŞAKÇA GİZLİYORUZ
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime; 
                float progress = Mathf.Clamp01(timer / fadeDuration);
                
                // Ekranı siyaha boya
                GameManager.Instance.fadePanel.color = new Color(0f, 0f, 0f, progress);
                
                // HUD panelinin alpha değerini 1'den 0'a düşür
                if (GameManager.Instance.inGameHUDCanvasGroup != null)
                {
                    GameManager.Instance.inGameHUDCanvasGroup.alpha = 1f - progress;
                }
                
                yield return null;
            }

            // Tam kararma anında değerleri sabitleyelim
            GameManager.Instance.fadePanel.color = new Color(0f, 0f, 0f, 1f);
            if (GameManager.Instance.inGameHUDCanvasGroup != null)
            {
                GameManager.Instance.inGameHUDCanvasGroup.alpha = 0f;
            }
        }

        // 2. TAM KARANLIK AN (Arka planda harita ve müzik oyuncu görmeden değişir)
        UpdateSystems(levels[currentLevelIndex]);

        // ENTEGRASYON: Yeni bölge arka planda tam olarak yüklendiği salisede, 
        // ekran hala siyahken ilk defa giriş yazısı kontrolünü tetikliyoruz.
        if (GameManager.Instance != null && currentLevel != null)
        {
            GameManager.Instance.CheckAndShowZoneIntro(currentLevel);
        }

        // Belirlediğin duraklama süresi kadar karanlık ekranda bekle
        yield return new WaitForSecondsRealtime(transitionPauseDuration);

        // Oyunu tekrar akıtmaya başlıyoruz ki ekran açılırken dinozor koşmaya başlasın
        Time.timeScale = 1f;

        // 3. EKRAN AÇILIRKEN SKORU (HUD) PÜRÜZSÜZCE GERİ GETİRİYORUZ
        if (GameManager.Instance != null && GameManager.Instance.fadePanel != null)
        {
            float fadeDuration = 0.4f;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / fadeDuration);
                
                // Ekranı aç
                GameManager.Instance.fadePanel.color = new Color(0f, 0f, 0f, 1f - progress);
                
                // HUD panelinin alpha değerini 0'dan 1'e yükselt
                if (GameManager.Instance.inGameHUDCanvasGroup != null)
                {
                    GameManager.Instance.inGameHUDCanvasGroup.alpha = progress;
                }
                
                yield return null;
            }
            
            // Geçiş tamamen bitti, değerleri normal oyun moduna sabitle
            GameManager.Instance.fadePanel.color = new Color(0f, 0f, 0f, 0f);
            if (GameManager.Instance.inGameHUDCanvasGroup != null)
            {
                GameManager.Instance.inGameHUDCanvasGroup.alpha = 1f;
            }
            
            GameManager.Instance.fadePanel.raycastTarget = false; 
        }

        isTransitioning = false;
    }

    void UpdateSystems(LevelData data)
    {
        if (data == null) return;

        PlayerMovement playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null) playerMovement.ResetTemporaryBoosts();

        if (data.skyboxMaterial != null)
        {
            RenderSettings.skybox = data.skyboxMaterial;
            DynamicGI.UpdateEnvironment(); 
        }

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

        if (GameManager.Instance != null && GameManager.Instance.isGameStarted)
        {
            PlayCurrentZoneBGM();
        }
    }

    public void PlayCurrentZoneBGM()
    {
        if (levels.Count > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.ChangeBGM(levels[currentLevelIndex].zoneBGM);
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