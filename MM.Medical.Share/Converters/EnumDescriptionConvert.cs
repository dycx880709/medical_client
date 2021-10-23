using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Share.Converters
{
    public class EnumDescriptionConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length == 0)
                    return value.ToString();
                else
                    return (attribArray[0] as DescriptionAttribute).Description;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
