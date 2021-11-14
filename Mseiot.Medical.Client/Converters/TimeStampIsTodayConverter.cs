using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Client.Converters
{
    public class TimeStampIsTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime = null;
            if (value is DateTime dt)
                dateTime = dt;
            else if (value is long ldt)
                dateTime = TimeHelper.FromUnixTime(ldt);
            else if (value is int idt)
                dateTime = TimeHelper.FromUnixTime(idt);
            if (dateTime != null)
            {
                var currentDate = DateTime.Now.ToShortDateString();
                var compareDate = dateTime.Value.ToShortDateString();
                return currentDate == compareDate;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
