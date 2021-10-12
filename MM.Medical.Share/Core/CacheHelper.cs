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
        public static string ProductName { get; set; }
        public static string ProcessName { get; set; }
        protected static string SettingPath { get { return Path.Combine(CacheHelper.ApplicationPath, "config.data"); } }

        static CacheHelper()
        {
            CacheHelper.InitProperty();
        }

        private static void InitProperty()
        {
            var product = (AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute));
            var compary = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute));
            var version = Assembly.GetEntryAssembly().GetName().Version;
            CacheHelper.ClientVersion = $"{version.Major}.{version.Minor}.{version.Build}.{(version.Revision.ToString().Length > 3 ? version.Revision.ToString() : "0" + version.Revision.ToString())}";
            CacheHelper.ProductName = product.Product;
            CacheHelper.ProcessName = Process.GetCurrentProcess().ProcessName;
            CacheHelper.ApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), compary.Company, $"{ product.Product }_v{version.Major}.{version.Minor}");
            if (!Directory.Exists(CacheHelper.ApplicationPath))
                Directory.CreateDirectory(CacheHelper.ApplicationPath);
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
