using System;
using System.Globalization;
using System.Windows.Data;

namespace PlantsVsZombiesStudio.I18n
{
    [ValueConversion(typeof(string), typeof(string))]
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return LanguageManager.CurrentLanguage.Query(s);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
