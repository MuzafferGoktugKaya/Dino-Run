using UnityEngine;

namespace AirZone.Weather
{
    public abstract class WeatherEffectBase : MonoBehaviour, IWeatherEffect
    {
        [Header("Display")]
        [SerializeField] private string displayName = "Weather Effect";

        public string DisplayName => displayName;

        public abstract AirWeatherType Type { get; }

        public abstract Vector3 GetVelocity(float time);

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}