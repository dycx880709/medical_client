using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MM.Medical.Share.Core
{
    public class CacheHelper
    {
        public static string ApplicationPath { get; private set; }
        public static string ClientVersion { get; private set; }
        public static string MainVersion { get; private set; }
        public static string ProductName { get; set; }
        public static string ProcessName { get; set; }
        public static string TempPath { get { return Path.Combine(ApplicationPath, "Temp"); } }
        protected static string SettingPath { get { return Path.Combine(CacheHelper.ApplicationPath, "config.data"); } }
        protected static string LogPath { get { return Path.Combine(ApplicationPath, "Log"); } }
        static CacheHelper()
        {
            CacheHelper.InitProperty();
        }

        private static void InitProperty()
        {
            var product = (AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute));
            var compary = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute));
            var version = Assembly.GetEntryAssembly().GetName().Version;
            CacheHelper.ClientVersion = $"{version.Major}.{version.Minor}.{version.Build}";
            CacheHelper.MainVersion = version.Major.ToString();
            //CacheHelper.ClientVersion = $"{version.Major}.{version.Minor}.{version.Build}.{(version.Revision.ToString().Length > 3 ? version.Revision.ToString() : "0" + version.Revision.ToString())}";
            CacheHelper.ProductName = product.Product;
            CacheHelper.ProcessName = Process.GetCurrentProcess().ProcessName;
            CacheHelper.ApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), compary.Company, product.Product);
            if (!Directory.Exists(CacheHelper.ApplicationPath))
                Directory.CreateDirectory(CacheHelper.ApplicationPath);
            if (!Directory.Exists(CacheHelper.TempPath))
                Directory.CreateDirectory(CacheHelper.TempPath);
            if (!Directory.Exists(CacheHelper.LogPath))
                Directory.CreateDirectory(CacheHelper.LogPath);
        }

        public static T GetResource<T>(string resourceName)
        {
            var resources = Application.Current.Resources;
            if (resources.Contains(resourceName))
                return (T)Convert.ChangeType(resources[resourceName], typeof(T));
            return default;
        }
        public static void InitialLogSetting()
        {
            LogHelper.Instance.ConfigLog(CacheHelper.LogPath);
            LogHelper.Instance.OpenAllLevelLog(CacheHelper.LogPath);
            LogHelper.Instance.Info(CacheHelper.LogPath);
            LogHelper.Instance.Error(CacheHelper.LogPath);
        }
    }
}
