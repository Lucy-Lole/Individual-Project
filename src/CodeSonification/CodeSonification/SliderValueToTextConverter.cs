using System;
using System.Globalization;
using System.Windows.Data;

namespace CodeSonification
{
    public class SliderValueToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sliderVal)
            {
                return ((int)(sliderVal)).ToString();
            }
            else
            {
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
