using UnityEngine;

namespace AirZone.Weather
{
    public interface IWeatherEffect
    {
        AirWeatherType Type { get; }
        string DisplayName { get; }

        Vector3 GetVelocity(float time);

        void OnEnter();
        void OnExit();
    }
}