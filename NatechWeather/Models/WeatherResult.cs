namespace NatechWeather.Models
{
    public class WeatherResult
    {
        public Main Main { get; set; }
        public Weather[] Weather { get; set; }
        public string Name { get; set; }
    }
}
