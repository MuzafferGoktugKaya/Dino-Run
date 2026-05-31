using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool shouldSkipTitle = false; 

    [Header("UI Panelleri")]
    public GameObject titleScreenPanel;
    public GameObject inGameHUDPanel;
    public GameObject gameOverPanel;

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

    [Header("Ses Kaynakları")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Müzik Fade Ayarları")]
    public float bgmFadeInDuration = 1.2f; 
    [Range(0f, 1f)] public float maxBGMVolume = 0.6f;

    [Header("Genel Ses Klipleri")]
    public AudioClip buttonClickSFX;
    public AudioClip jumpSFX;
    public AudioClip coinSFX;
    public AudioClip powerUpSFX;
    
    [Header("Yenilgi Sesleri")]
    public AudioClip bonkSFX;         
    public AudioClip gameOverJingle;  

    [Header("Oyun Durumu")]
    public bool isGameStarted = false;
    public bool isGameOver = false;
    public int score = 0;

    private Coroutine bgmFadeCoroutine; 

    private void Awake()
    {
        Instance = this;
        PlayerPrefs.DeleteKey("HighScore");
    }

    private void Start()
    {
        isGameOver = false;

        if (fadePanel != null)
        {
            fadePanel.color = new Color(0f, 0f, 0f, 0f);
            fadePanel.raycastTarget = false;
        }

        // Başlangıçta HUD opaklığını tamamen açık yapalım
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;

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
        PlaySFX(buttonClickSFX);
        
        isGameStarted = true;
        Time.timeScale = 1f;

        if (titleScreenPanel != null) titleScreenPanel.SetActive(false);
        if (inGameHUDPanel != null) inGameHUDPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;
        
        score = 0;
        UpdateScoreUI();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.PlayCurrentZoneBGM();
        }
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

        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        if (bgmSource != null) bgmSource.Stop(); 

        PlaySFX(bonkSFX);
        StartCoroutine(GameOverSequenceRoutine());
    }

    private IEnumerator GameOverSequenceRoutine()
    {
        // Başlangıçta panelleri tamamen şeffaf yapıyoruz
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

        // Bonk sesinin duyulması için minik bir es ve jingle başlangıcı
        yield return new WaitForSecondsRealtime(0.15f);
        PlaySFX(gameOverJingle);

        // KARE TEMİZLEME: İlk karedeki lag/kasma bilgisini çöpe atmak için 1 kare bekliyoruz
        yield return null;

        // 1. AŞAMA: GAME OVER Yazısının Belirmesi (Fade In)
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

        // Yazı tamamen açıldıktan sonra sinematik bekleme
        yield return new WaitForSecondsRealtime(0.5f);

        // 2. AŞAMA: Butonların ve Skorların Belirmesi (Fade In)
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
        PlaySFX(buttonClickSFX);
        shouldSkipTitle = true; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitle()
    {
        PlaySFX(buttonClickSFX);
        shouldSkipTitle = false; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        PlaySFX(buttonClickSFX);
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

    // --- ENTEGRE HUD FADE DESTEKLİ ZONE GEÇİŞİ ---
    private IEnumerator FadeRoutine(Action zoneSwitchLogic)
    {
        fadePanel.raycastTarget = true;
        float duration = 0.4f;
        float timer = 0f;

        // Ekran Siyaha Boyanırken HUD (Skor) Yavaşça Kayboluyor
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; 
            float progress = Mathf.Clamp01(timer / duration);
            
            fadePanel.color = new Color(0f, 0f, 0f, progress);
            
            if (inGameHUDCanvasGroup != null) 
                inGameHUDCanvasGroup.alpha = 1f - progress; // Ters orantı (Görünmez oluyor)

            yield return null;
        }
        
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 0f;

        // Arka planda bölgeyi değiştiriyoruz
        if (zoneSwitchLogic != null)
        {
            zoneSwitchLogic.Invoke(); 
        }

        yield return new WaitForSecondsRealtime(0.2f); 

        timer = 0f;
        // Ekran Açılırken HUD (Skor) Pürüzsüzce Geri Geliyor
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            
            fadePanel.color = new Color(0f, 0f, 0f, 1f - progress);
            
            if (inGameHUDCanvasGroup != null) 
                inGameHUDCanvasGroup.alpha = progress; // Doğru orantı (Görünür oluyor)

            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        if (inGameHUDCanvasGroup != null) inGameHUDCanvasGroup.alpha = 1f;

        fadePanel.raycastTarget = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        if (bgmSource == null) return;
        if (bgmSource.clip == newBGM && bgmSource.isPlaying) return;

        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeInBGMRoutine(newBGM));
    }

    private IEnumerator FadeInBGMRoutine(AudioClip newBGM)
    {
        bgmSource.volume = 0f;
        bgmSource.Stop();

        if (newBGM != null)
        {
            bgmSource.clip = newBGM;
            bgmSource.loop = true;
            bgmSource.Play();

            float timer = 0f;
            while (timer < bgmFadeInDuration)
            {
                timer += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(0f, maxBGMVolume, timer / bgmFadeInDuration);
                yield return null;
            }
            bgmSource.volume = maxBGMVolume;
        }
    }
}