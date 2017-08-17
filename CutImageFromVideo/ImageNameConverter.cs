using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CutImageFromVideo {
    class ImageNameConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (!bool.Parse(values[0].ToString())) {
                values[1] = "0";
            }
            var format = new StringBuilder("{0:D").Append(values[1]).Append("}").ToString();
            var zeros = string.Format(format,0);
            return new StringBuilder(zeros).Append(values[2]).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return value.ToString().Split(',');
        }
    }
}
