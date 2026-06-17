using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MasterVolumeKey = "DinoRun.MasterVolume";
    private const string MusicVolumeKey = "DinoRun.MusicVolume";
    private const string SfxVolumeKey = "DinoRun.SfxVolume";
    private const string MutedKey = "DinoRun.Muted";

    [Header("Ses Kaynaklari")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Muzik Fade Ayarlari")]
    public float bgmFadeInDuration = 1.2f;
    public float bgmFadeOutDuration = 0.55f;
    [Range(0f, 1f)] public float maxBGMVolume = 0.6f;

    [Header("Ses Miks Ayarlari")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 0.9f;
    [Range(0f, 0.25f)] public float sfxPitchVariation = 0.06f;
    public bool isMuted = false;

    [Header("Genel Ses Klipleri")]
    public AudioClip titleBGM;
    public AudioClip buttonClickSFX;
    public AudioClip jumpSFX;
    public AudioClip slideSFX;
    public AudioClip coinSFX;
    public AudioClip powerUpSFX;

    [Header("Yenilgi Sesleri")]
    public AudioClip bonkSFX;
    public AudioClip gameOverJingle;

    [Header("Power Up Ozel Muzigi")]
    public AudioClip powerUpBGM;
    private AudioClip savedZoneBGM;

    [Header("Coin Ozel Sesleri")]
    public AudioClip specialCoinSFX;
    public AudioClip negativeCoinSFX;

    private AudioSource secondaryBgmSource;
    private AudioSource activeBgmSource;
    private Coroutine bgmFadeCoroutine;
    private bool isPowerUpBGMActive = false;

    private float EffectiveMusicVolume => isMuted ? 0f : maxBGMVolume * masterVolume * musicVolume;
    private float EffectiveSfxVolume => isMuted ? 0f : masterVolume * sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioPrefs();
            EnsureAudioSources();
            ApplyMusicVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        GameObject secondaryBgmObject = new GameObject("Secondary BGM Source");
        secondaryBgmObject.transform.SetParent(transform);
        secondaryBgmSource = secondaryBgmObject.AddComponent<AudioSource>();
        secondaryBgmSource.loop = true;
        secondaryBgmSource.playOnAwake = false;
        secondaryBgmSource.spatialBlend = 0f;
        secondaryBgmSource.volume = 0f;

        activeBgmSource = bgmSource;
    }

    private void LoadAudioPrefs()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, masterVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, sfxVolume);
        isMuted = PlayerPrefs.GetInt(MutedKey, isMuted ? 1 : 0) == 1;
    }

    private void SaveAudioPrefs()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.SetInt(MutedKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null || EffectiveSfxVolume <= 0f) return;

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(1f - sfxPitchVariation, 1f + sfxPitchVariation);
        sfxSource.PlayOneShot(clip, EffectiveSfxVolume);
        sfxSource.pitch = originalPitch;
    }

    public void PlayButtonSFX() => PlaySFX(buttonClickSFX);
    public void PlayJumpSFX() => PlaySFX(jumpSFX);
    public void PlayCoinSFX() => PlaySFX(coinSFX);
    public void PlayBonkSFX() => PlaySFX(bonkSFX);
    public void PlayGameOverJingle() => PlaySFX(gameOverJingle);

    public void StartPowerUpAudio()
    {
        PlaySFX(powerUpSFX);

        if (powerUpBGM != null)
        {
            if (!isPowerUpBGMActive)
            {
                savedZoneBGM = activeBgmSource != null ? activeBgmSource.clip : null;
            }

            isPowerUpBGMActive = true;
            ChangeBGM(powerUpBGM);
        }
    }

    public void StopPowerUpAudio()
    {
        if (!isPowerUpBGMActive) return;
        isPowerUpBGMActive = false;

        if (savedZoneBGM != null)
        {
            ChangeBGM(savedZoneBGM);
        }
    }

    public void StopBGM()
    {
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        StopSource(bgmSource);
        StopSource(secondaryBgmSource);
        isPowerUpBGMActive = false;
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        if (bgmSource == null || secondaryBgmSource == null) return;
        if (activeBgmSource != null && activeBgmSource.clip == newBGM && activeBgmSource.isPlaying) return;

        if (isPowerUpBGMActive && newBGM != powerUpBGM)
        {
            savedZoneBGM = newBGM;
            return;
        }

        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(CrossFadeBGMRoutine(newBGM));
    }

    private IEnumerator CrossFadeBGMRoutine(AudioClip newBGM)
    {
        AudioSource fromSource = activeBgmSource == secondaryBgmSource ? secondaryBgmSource : bgmSource;
        AudioSource toSource = fromSource == bgmSource ? secondaryBgmSource : bgmSource;

        if (newBGM == null)
        {
            yield return FadeOutSourceRoutine(fromSource, bgmFadeOutDuration);
            bgmFadeCoroutine = null;
            yield break;
        }

        toSource.clip = newBGM;
        toSource.loop = true;
        toSource.volume = 0f;
        toSource.Play();

        float duration = Mathf.Max(0.05f, bgmFadeInDuration);
        float fromStartVolume = fromSource != null ? fromSource.volume : 0f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, timer / duration);

            if (fromSource != null) fromSource.volume = Mathf.Lerp(fromStartVolume, 0f, progress);
            toSource.volume = Mathf.Lerp(0f, EffectiveMusicVolume, progress);
            yield return null;
        }

        StopSource(fromSource);
        toSource.volume = EffectiveMusicVolume;
        activeBgmSource = toSource;
        bgmFadeCoroutine = null;
    }

    public void StopBGMWithFade(float fadeDuration = 1.0f)
    {
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeOutSourceRoutine(activeBgmSource, fadeDuration));
    }

    private IEnumerator FadeOutSourceRoutine(AudioSource source, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float timer = 0f;
        duration = Mathf.Max(0.05f, duration);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        StopSource(source);
        bgmFadeCoroutine = null;
    }

    private void StopSource(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        source.clip = null;
        source.volume = 0f;
    }

    private void ApplyMusicVolume()
    {
        if (activeBgmSource != null && activeBgmSource.isPlaying)
        {
            activeBgmSource.volume = EffectiveMusicVolume;
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyMusicVolume();
        SaveAudioPrefs();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyMusicVolume();
        SaveAudioPrefs();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        SaveAudioPrefs();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMusicVolume();
        SaveAudioPrefs();
    }
}
