namespace NatechWeather.Helpers
{
    internal static class Extensions
    {
        public static Color GetThemeColor(this Application application, string key)
        {
            Color selectedColor;
            try
            {
                selectedColor = application.Resources[key] as Color;
            }
            catch
            {
                selectedColor = application.Resources.MergedDictionaries.FirstOrDefault()[key] as Color;
            }

            return selectedColor;
        }

    }
}
