using SkiaSharp;
using System.ComponentModel;
using System.Globalization;

namespace NatechCharts.Converters
{
    public class StringToSKColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string strColor)
            {
                if (SKColor.TryParse(strColor, out var skColor))
                {
                    return skColor;
                }
            }
            return SKColors.Black;
        }
    }
}
