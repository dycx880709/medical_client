using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Client.Converters
{
    public class SizeLengthFormatConverter : IValueConverter
    {

        private readonly static string[] arrUnit = new string[6] { "B", "K", "M", "G", "T", "P" };
        private const int baseStep = 1024;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileSize = System.Convert.ToDouble(value);
            var unitIndex = 0;
            while (fileSize >= baseStep && unitIndex < arrUnit.Length - 1)
            {
                unitIndex++;
                fileSize /= baseStep;
            }
            fileSize = Math.Round(fileSize, 2, MidpointRounding.AwayFromZero);
            return fileSize + " " + arrUnit[unitIndex];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
