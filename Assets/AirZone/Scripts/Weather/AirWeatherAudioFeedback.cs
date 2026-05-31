using UnityEngine;

namespace AirZone.Weather
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AirWeatherAudioFeedback : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AirWeatherManager weatherManager;

        [Header("Wind Audio")]
        [SerializeField] private AudioClip windClip;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float activeWindVolume = 0.8f;

        [Range(0f, 1f)]
        [SerializeField] private float calmWindVolume = 0.08f;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = calmWindVolume;

            if (windClip != null)
            {
                audioSource.clip = windClip;
            }

            if (weatherManager == null)
            {
                weatherManager = GetComponent<AirWeatherManager>();
            }

            if (weatherManager == null)
            {
                weatherManager = FindFirstObjectByType<AirWeatherManager>();
            }
        }

        private void OnEnable()
        {
            if (weatherManager != null)
            {
                weatherManager.OnWeatherChanged += HandleWeatherChanged;

                if (weatherManager.CurrentEffect != null)
                {
                    HandleWeatherChanged(weatherManager.CurrentEffect, weatherManager.RemainingTime);
                }
            }
        }

        private void Start()
        {
            StartWindLoopIfReady();
        }

        private void OnDisable()
        {
            if (weatherManager != null)
            {
                weatherManager.OnWeatherChanged -= HandleWeatherChanged;
            }
        }

        private void HandleWeatherChanged(IWeatherEffect weatherEffect, float duration)
        {
            if (weatherEffect == null)
            {
                return;
            }

            StartWindLoopIfReady();

            float targetVolume = GetVolumeForWeather(weatherEffect.Type);
            audioSource.volume = targetVolume;
        }

        private void StartWindLoopIfReady()
        {
            if (audioSource == null || windClip == null)
            {
                return;
            }

            if (audioSource.clip != windClip)
            {
                audioSource.clip = windClip;
            }

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private float GetVolumeForWeather(AirWeatherType weatherType)
        {
            switch (weatherType)
            {
                case AirWeatherType.ForwardWind:
                case AirWeatherType.BackwardWind:
                    return activeWindVolume;

                case AirWeatherType.Calm:
                    return calmWindVolume;

                default:
                    return calmWindVolume;
            }
        }
    }
}