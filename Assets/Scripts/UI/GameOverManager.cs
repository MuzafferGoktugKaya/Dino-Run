using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// GameOverManager – Oyun sonu panelini yönetir.
/// GameManager.OnGameOver eventine abone olarak paneli açar.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Panel")]
    [Tooltip("Oyun sonu paneli (başlangıçta kapalı olmalı)")]
    public GameObject gameOverPanel;

    [Header("Score Texts")]
    public TMP_Text finalScoreText;
    public TMP_Text finalHighScoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver(int score, int highScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score.ToString("D5");

        if (finalHighScoreText != null)
            finalHighScoreText.text = "Best: " + highScore.ToString("D5");
    }

    /// <summary>Restart butonuna bağlanacak metod.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Main Menu butonuna bağlanacak metod.</summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
