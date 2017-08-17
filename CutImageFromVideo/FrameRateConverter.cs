using System;
using System.Globalization;
using System.Windows.Data;

namespace CutImageFromVideo {
    class FrameRateConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Math.Pow(10, int.Parse(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}
