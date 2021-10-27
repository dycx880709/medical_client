using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Share.Converters
{
    public class SexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            if ((bool)value)
            {
                return "男";
            }
            else
            {
                return "女";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && targetType.IsEnum)
                return Enum.ToObject(targetType, intValue);
            return Enum.ToObject(targetType, 0);
        }
    }
}
