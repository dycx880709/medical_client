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
    public class PatientStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PatientStatus patientStatus)
            {
                switch (patientStatus)
                {
                    case PatientStatus.UnRegist:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC069"));
                    case PatientStatus.Regist:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7875"));
                    case PatientStatus.Checking:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#85A5FF"));
                    case PatientStatus.Checked:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95DE64"));
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
