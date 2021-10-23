using System;
using System.Linq;
using Ms.Controls;
using System.Windows;
using Ms.Libs.SysLib;
using System.Windows.Input;
using Mseiot.Medical.Service.Services;
using Version = Mseiot.Medical.Service.Entities.Version;
using Ms.Controls.Models;
using MM.Medical.Client.Core;
using MM.Medical.Client.Views.Update;

namespace MM.Medical.Client.Views.Login
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            CacheHelper.LoadLocalSetting();
            this.Loaded += LoginView_Loaded;
            Title = CacheHelper.APPTitle;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            InitProperty();
            Updater();
        }

        private async void Updater()
        {
            var result = await SocketProxy.Instance.VerifyVersion(new Version { Code = CacheHelper.ClientVersion });
            if (result.IsSuccess && result.Content != null && !string.IsNullOrEmpty(result.Content.Code))
            {
                var view = new UpdateMain(result.Content);
                view.ShowDialog("新版本提示");
            }
        }

        private void InitProperty()
        {
            var localSetting = CacheHelper.LocalSetting;
            tbName.Text = localSetting.UserRecord.LoginName ?? "";
            tbPwd.Text = localSetting.UserRecord.LoginPwd ?? "";
            cbRemember.IsChecked = localSetting.IsRemember;

            string server = CacheHelper.GetConfig("Server");
            int httpPort;
            int.TryParse(CacheHelper.GetConfig("HTTPPort"), out httpPort);
            int tcpPort;
            int.TryParse(CacheHelper.GetConfig("TCPPort"), out tcpPort);
            SocketProxy.Instance.Load(server, httpPort, tcpPort);
            btLogin.IsEnabled = true;
        }

        #region 登录

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            SetPrompt();
            var loginName = tbName.Text.Trim();
            var loginPwd = tbPwd.Text.Trim();
            if (string.IsNullOrEmpty(loginName))
            {
                SetPrompt("登录名称不能为空");
                return;
            }
            if (string.IsNullOrEmpty(loginPwd))
            {
                SetPrompt("登录密码不能为空");
                return;
            }
            btLogin.IsEnabled = false;
            var result = await SocketProxy.Instance.Login(loginName, loginPwd);
            if (result.IsSuccess)
            {
                var localSetting = CacheHelper.LocalSetting;
                localSetting.IsRemember = cbRemember.IsChecked.Value;
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
            }
            else
            {
                SetPrompt(result.Error);
            }
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

        #region 提示

        public string Prompt
        {
            get { return (string)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prompt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof(string), typeof(LoginView), new PropertyMetadata(""));


        public void SetPrompt(params string[] messages)
        {
            Prompt = "";
            if (messages != null&&messages.Length>0)
            {
                foreach(var item in messages)
                {
                    Prompt += item + ":";
                }
                Prompt = Prompt.Substring(0, Prompt.Length-1);
                borderPrompt.Visibility = Visibility.Visible;
            }
            else
            {
                Prompt = "";
                borderPrompt.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        private void tbName_Selected(object sender, CustomEventArgs e)
        {
            var localSetting = CacheHelper.LocalSetting;
            var userRecord = localSetting.UserRecords.FirstOrDefault(t => t.LoginName.Equals(e.PropertyValue));
            if (TimeHelper.ToUnixTime(DateTime.Now) - userRecord.LoginTime > 3600 * 24 * 7)
                tbPwd.Text = userRecord.LoginPwd = "";
            else
                tbPwd.Text = userRecord.LoginPwd;
            userRecord.CopyTo(localSetting.UserRecord);
        }

        private void tbName_Removed(object sender, CustomEventArgs e)
        {
            var localSetting = CacheHelper.LocalSetting;
            localSetting.UserRecords.RemoveAll(t => t.LoginName.Equals(e.PropertyValue));
            CacheHelper.SaveLocalSetting();
        }

        private void Move_Click(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton== MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
