using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Client.Converters
{
    public class EndoscopeStateIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EndoscopeState state)
                return state != EndoscopeState.Disabled;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool result)
                return result ? EndoscopeState.Waiting : EndoscopeState.Disabled;
            return EndoscopeState.Disabled;
        }
    }
}
