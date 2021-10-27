using MM.Medical.Client.Core;
using MM.Medical.Client.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MM.Medical.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CacheHelper.LoadLocalSetting();
            if (e.Args.Length > 0)
            {
                if (CacheHelper.LocalSetting.AutoLogin)
                {
                    CacheHelper.LocalSetting.AutoLogin = false;
                    CacheHelper.SaveLocalSetting();
                }
            }
            var login = new LoginView();
            login.ShowDialog();
        }
    }
}
