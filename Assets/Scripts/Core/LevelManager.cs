using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data List")]
    [Tooltip("0. Eleman her zaman LAND zone olmalidir!")]
    public List<LevelData> levels = new List<LevelData>();

    [Header("Transition Settings")]
    public int minScoreStep = 10;
    public int maxScoreStep = 15;
    public float transitionPauseDuration = 0.45f;
    public float transitionFadeDuration = 0.45f;
    public Color transitionFadeColor = Color.black;

    private int currentScoreStep;
    private int nextLevelThreshold;
    private int currentLevelIndex = 0;
    private int totalLevelsPassed = 0;
    private ObjectSpawner spawner;
    private GroundLooper groundLooper;
    private bool isTransitioning = false;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        spawner = Object.FindFirstObjectByType<ObjectSpawner>();
        groundLooper = Object.FindFirstObjectByType<GroundLooper>();

        currentLevelIndex = 0;
        currentScoreStep = Random.Range(minScoreStep, maxScoreStep + 1);
        nextLevelThreshold = currentScoreStep;

        if (levels.Count > 0)
        {
            UpdateSystems(levels[currentLevelIndex]);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.isGameStarted ||
            GameManager.Instance.isGameOver ||
            GameManager.Instance.isPaused ||
            levels.Count == 0 ||
            isTransitioning)
        {
            return;
        }

        if (GameManager.Instance.score >= nextLevelThreshold)
        {
            totalLevelsPassed++;
            ScheduleNextThreshold();
            SelectNextRandomLevelIndex();
            StartCoroutine(NextLevelTransitionRoutine());
        }
    }

    private void ScheduleNextThreshold()
    {
        int difficultyBonus = Mathf.FloorToInt(totalLevelsPassed * 0.35f);
        currentScoreStep = Random.Range(minScoreStep, maxScoreStep + 1) + difficultyBonus;
        nextLevelThreshold += currentScoreStep;
        Debug.Log("Next zone change at score: " + nextLevelThreshold);
    }

    private void SelectNextRandomLevelIndex()
    {
        if (levels.Count <= 1) return;

        int nextIndex = currentLevelIndex;
        int safety = 0;
        while (nextIndex == currentLevelIndex && safety < 20)
        {
            nextIndex = Random.Range(0, levels.Count);
            safety++;
        }
        currentLevelIndex = nextIndex;
    }

    private IEnumerator NextLevelTransitionRoutine()
    {
        isTransitioning = true;
        Time.timeScale = 0f;

        LevelData nextLevel = currentLevel;
        if (GameManager.Instance != null && nextLevel != null)
        {
            GameManager.Instance.ShowZoneAnnouncement(nextLevel);
        }

        yield return FadeScreenRoutine(0f, 1f);

        UpdateSystems(nextLevel);

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.RegisterZoneTransition();
        }

        if (GameManager.Instance != null && nextLevel != null)
        {
            GameManager.Instance.CheckAndShowZoneIntro(nextLevel);
        }

        yield return new WaitForSecondsRealtime(transitionPauseDuration);

        Time.timeScale = 1f;
        yield return FadeScreenRoutine(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator FadeScreenRoutine(float fromAlpha, float toAlpha)
    {
        if (GameManager.Instance == null || GameManager.Instance.fadePanel == null)
        {
            yield break;
        }

        GameManager.Instance.fadePanel.gameObject.SetActive(true);
        GameManager.Instance.fadePanel.enabled = true;
        GameManager.Instance.fadePanel.raycastTarget = toAlpha > fromAlpha;

        float timer = 0f;
        float duration = Mathf.Max(0.05f, transitionFadeDuration);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, timer / duration);
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, progress);
            GameManager.Instance.fadePanel.color = new Color(transitionFadeColor.r, transitionFadeColor.g, transitionFadeColor.b, alpha);

            if (GameManager.Instance.inGameHUDCanvasGroup != null)
            {
                GameManager.Instance.inGameHUDCanvasGroup.alpha = 1f - alpha;
            }

            yield return null;
        }

        GameManager.Instance.fadePanel.color = new Color(transitionFadeColor.r, transitionFadeColor.g, transitionFadeColor.b, toAlpha);
        if (GameManager.Instance.inGameHUDCanvasGroup != null)
        {
            GameManager.Instance.inGameHUDCanvasGroup.alpha = 1f - toAlpha;
        }
        GameManager.Instance.fadePanel.raycastTarget = toAlpha > 0.01f;
    }

    private void UpdateSystems(LevelData data)
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
