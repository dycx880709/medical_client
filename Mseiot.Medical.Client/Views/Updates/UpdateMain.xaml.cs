using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Version = Mseiot.Medical.Service.Entities.Version;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// UpdateMain.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateMain : UserControl
    {
        #region 类成员

        private Version version;

        #endregion

        #region 构造器

        public UpdateMain(Version version)
        {
            InitializeComponent();
            this.version = version;
            tbContent.Text = version.Content;
            tbTime.Text = TimeHelper.FromUnixTime(version.Time).ToString("yyyy/MM/dd HH:mm:ss");
            tbVersion.Text = version.Code;
        }

        #endregion

        #region 事件

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            #region 拷贝升级程序
            var runPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"update");
            if (!Directory.Exists(runPath)) Directory.CreateDirectory(runPath);
            var runExe = Path.Combine(runPath, "update.exe");
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "update.exe", runExe, true);
            #endregion
            var localFilePath = runPath + "/update" + Path.GetFileName(version.Path);
            var result = loading.AsyncWait("下载升级包中,请稍后", SocketProxy.Instance.HttpProxy.DownloadFile("files/" + version.Path, localFilePath));
            if (result.IsSuccess)
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                #region 运行升级程序，并关掉主进程 
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = $"{processName} \"{localFilePath}\"",
                    FileName = runExe
                };
                Process.Start(psi);
                Application.Current.Shutdown();
                #endregion
            }
            else MsWindow.ShowDialog($"升级包下载失败,{ result.Error }");
        }

        #endregion
    }
}
