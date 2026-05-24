using System.Collections.Generic;
using UnityEngine;

namespace AirZone.Weather
{
    [DisallowMultipleComponent]
    public class RandomWeatherSelector : MonoBehaviour, IWeatherSelector
    {
        public int SelectNextIndex(IReadOnlyList<IWeatherEffect> effects, int currentIndex)
        {
            if (effects == null || effects.Count == 0)
                return -1;

            if (effects.Count == 1)
                return 0;

            int nextIndex;

            do
            {
                nextIndex = Random.Range(0, effects.Count);
            }
            while (nextIndex == currentIndex);

            return nextIndex;
        }
    }
}