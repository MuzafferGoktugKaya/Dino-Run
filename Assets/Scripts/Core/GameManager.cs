using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool shouldSkipTitle = false; 

    [Header("UI Panelleri")]
    public GameObject titleScreenPanel;
    public GameObject inGameHUDPanel;
    public GameObject gameOverPanel;
    private Coroutine zoneIntroRoutine;

    [Header("Zone Bilgilendirme Paneli")]
    public GameObject zoneIntroPanel; 
    public TMP_Text zoneIntroText;    

    [Header("HUD Fade Ayarı")]
    [Tooltip("Zone geçişlerinde HUD'ın (Skorun) yumuşakça kaybolup gelmesi için Canvas Group")]
    public CanvasGroup inGameHUDCanvasGroup;

    [Header("Game Over Sinematik Ayarları")]
    public CanvasGroup gameOverTitleGroup; 
    public CanvasGroup gameOverContentGroup; 

    [Header("Fade Ayarları")]
    public Image fadePanel; 

    [Header("UI Metinleri")]
    public TMP_Text scoreText;
    public TMP_Text gameOverCurrentScoreText;
    public TMP_Text gameOverHighScoreText;

    [Header("Bildirim (Notification) Ayarları")]
    public TMP_Text notificationText;
    private Coroutine notificationRoutine;

    [Header("Oyun Durumu")]
    public bool isGameStarted = false;
    public bool isGameOver = false;
    public int score = 0;

    private HashSet<string> visitedZones = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
        PlayerPrefs.DeleteKey("HighScore");
    }

    private void Start()
    {
        isGameOver = false;

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

    public void StartGame()
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayButtonSFX();
            AudioManager.Instance.StopBGM(); 
        }

        isGameStarted = true;
        Time.timeScale = 1f;

        if (titleScreenPanel != null) titleScreenPanel.SetActive(false);
        if (inGameHUDPanel != null) inGameHUDPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;
        
        score = 0;
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
        float duration = 0.25f; // Fade In süresi
        float elapsed = 0f;
        Color c = notificationText.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Oyun durduğunda bile düzgün çalışması için unscaled kullandık
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            notificationText.color = c;
            yield return null;
        }
        c.a = 1f;
        notificationText.color = c;

        yield return new WaitForSecondsRealtime(0.8f);

        duration = 0.4f;
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
                zoneIntroRoutine = StartCoroutine(ShowZoneIntroRoutine(zoneData.firstTimeDescription));
            }
        }
    }

    private IEnumerator ShowZoneIntroRoutine(string message)
    {
        if (zoneIntroPanel == null || zoneIntroText == null) yield break;

        zoneIntroText.text = message;
        zoneIntroPanel.SetActive(true);

        CanvasGroup panelGroup = zoneIntroPanel.GetComponent<CanvasGroup>();
        if (panelGroup != null)
        {
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                panelGroup.alpha = Mathf.Clamp01(t / 0.5f);
                yield return null;
            }
            panelGroup.alpha = 1f;
        }

        yield return new WaitForSeconds(3f);

        if (panelGroup != null)
        {
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                panelGroup.alpha = Mathf.Clamp01(1f - (t / 0.5f));
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

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f; 

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayBonkSFX();
        }

        StartCoroutine(GameOverSequenceRoutine());
    }

    private IEnumerator GameOverSequenceRoutine()
    {
        if (gameOverTitleGroup != null) gameOverTitleGroup.alpha = 0f;
        if (gameOverContentGroup != null) gameOverContentGroup.alpha = 0f;

        if (inGameHUDPanel != null) inGameHUDPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (gameOverCurrentScoreText != null) gameOverCurrentScoreText.text = "Score: " + score;
        if (gameOverHighScoreText != null) gameOverHighScoreText.text = "High Score: " + highScore;

        yield return new WaitForSecondsRealtime(0.15f);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverJingle();

        yield return null;

        float fadeDuration = 0.8f; 
        float timer = 0f;
        
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            if (gameOverTitleGroup != null)
            {
                gameOverTitleGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            }
            yield return null;
        }
        if (gameOverTitleGroup != null) gameOverTitleGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(0.5f);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (gameOverContentGroup != null)
            {
                gameOverContentGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            }
            yield return null;
        }
        if (gameOverContentGroup != null) gameOverContentGroup.alpha = 1f;
    }

    public void RestartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        shouldSkipTitle = true; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitle()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        shouldSkipTitle = false; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSFX();
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit();
    }

    public void TriggerZoneTransition(Action zoneSwitchLogic)
    {
        if (fadePanel != null)
        {
            StartCoroutine(FadeRoutine(zoneSwitchLogic));
        }
        else
        {
            zoneSwitchLogic?.Invoke();
        }
    }

    private IEnumerator FadeRoutine(Action zoneSwitchLogic)
    {
        fadePanel.raycastTarget = true;
        float duration = 0.4f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; 
            float progress = Mathf.Clamp01(timer / duration);
            
            fadePanel.color = new Color(0f, 0f, 0f, progress);
            
            if (inGameHUDCanvasGroup != null) 
                inGameHUDCanvasGroup.alpha = 1f - progress;

            yield return null;
        }
        
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 0f;

        if (zoneSwitchLogic != null)
        {
            zoneSwitchLogic.Invoke(); 
        }

        yield return new WaitForSecondsRealtime(0.2f); 

        if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
        {
            CheckAndShowZoneIntro(LevelManager.Instance.currentLevel);
        }

        timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            
            fadePanel.color = new Color(0f, 0f, 0f, 1f - progress);
            
            if (inGameHUDCanvasGroup != null) 
                inGameHUDCanvasGroup.alpha = progress;

            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;

        fadePanel.raycastTarget = false;
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