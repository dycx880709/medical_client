using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MM.Medical.Client.Converters
{
    public class EndoscopeStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EndoscopeState endoscopeState)
            {
                switch (endoscopeState)
                {
                    case EndoscopeState.Waiting:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B37FEB"));
                    case EndoscopeState.Using:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7875"));
                    case EndoscopeState.Decontaminating:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#85A5FF"));
                    case EndoscopeState.Disabled:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9C6E"));
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
