using Microsoft.Win32;
using MM.Medical.Client.Core;
using MM.Medical.Client.Entities;
using Ms.Controls;
using Ms.Libs.Models;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using Newtonsoft.Json.Linq;
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
        private float[] rects;

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
            SetROI();
        }

        private void SetROI()
        {
            if (CacheHelper.LocalSetting.ROI == null)
                rects = new float[4] { 0, 0, 1, 1 };
            else
            {
                rects = new float[4];
                Array.Copy(CacheHelper.LocalSetting.ROI, rects, rects.Length);
            }
            tb_roi.Text = string.Join(",", rects);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSettingExtend setting)
            {
                loading.Start("更新系统设置中,请稍后");
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
                var result = await SocketProxy.Instance.UpdateSystemSetting(setting);
                this.Dispatcher.Invoke(() =>
                {
                    loading.Stop();
                    if (!result.IsSuccess)
                        Alert.ShowMessage(true, AlertType.Error, $"更新系统设置失败,{result.Error}", "软件提示");
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Success, "系统设置更新成功");
                        CacheHelper.LocalSetting.ROI = rects;
                        CacheHelper.SaveLocalSetting();
                    }
                });
            }
        }

        private void UpdateReportIcon_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SystemSettingExtend setting)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = ".jpg|*.jpg|.png|*.png|.jpeg|*.jpeg|.bmp|*.bmp";
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

        private void SetROI_Click(object sender, RoutedEventArgs e)
        {
            var player = new Video();
            player.Loaded += async (_, ex) =>
            {
                player.SetSource(0);
                player.Start();
                await Task.Delay(500);
                player.Dispatcher.Invoke(() =>
                {
                    player.Draw(rects[0], rects[1], rects[2], rects[3]);
                    player.SettingROI = true;
                    player.NotifyDrawComplete += (s, draw) =>
                    {
                        rects[0] = (float)Math.Round(draw.X1, 2);
                        rects[1] = (float)Math.Round(draw.Y1, 2);
                        rects[2] = (float)Math.Round(draw.X2, 2);
                        rects[3] = (float)Math.Round(draw.Y2, 2);
                        tb_roi.Text = string.Join(",", rects);
                    };
                });
            };
            player.ShowDialog("采图截屏区");
            player.Stop();
        }
    }
}
