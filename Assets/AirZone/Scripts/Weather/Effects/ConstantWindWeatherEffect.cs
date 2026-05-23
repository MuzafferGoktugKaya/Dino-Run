using UnityEngine;

namespace AirZone.Weather
{
    [DisallowMultipleComponent]
    public class ConstantWindWeatherEffect : WeatherEffectBase
    {
        [Header("Weather Type")]
        [SerializeField] private AirWeatherType weatherType = AirWeatherType.ForwardWind;

        [Header("Wind Settings")]
        [SerializeField] private Vector3 direction = Vector3.forward;
        [SerializeField] private float strength = 2.5f;

        public override AirWeatherType Type => weatherType;

        public override Vector3 GetVelocity(float time)
        {
            return direction.normalized * strength;
        }
    }
}