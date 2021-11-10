using Microsoft.Win32;
using MM.Medical.Client.Entities;
using Ms.Controls;
using Ms.Libs.Models;
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
            if (result.IsSuccess) this.DataContext = new SystemSettingExtend(result.Content);
            else MsWindow.ShowDialog($"获取系统设置失败,{ result.Error }", "软件提示");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSettingExtend setting)
            {
                var result = loading.AsyncWait("更新系统设置中,请稍后", Task.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(setting.ReportIconLocal))
                    {
                        var res1 = await SocketProxy.Instance.UploadFile(setting.ReportIconLocal);
                        if (res1.IsSuccess) setting.ReportIcon = res1.Content;
                        else new MsResult<bool>() { IsSuccess = false, Error = res1.Error };
                    }
                    if (!string.IsNullOrEmpty(setting.CutshotSoundLocal))
                    {
                        var res2 = await SocketProxy.Instance.UploadFile(setting.CutshotSoundLocal);
                        if (res2.IsSuccess) setting.CutshotSound = res2.Content;
                        else new MsResult<bool>() { IsSuccess = false, Error = res2.Error };
                    }
                    return await SocketProxy.Instance.UpdateSystemSetting(setting);
                }));
                if (!result.IsSuccess)
                    MsWindow.ShowDialog($"更新系统设置失败,{ result.Error }", "软件提示");
            }
        }

        private void UpdateReportIcon_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSettingExtend setting)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = ".jpg|*.jpg|.png|*.png|.jpeg|*.jpeg";
                if (dialog.ShowDialog().Value)
                    setting.ReportIconLocal = dialog.FileName;
            }
            
        }

        private void UploadCutshotSound_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSettingExtend setting)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = ".mp3|*.mp3|.wav|*.wav";
                if (dialog.ShowDialog().Value)
                    setting.CutshotSoundLocal = dialog.FileName;
            }
        }
    }
}
