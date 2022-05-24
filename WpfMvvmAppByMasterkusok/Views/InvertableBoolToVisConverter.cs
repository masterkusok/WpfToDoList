using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMvvmAppByMasterkusok.Views
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InvertableBoolToVisConverter : IValueConverter
    {
        enum Parameters
        {
            Normal, Inverted
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            Parameters direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
            if(direction == Parameters.Normal)
            {
                return boolValue ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return boolValue ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
