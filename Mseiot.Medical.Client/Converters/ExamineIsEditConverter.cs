using MM.Medical.Client.Core;
using Ms.Libs.SysLib;
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
    public class ExamineToCanEditConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Examination examination)
                return examination.DoctorName.Equals(CacheHelper.CurrentUser.Name) && TimeHelper.FromUnixTime(examination.ExaminationTime).Date == DateTime.Now.Date;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
