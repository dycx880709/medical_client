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
    public class AppointmentStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AppointmentStatus appointmentStatus)
            {
                switch (appointmentStatus)
                {
                    case AppointmentStatus.Reserved:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC069"));
                    case AppointmentStatus.PunchIn:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7875"));
                    case AppointmentStatus.Waiting:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B37FEB"));
                    case AppointmentStatus.Checking:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#85A5FF"));
                    case AppointmentStatus.Checked:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#69C0FF"));
                    case AppointmentStatus.Reported:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95DE64"));
                    case AppointmentStatus.Cancel:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9C6E"));
                    case AppointmentStatus.Exprire:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399"));
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
