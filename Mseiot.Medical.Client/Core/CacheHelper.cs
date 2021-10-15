using Mseiot.Medical.Client.Entities;
using Mseiot.Medical.Service.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mseiot.Medical.Client.Core
{
    public class CacheHelper : MM.Medical.Share.Core.CacheHelper
    {
        public static string UserName { get { return CurrentUser.Name; } }
        public static LocalSetting LocalSetting { get; private set; }
        public static User CurrentUser { get; set; } = new User();


        public static void LoadLocalSetting()
        {
            if (File.Exists(SettingPath))
            {
                var json = File.ReadAllText(SettingPath, Encoding.Unicode);
                CacheHelper.LocalSetting = JsonConvert.DeserializeObject<LocalSetting>(json);
            }
            else CacheHelper.LocalSetting = new LocalSetting();
        }

        public static void SaveLocalSetting()
        {
            var json = JsonConvert.SerializeObject(CacheHelper.LocalSetting);
            File.WriteAllText(SettingPath, json, Encoding.Unicode);
        }
    }
}
