using System;
using System.Globalization;
using System.Windows.Data;

namespace CodeSonification
{
    public class LayerToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LayerState val && parameter is string name)
            {
                LayerState thisButton;

                if (name == "All")
                {
                    thisButton = LayerState.All;
                }
                else if (name == "Method")
                {
                    thisButton = LayerState.Method;
                }
                else if (name == "Class")
                {
                    thisButton = LayerState.Class;
                }
                else
                {
                    thisButton = LayerState.Internals;
                }

                if (thisButton == val)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
