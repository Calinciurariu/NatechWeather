using SkiaSharp;

namespace NatechCharts.Extensions
{
    public static class ColorExtensions
    {
        public static SKColor ToSKColor(this Color mauiColor)
        {
            return new SKColor(
                (byte)(mauiColor.Red * 255),
                (byte)(mauiColor.Green * 255),
                (byte)(mauiColor.Blue * 255),
                (byte)(mauiColor.Alpha * 255));
        }
    }
}
