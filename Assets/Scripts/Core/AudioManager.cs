using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
    public AudioClip powerUpSFX; // Karacoin ilk alındığında çalacak efekt
    
    [Header("Yenilgi Sesleri")]
    public AudioClip bonkSFX;         
    public AudioClip gameOverJingle;  

    [Header("Power Up Özel Müziği")]
    public AudioClip powerUpBGM;   // Süre boyunca çalacak coşkulu müzik
    private AudioClip savedZoneBGM; // Süre bittiğinde geri dönmek için eski müziği saklar

    private Coroutine bgmFadeCoroutine;
    private bool isPowerUpBGMActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonSFX() => PlaySFX(buttonClickSFX);
    public void PlayJumpSFX() => PlaySFX(jumpSFX);
    public void PlayCoinSFX() => PlaySFX(coinSFX);
    public void PlayBonkSFX() => PlaySFX(bonkSFX);
    public void PlayGameOverJingle() => PlaySFX(gameOverJingle);

    // --- KARACOIN SES MEKANİKLERİ ---
    public void StartPowerUpAudio()
    {
        PlaySFX(powerUpSFX); // İlk alınış efektini çal

        if (bgmSource != null && powerUpBGM != null)
        {
            // Eğer halihazırda powerup müziği çalmıyorsa mevcut zone müziğini yedekle
            if (!isPowerUpBGMActive)
            {
                savedZoneBGM = bgmSource.clip;
            }

            isPowerUpBGMActive = true;
            ChangeBGM(powerUpBGM);
        }
    }

    public void StopPowerUpAudio()
    {
        if (!isPowerUpBGMActive) return;
        isPowerUpBGMActive = false;

        // Süre bittiğinde yedeklediğimiz orijinal bölge müziğine pürüzsüzce geri dön
        if (bgmSource != null && savedZoneBGM != null)
        {
            ChangeBGM(savedZoneBGM);
        }
    }

    public void StopBGM()
    {
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        if (bgmSource != null) bgmSource.Stop();
        isPowerUpBGMActive = false;
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        if (bgmSource == null) return;
        if (bgmSource.clip == newBGM && bgmSource.isPlaying) return;

        // Eğer geçiş yapılan müzik powerup müziği değilse ve powerup modu aktifse, 
        // araya girip düzeni bozmamak için sadece yedek klibi güncelliyoruz
        if (isPowerUpBGMActive && newBGM != powerUpBGM)
        {
            savedZoneBGM = newBGM;
            return;
        }

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