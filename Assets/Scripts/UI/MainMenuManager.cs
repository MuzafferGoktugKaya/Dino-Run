using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MainMenuManager – Ana menüdeki Play ve Quit butonlarını yönetir.
/// MainMenuScene sahnesinde bir GameObject'e ekleyin.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Oyunun ana oynanış sahnesi")]
    public string gameSceneName = "MainSceneBackup";

    private void Start()
    {
        // Müzik başlasın (AudioManager varsa)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
    }

    /// <summary>
    /// Play butonuna bağlanacak metod.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Quit butonuna bağlanacak metod.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
