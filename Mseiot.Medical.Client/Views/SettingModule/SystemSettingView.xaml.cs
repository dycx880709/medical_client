using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// SystemSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettingView : UserControl
    {
        public SystemSettingView()
        {
            InitializeComponent();
            this.Loaded += SystemSettingView_Loaded;
        }

        private void SystemSettingView_Loaded(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("获取系统设置中,请稍后", SocketProxy.Instance.GetSystemSetting());
            if (result.IsSuccess) this.DataContext = result.Content;
            else MsWindow.ShowDialog($"获取系统设置失败,{ result.Error }", "软件提示");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSetting setting)
            {
                var result = loading.AsyncWait("更新系统设置中,请稍后", SocketProxy.Instance.UpdateSystemSetting(setting));
                if (result.IsSuccess) this.DataContext = result.Content;
                else MsWindow.ShowDialog($"更新系统设置失败,{ result.Error }", "软件提示");
            }
        }

        private void UpdateIcon_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UploadCutshotSound_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
