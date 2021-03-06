﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VerejneOsvetlenieData.Data;

namespace VerejneOsvetlenie.Converters
{
    public class FormularVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(parameter?.ToString()))
                return value is SStlp && parameter.ToString() == "stlp" || value is SUlica && parameter.ToString() == "ulica" || value is STechnik && parameter.ToString() == "technik" ? Visibility.Visible : Visibility.Collapsed;
            else
                return value is SStlp || value is SUlica || value is STechnik ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
