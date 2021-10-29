using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Share.Converters
{
    public class TimeStampToStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime = null;
            if (value is int timeStamp)
                dateTime = TimeHelper.FromUnixTime(timeStamp);
            else if (value is long timeStampL)
                dateTime = TimeHelper.FromUnixTime(timeStampL);
            else if (value is DateTime dt)
                dateTime = dt;
            else dateTime = new DateTime();
            dateTime = dateTime.Value.ToLocalTime();
            string stringFormat = "yyyy/MM/dd HH:mm:ss";
            if (parameter is string format)
                stringFormat = format;
            return dateTime.Value.ToString(stringFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
