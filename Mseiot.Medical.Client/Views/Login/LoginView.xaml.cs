using System;
using System.Linq;
using Ms.Controls;
using System.Windows;
using Ms.Libs.SysLib;
using System.Windows.Input;
using Mseiot.Medical.Client.Core;
using Mseiot.Medical.Service.Services;
using Version = Mseiot.Medical.Service.Entities.Version;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : MsWindow
    {
        public LoginView()
        {
            InitializeComponent();
            this.Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            InitProperty();
            Updater();
        }

        private async void Updater()
        {
            var result = await SocketProxy.Instance.VerifyVersion(new Version { Code = CacheHelper.ClientVersion });
            if (result.IsSuccess && !string.IsNullOrEmpty(result.Content.Code))
            {
                //var view = new UpdateMain(result.Content);
                //view.ShowDialog("新版本提示");
            }
        }

        private void InitProperty()
        {
            var localSetting = CacheHelper.LocalSetting;
            this.Title = tb_title.Text = CacheHelper.ProductName;
            tbName.Text = localSetting.UserRecord.LoginName ?? "";
            tbPwd.Password = localSetting.UserRecord.LoginPwd ?? "";
            if (string.IsNullOrEmpty(localSetting.ServerSetting.Address) 
                || localSetting.ServerSetting.HttpPort == 0
                || localSetting.ServerSetting.TcpPort == 0) 
            {
                tbTips.Text = "请配置服务后登录";
            } else 
            {
                var serverSetting = localSetting.ServerSetting;
                SocketProxy.Instance.Load(serverSetting.Address, serverSetting.HttpPort, serverSetting.TcpPort);
                btLogin.IsEnabled = true;
            }
            tb_version.Text = CacheHelper.ClientVersion;
            layout.MouseMove += (o, e) => 
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        #region 登录

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginName = tbName.Text.Trim();
            var loginPwd = tbPwd.Password.Trim();
            if (string.IsNullOrEmpty(loginName))
            {
                MsWindow.ShowDialog("登录名称不能为空");
                return;
            }
            if (string.IsNullOrEmpty(loginPwd))
            {
                MsWindow.ShowDialog("登录密码不能为空");
                return;
            }
            btLogin.IsEnabled = false;
            var result = await SocketProxy.Instance.Login(loginName, loginPwd);
            if (result.IsSuccess) 
            {
                var localSetting = CacheHelper.LocalSetting;
                var record = localSetting.UserRecord;
                record.LoginName = loginName;
                if (localSetting.IsRemember) record.LoginPwd = loginPwd;
                record.LoginTime = TimeHelper.ToUnixTime(DateTime.Now);
                var condition = localSetting.UserRecords.FirstOrDefault(t => t.LoginName.Equals(loginName));
                if (condition == null) localSetting.UserRecords.Add(record.Copy()); else record.CopyTo(condition);
                CacheHelper.CurrentUser = result.Content;
                CacheHelper.SaveLocalSetting();
                MainWindow mw = new MainWindow();
                Application.Current.MainWindow = mw;
                mw.Show();
                this.Close();
            } else tbTips.Text = result.Error;
            btLogin.IsEnabled = true;
        }

        #endregion

        #region 窗口事件

        //关闭窗体
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void SystemSetting_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
