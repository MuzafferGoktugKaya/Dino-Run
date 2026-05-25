using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AirZone.Weather
{
    [DisallowMultipleComponent]
    public class AirWeatherHud : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AirWeatherManager weatherManager;
        [SerializeField] private Image weatherIconImage;
        [SerializeField] private TMP_Text weatherNameText;
        [SerializeField] private TMP_Text weatherDescriptionText;
        [SerializeField] private TMP_Text weatherTimerText;

        [Header("Weather HUD Entries")]
        [SerializeField] private List<AirWeatherHudEntry> weatherEntries = new();

        private Dictionary<AirWeatherType, AirWeatherHudEntry> entryByType;

        private void Awake()
        {
            BuildLookup();
        }

        private void OnEnable()
        {
            if (weatherManager == null)
            {
                weatherManager = FindFirstObjectByType<AirWeatherManager>();
            }

            if (weatherManager == null)
            {
                Debug.LogWarning("[AirWeatherHud] AirWeatherManager could not be found.");
                return;
            }

            weatherManager.OnWeatherChanged += HandleWeatherChanged;

            if (weatherManager.CurrentEffect != null)
            {
                HandleWeatherChanged(weatherManager.CurrentEffect, weatherManager.RemainingTime);
            }
        }

        private void OnDisable()
        {
            if (weatherManager != null)
            {
                weatherManager.OnWeatherChanged -= HandleWeatherChanged;
            }
        }

        private void Update()
        {
            if (weatherManager == null || weatherTimerText == null)
            {
                return;
            }

            int secondsLeft = Mathf.CeilToInt(weatherManager.RemainingTime);
            weatherTimerText.text = $"{secondsLeft}s";
        }

        private void BuildLookup()
        {
            entryByType = new Dictionary<AirWeatherType, AirWeatherHudEntry>();

            foreach (AirWeatherHudEntry entry in weatherEntries)
            {
                if (entry == null)
                {
                    continue;
                }

                entryByType[entry.WeatherType] = entry;
            }
        }

        private void HandleWeatherChanged(IWeatherEffect weatherEffect, float duration)
        {
            if (weatherEffect == null)
            {
                ApplyFallback("Unknown", "Weather effect active");
                return;
            }

            if (!entryByType.TryGetValue(weatherEffect.Type, out AirWeatherHudEntry entry))
            {
                ApplyFallback(weatherEffect.DisplayName, "Weather effect active");
                Debug.LogWarning($"[AirWeatherHud] No HUD entry found for weather type: {weatherEffect.Type}");
                return;
            }

            if (weatherIconImage != null)
            {
                weatherIconImage.sprite = entry.Icon;
                weatherIconImage.enabled = entry.Icon != null;
            }

            if (weatherNameText != null)
            {
                weatherNameText.text = entry.DisplayName;
            }

            if (weatherDescriptionText != null)
            {
                weatherDescriptionText.text = entry.Description;
            }

            if (weatherTimerText != null)
            {
                weatherTimerText.text = $"{Mathf.CeilToInt(duration)}s";
            }
        }

        private void ApplyFallback(string displayName, string description)
        {
            if (weatherIconImage != null)
            {
                weatherIconImage.enabled = false;
            }

            if (weatherNameText != null)
            {
                weatherNameText.text = displayName;
            }

            if (weatherDescriptionText != null)
            {
                weatherDescriptionText.text = description;
            }
        }
    }

    [Serializable]
    public class AirWeatherHudEntry
    {
        public AirWeatherType WeatherType;
        public string DisplayName;

        [TextArea]
        public string Description;

        public Sprite Icon;
    }
}