using MM.Medical.Decontaminate.Entities;
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

namespace MM.Medical.Decontaminate.Core
{
    public class CacheHelper
    {
        public static string UserName { get { return CurrentUser.Name; } }
        public static string ApplicationPath { get; private set; }
        public static string ClientVersion { get; private set; }
        public static string ProductName { get; set; }
        public static string ProcessName { get; set; }
        public static LocalSetting LocalSetting { get; private set; }
        public static User CurrentUser { get; set; } = new User();

        private static string SettingPath { get { return Path.Combine(CacheHelper.ApplicationPath, "config.data"); } }

        static CacheHelper()
        {
            CacheHelper.InitProperty();
        }

        private static void InitProperty()
        {
            var product = (AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute));
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            CacheHelper.ClientVersion = $"{version.Major}.{version.Minor}.{version.Build}.{(version.Revision.ToString().Length > 3 ? version.Revision.ToString() : "0" + version.Revision.ToString())}";
            CacheHelper.ApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mseiot", $"{ product.Product }_v{version.Major}.{version.Minor}");
            CacheHelper.ProductName = product.Product;
            CacheHelper.ProcessName = Process.GetCurrentProcess().ProcessName;
            if (!Directory.Exists(CacheHelper.ApplicationPath))
                Directory.CreateDirectory(CacheHelper.ApplicationPath);
        }

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

        public static T GetResource<T>(string resourceName)
        {
            var resources = Application.Current.Resources;
            if (resources.Contains(resourceName))
                return (T)Convert.ChangeType(resources[resourceName], typeof(T));
            return default;
        }
    }
}
