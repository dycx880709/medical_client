﻿using MM.Medical.Client.Core;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MM.Medical.Client.Converters
{
    public class PathToWebUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var serverSetting = CacheHelper.LocalSetting.ServerSetting;
                var url = $"http://{ serverSetting.Address }:{ serverSetting.HttpPort }";
                var filePath = url + "/files/" + value.ToString();
                return filePath;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
