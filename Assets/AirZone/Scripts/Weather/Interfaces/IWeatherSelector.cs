using System.Collections.Generic;

namespace AirZone.Weather
{
    public interface IWeatherSelector
    {
        int SelectNextIndex(IReadOnlyList<IWeatherEffect> effects, int currentIndex);
    }
}