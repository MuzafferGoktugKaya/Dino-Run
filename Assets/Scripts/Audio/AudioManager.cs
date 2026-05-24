using UnityEngine;

/// <summary>
/// AudioManager – Singleton ses yöneticisi. DontDestroyOnLoad ile sahneler arası hayatta kalır.
/// GameManager.OnGameOver eventine abone olarak çarpışma sesi çalar ve müziği durdurur.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip jumpClip;
    public AudioClip coinClip;
    public AudioClip collisionClip;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetupAudioSources();
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= HandleGameOver;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // ─── BGM ─────────────────────────────────────────────────────────────────

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PauseMusic()  { if (musicSource != null) musicSource.Pause(); }
    public void ResumeMusic() { if (musicSource != null) musicSource.UnPause(); }

    // ─── SFX ─────────────────────────────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayJump()      => PlaySFX(jumpClip);
    public void PlayCoin()      => PlaySFX(coinClip);
    public void PlayCollision() => PlaySFX(collisionClip);

    // ─── Event Handler ───────────────────────────────────────────────────────

    private void HandleGameOver(int score, int highScore)
    {
        PlayCollision();
        StopMusic();
    }

    // ─── Volume Control ──────────────────────────────────────────────────────

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }
}
