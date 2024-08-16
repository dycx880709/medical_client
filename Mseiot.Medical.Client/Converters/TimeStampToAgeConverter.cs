using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Client.Converters
{
    public class TimestampToAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            long timestamp = (int)value;
            DateTime birthDate = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            int age = CalculateAge(birthDate);
            return age;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int age)
            {
                DateTime birthDate = DateTime.Now.AddYears(-age);
                long timestamp = ((DateTimeOffset)birthDate).ToUnixTimeSeconds();
                return timestamp;
            }
            return 0;
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
