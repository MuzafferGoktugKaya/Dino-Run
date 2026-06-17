using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirZone.Weather
{
    [DisallowMultipleComponent]
    public class AirWeatherManager : MonoBehaviour
    {
        [Header("Weather Effects")]
        [SerializeField] private List<WeatherEffectBase> weatherEffects = new();

        [Header("Selection Strategy")]
        [SerializeField] private MonoBehaviour selectorBehaviour;

        [Header("Timing")]
        [SerializeField] private float minChangeInterval = 4f;
        [SerializeField] private float maxChangeInterval = 8f;

        public event Action<IWeatherEffect, float> OnWeatherChanged;

        public IWeatherEffect CurrentEffect { get; private set; }
        public Vector3 CurrentWeatherVelocity { get; private set; }
        public float RemainingTime { get; private set; }

        private IWeatherSelector selector;
        private int currentIndex = -1;

        private void Awake()
        {
            selector = selectorBehaviour as IWeatherSelector;

            if (selector == null)
            {
                Debug.LogError($"{nameof(AirWeatherManager)} needs a component that implements IWeatherSelector.");
                enabled = false;
            }
        }

        private void Start()
        {
            ChangeWeather();
        }

        private void Update()
        {
            if (CurrentEffect == null)
                return;

            RemainingTime -= Time.deltaTime;
            CurrentWeatherVelocity = CurrentEffect.GetVelocity(Time.time);

            if (RemainingTime <= 0f)
            {
                ChangeWeather();
            }
        }

        private void ChangeWeather()
        {
            if (weatherEffects == null || weatherEffects.Count == 0)
            {
                Debug.LogWarning($"{nameof(AirWeatherManager)} has no weather effects assigned.");
                return;
            }

            CurrentEffect?.OnExit();

            List<IWeatherEffect> selectableEffects = new();

            foreach (WeatherEffectBase effect in weatherEffects)
            {
                if (effect != null)
                {
                    selectableEffects.Add(effect);
                }
            }

            if (selectableEffects.Count == 0)
            {
                Debug.LogWarning($"{nameof(AirWeatherManager)} has no valid weather effects assigned.");
                return;
            }

            currentIndex = selector.SelectNextIndex(selectableEffects, currentIndex);

            if (currentIndex < 0 || currentIndex >= selectableEffects.Count)
                return;

            CurrentEffect = selectableEffects[currentIndex];
            CurrentEffect.OnEnter();

            RemainingTime = UnityEngine.Random.Range(minChangeInterval, maxChangeInterval);
            CurrentWeatherVelocity = CurrentEffect.GetVelocity(Time.time);

            OnWeatherChanged?.Invoke(CurrentEffect, RemainingTime);

            Debug.Log($"Weather changed: {CurrentEffect.DisplayName} | Velocity: {CurrentWeatherVelocity} | Duration: {RemainingTime:F1}s");
        }
    }
}