using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("UI Referansları")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        // GameManager'daki evente abone ol
        GameManager.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        // Aboneliği iptal et (Hataları önlemek için şart)
        GameManager.OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        RefreshHighScore();
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString("D5");
    }

    public void RefreshHighScore()
    {
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "Best: " + hs.ToString("D5");
    }
}