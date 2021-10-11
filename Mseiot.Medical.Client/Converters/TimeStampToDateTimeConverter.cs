using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mseiot.Medical.Client.Converters
{
    public class TimeStampToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var param = value.ToString();
                long timeStamp;
                if (param.Length == 13)
                    timeStamp = long.Parse(value.ToString()) / 1000;
                else
                    timeStamp = long.Parse(value.ToString());
                DateTime dateTime = TimeHelper.FromUnixTime(timeStamp);
                return dateTime.ToLocalTime();
            }
            return DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                DateTime input = System.Convert.ToDateTime(value);
                return TimeHelper.ToUnixTime(input);
            }
            else
            {
                return 0;
            }
        }
    }
}
