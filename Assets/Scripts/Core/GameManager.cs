using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool shouldSkipTitle = false;

    private const string HighScoreKey = "DinoRun.HighScore";
    private static int sessionHighScore = 0;

    [Header("UI Panelleri")]
    public GameObject titleScreenPanel;
    public GameObject inGameHUDPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    private Coroutine zoneIntroRoutine;

    [Header("Zone Bilgilendirme Paneli")]
    public GameObject zoneIntroPanel;
    public TMP_Text zoneIntroText;

    [Header("HUD Fade Ayari")]
    public CanvasGroup inGameHUDCanvasGroup;

    [Header("Game Over Sinematik Ayarlari")]
    public CanvasGroup gameOverTitleGroup;
    public CanvasGroup gameOverContentGroup;

    [Header("Fade Ayarlari")]
    public Image fadePanel;
    public float sceneReloadFadeDuration = 0.35f;

    [Header("UI Metinleri")]
    public TMP_Text scoreText;
    public TMP_Text gameOverCurrentScoreText;
    public TMP_Text gameOverHighScoreText;
    public TMP_Text healthText;
    public TMP_Text comboText;
    public TMP_Text missionText;

    [Header("Combo Ayarlari")]
    public float comboTimeout = 2.2f;
    public int comboBonusEvery = 5;
    private int currentCombo = 0;
    private float lastComboTime = -99f;

    [Header("Bildirim (Notification) Ayarlari")]
    public TMP_Text notificationText;
    private Coroutine notificationRoutine;

    [Header("Oyun Durumu")]
    public bool isGameStarted = false;
    public bool isGameOver = false;
    public bool isPaused = false;
    public int score = 0;

    private HashSet<string> visitedZones = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
        sessionHighScore = PlayerPrefs.GetInt(HighScoreKey, sessionHighScore);
    }

    private void Start()
    {
        isGameOver = false;
        isPaused = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeBGM(AudioManager.Instance.titleBGM);
        }

        if (fadePanel != null)
        {
            fadePanel.color = new Color(0f, 0f, 0f, 0f);
            fadePanel.raycastTarget = false;
        }

        if (notificationText != null)
        {
            Color c = notificationText.color;
            c.a = 0f;
            notificationText.color = c;
        }

        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;
        if (zoneIntroPanel != null) zoneIntroPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        EnsureMissionManager();
        UpdateGameOverHighScoreUI();
        UpdateComboUI();

        if (shouldSkipTitle)
        {
            shouldSkipTitle = false;
            StartGame();
        }
        else
        {
            Time.timeScale = 0f;
            isGameStarted = false;

            if (titleScreenPanel != null) titleScreenPanel.SetActive(true);
            if (inGameHUDPanel != null) inGameHUDPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isGameStarted || isGameOver) return;

        if (currentCombo > 0 && Time.unscaledTime - lastComboTime > comboTimeout)
        {
            ResetCombo();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void EnsureMissionManager()
    {
        MissionManager manager = MissionManager.Instance;
        if (manager == null)
        {
            manager = gameObject.AddComponent<MissionManager>();
        }

        if (manager.missionText == null)
        {
            manager.missionText = missionText;
        }
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSFX();
        }

        isGameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;

        if (titleScreenPanel != null) titleScreenPanel.SetActive(false);
        if (inGameHUDPanel != null) inGameHUDPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;

        score = 0;
        ResetCombo();
        UpdateScoreUI();

        if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
        {
            CheckAndShowZoneIntro(LevelManager.Instance.currentLevel);
            LevelManager.Instance.PlayCurrentZoneBGM();
        }
    }

    public void ShowNotification(string message, Color textColor)
    {
        if (notificationText == null) return;

        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
        }

        notificationText.text = message;
        notificationText.color = textColor;
        notificationRoutine = StartCoroutine(NotificationFadeSequenceRoutine());
    }

    private IEnumerator NotificationFadeSequenceRoutine()
    {
        float duration = 0.18f;
        float elapsed = 0f;
        Color c = notificationText.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            notificationText.color = c;
            yield return null;
        }

        c.a = 1f;
        notificationText.color = c;

        yield return new WaitForSecondsRealtime(0.9f);

        duration = 0.35f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            notificationText.color = c;
            yield return null;
        }

        c.a = 0f;
        notificationText.color = c;
    }

    public void ShowZoneAnnouncement(LevelData zoneData)
    {
        if (zoneData == null) return;

        string zoneName = string.IsNullOrEmpty(zoneData.displayName) ? zoneData.name : zoneData.displayName;
        string message = string.IsNullOrEmpty(zoneData.transitionMessage) ? "Entering " + zoneName : zoneData.transitionMessage;
        ShowNotification(message, zoneData.themeColor);
    }

    public void CheckAndShowZoneIntro(LevelData zoneData)
    {
        if (zoneData == null) return;
        string uniqueZoneKey = zoneData.name;

        if (!visitedZones.Contains(uniqueZoneKey))
        {
            visitedZones.Add(uniqueZoneKey);

            if (!string.IsNullOrEmpty(zoneData.firstTimeDescription))
            {
                if (zoneIntroRoutine != null) StopCoroutine(zoneIntroRoutine);
                zoneIntroRoutine = StartCoroutine(ShowZoneIntroRoutine(zoneData));
            }
        }
    }

    private IEnumerator ShowZoneIntroRoutine(LevelData zoneData)
    {
        if (zoneIntroPanel == null || zoneIntroText == null) yield break;

        string zoneName = string.IsNullOrEmpty(zoneData.displayName) ? zoneData.name : zoneData.displayName;
        zoneIntroText.text = zoneName + "\n" + zoneData.firstTimeDescription;
        zoneIntroPanel.SetActive(true);

        CanvasGroup panelGroup = zoneIntroPanel.GetComponent<CanvasGroup>();
        if (panelGroup != null)
        {
            float t = 0f;
            while (t < 0.35f)
            {
                t += Time.unscaledDeltaTime;
                panelGroup.alpha = Mathf.SmoothStep(0f, 1f, t / 0.35f);
                yield return null;
            }
            panelGroup.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(2.4f);

        if (panelGroup != null)
        {
            float t = 0f;
            while (t < 0.35f)
            {
                t += Time.unscaledDeltaTime;
                panelGroup.alpha = Mathf.SmoothStep(1f, 0f, t / 0.35f);
                yield return null;
            }
            panelGroup.alpha = 0f;
        }

        zoneIntroPanel.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (isGameOver || !isGameStarted) return;
        score += amount;
        UpdateScoreUI();
    }

    public void RegisterCoinPickup(int scoreAmount)
    {
        if (isGameOver || !isGameStarted) return;

        AddScore(scoreAmount);
        currentCombo++;
        lastComboTime = Time.unscaledTime;

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.RegisterCoinCollected();
            MissionManager.Instance.RegisterCombo(currentCombo);
        }

        if (comboBonusEvery > 0 && currentCombo % comboBonusEvery == 0)
        {
            int bonus = currentCombo / comboBonusEvery;
            AddScore(bonus);
            ShowNotification("COMBO x" + currentCombo + " +" + bonus, Color.cyan);
        }

        UpdateComboUI();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        UpdateComboUI();
    }

    public void RemoveScore(int amount)
    {
        if (isGameOver || !isGameStarted) return;
        score -= amount;
        if (score < 0) score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth, bool shieldActive)
    {
        if (healthText != null)
        {
            healthText.text = shieldActive
                ? "Health: " + currentHealth + "/" + maxHealth + "  SHIELD"
                : "Health: " + currentHealth + "/" + maxHealth;
        }
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = currentCombo > 1 ? "Combo x" + currentCombo : string.Empty;
        }
    }

    private void UpdateGameOverHighScoreUI()
    {
        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "High Score: " + sessionHighScore;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGMWithFade(0.35f);
            AudioManager.Instance.PlayBonkSFX();
        }

        ResetCombo();

        bool newBest = score > sessionHighScore;
        if (newBest)
        {
            sessionHighScore = score;
            PlayerPrefs.SetInt(HighScoreKey, sessionHighScore);
            PlayerPrefs.Save();
        }

        StartCoroutine(GameOverSequenceRoutine(newBest));
    }

    private IEnumerator GameOverSequenceRoutine(bool newBest)
    {
        if (gameOverTitleGroup != null) gameOverTitleGroup.alpha = 0f;
        if (gameOverContentGroup != null) gameOverContentGroup.alpha = 0f;

        if (inGameHUDPanel != null) inGameHUDPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (gameOverCurrentScoreText != null)
        {
            gameOverCurrentScoreText.text = newBest ? "Score: " + score + "  NEW BEST!" : "Score: " + score;
        }
        UpdateGameOverHighScoreUI();

        yield return new WaitForSecondsRealtime(0.15f);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverJingle();

        float fadeDuration = 0.65f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (gameOverTitleGroup != null)
            {
                gameOverTitleGroup.alpha = Mathf.SmoothStep(0f, 1f, timer / fadeDuration);
            }
            yield return null;
        }
        if (gameOverTitleGroup != null) gameOverTitleGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(0.25f);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (gameOverContentGroup != null)
            {
                gameOverContentGroup.alpha = Mathf.SmoothStep(0f, 1f, timer / fadeDuration);
            }
            yield return null;
        }
        if (gameOverContentGroup != null) gameOverContentGroup.alpha = 1f;
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (!isGameStarted || isGameOver || isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        ShowNotification("Paused", Color.white);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        shouldSkipTitle = true;
        StartCoroutine(ReloadSceneRoutine());
    }

    public void GoToTitle()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        shouldSkipTitle = false;
        StartCoroutine(ReloadSceneRoutine());
    }

    private IEnumerator ReloadSceneRoutine()
    {
        Time.timeScale = 1f;

        if (fadePanel != null)
        {
            fadePanel.raycastTarget = true;
            float timer = 0f;
            while (timer < sceneReloadFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / sceneReloadFadeDuration);
                fadePanel.color = new Color(0f, 0f, 0f, progress);
                yield return null;
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        Debug.Log("Oyundan cikiliyor...");
        Application.Quit();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(clip);
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.ChangeBGM(newBGM);
    }
}
