using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// SplashManager – Oyun açıldığında logo/splash ekranını gösterir,
/// ardından otomatik olarak MainMenuScene sahnesine geçiş yapar.
/// SplashScene sahnesine ekleyin.
/// </summary>
public class SplashManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Fade animasyonu için Canvas Group bileşeni olan GameObject")]
    public CanvasGroup splashCanvasGroup;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenuScene";

    [Header("Timing")]
    [Tooltip("Logo ekranda ne kadar görünür kalır (saniye)")]
    public float displayDuration = 2.5f;

    [Tooltip("Fade-in süresi (saniye)")]
    public float fadeInDuration = 1f;

    [Tooltip("Fade-out süresi (saniye)")]
    public float fadeOutDuration = 1f;

    private void Start()
    {
        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        if (splashCanvasGroup == null)
        {
            Debug.LogWarning("SplashManager: splashCanvasGroup atanmamış! Lütfen Inspector'dan atayın.");
            yield return new WaitForSeconds(displayDuration);
            LoadMainMenu();
            yield break;
        }

        // Başlangıçta tamamen gizli
        splashCanvasGroup.alpha = 0f;

        // Fade In
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));

        // Ekranda bekle
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));

        // Ana menüye geç
        LoadMainMenu();
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            splashCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        splashCanvasGroup.alpha = to;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
