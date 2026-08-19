using System;

namespace P04.Core.State
{
    /// <summary>
    /// 环境状态：天气/温度。
    /// </summary>
    public sealed class EnvironmentState
    {
        public Weather Weather { get; private set; } = Weather.Clear;
        public float AmbientTemperature { get; private set; } = 20f; // 环境温度

        /// <summary>设置天气（由 WeatherSystem 或 AI Director 调用）。</summary>
        public void SetWeather(Weather weather)
        {
            Weather = weather;
            // 天气影响环境温度
            AmbientTemperature = weather switch
            {
                Weather.Clear => 20f,
                Weather.Rain => 10f,
                Weather.Storm => 5f,
                Weather.Snow => -5f,
                _ => 20f
            };
        }
    }

    public enum Weather
    {
        Clear,
        Rain,
        Storm,
        Snow
    }
}
