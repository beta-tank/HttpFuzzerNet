using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HttpFuzzer.Gui.Helpers
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value != Visibility.Visible;
            }
            else
            {
                return true;
            }
        }
    }
}
