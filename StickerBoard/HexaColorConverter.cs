using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StickerBoard
{
    public class HexaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try 
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString()));
                }
                catch (Exception) { }
            }

            return new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(SolidColorBrush))
            {
                Color c = ((SolidColorBrush)value).Color;
                return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
            }

            return "#000000";
        }
    }
}
