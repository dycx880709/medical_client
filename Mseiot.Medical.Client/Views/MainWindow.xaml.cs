using Ms.Controls;
using MM.Medical.Client.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ms.Controls.Core;
using System.Windows.Controls.Primitives;
using Mseiot.Medical.Service.Services;
using Ms.Libs.TcpLib;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MsWindow
    {
        private readonly Dictionary<string, UserControl> navigateItems 
            = new Dictionary<string, UserControl>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTime();
            await ConnectServer();
        }

        private void TcpProxy_ReceiveMessaged(object sender, Message e)
        {

        }

        private async void TcpProxy_ConnectStateChanged(object sender, ConnectStateArgs e)
        {
            switch (e.ConnectState)
            {
                case ConnectState.Faild:
                    this.Dispatcher.Invoke(() =>
                    {
                        foreach (var win in Application.Current.Windows)
                        {
                            if (win is MainWindow) continue;
                            (win as Window).WindowState = WindowState.Minimized;
                        }
                        loading.Start("启动连接已断开,断线重连中");
                    });
                    SocketProxy.Instance.TcpProxy.ReceiveMessaged -= TcpProxy_ReceiveMessaged;
                    await System.Threading.Tasks.Task.Delay(1000);
                    await ConnectServer();
                    break;
                case ConnectState.Success:
                    await System.Threading.Tasks.Task.Delay(3 * 100);
                    var result = await SocketProxy.Instance.Login(CacheHelper.CurrentUser.UserID);
                    if (result.IsSuccess)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            foreach (var win in Application.Current.Windows)
                            {
                                if (win is MainWindow) continue;
                                (win as Window).WindowState = WindowState.Normal;
                            }
                            loading.Stop();
                        });
                        SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
                    }
                    else
                    {
                        await System.Threading.Tasks.Task.Delay(3 * 1000);
                        this.Dispatcher.Invoke(() => loading.Start("服务连接失败,重试连接中"));
                        await ConnectServer();
                    }
                    break;
            }
        }

        private async System.Threading.Tasks.Task ConnectServer()
        {
            if (SocketProxy.Instance.TcpProxy != null)
            {
                SocketProxy.Instance.TcpProxy.ConnectStateChanged -= TcpProxy_ConnectStateChanged;
                SocketProxy.Instance.TcpProxy.ReceiveMessaged -= TcpProxy_ReceiveMessaged;
                await SocketProxy.Instance.TcpProxy.Stop();
            }
            SocketProxy.Instance.TcpProxy.ConnectStateChanged += TcpProxy_ConnectStateChanged;
            await SocketProxy.Instance.TcpProxy.Start();
        }

        private void UpdateTime()
        {
            tb_time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, ex) => tb_time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();
        }

        private void Restart(string message = "")
        {
            var processPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheHelper.ProcessName + ".exe");
            Process.Start(processPath, message);
            Application.Current.Shutdown();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Restart("Logout");
        }

        private void ModifyPwd_Click(object sender, RoutedEventArgs e)
        {
            var view = new ResetPwdView(this.loading);
            child.ShowDialog("修改密码", view);
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            if (SocketProxy.Instance.TcpProxy != null)
            {
                SocketProxy.Instance.TcpProxy.ConnectStateChanged -= TcpProxy_ConnectStateChanged;
                SocketProxy.Instance.TcpProxy.ReceiveMessaged -= TcpProxy_ReceiveMessaged;
                await SocketProxy.Instance.TcpProxy.Stop();
            }
            App.Current.Shutdown();
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&  element.Tag is string viewName)
            {
                if (!string.IsNullOrEmpty(viewName) && !navigateItems.ContainsKey(viewName))
                {
                    var type = Type.GetType(viewName);
                    var uc = Activator.CreateInstance(type) as UserControl;
                    navigateItems.Add(viewName, uc);
                }
                border.Child = navigateItems[viewName];
            }
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MemuItems_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked.Value)
            {
                radioButton.IsChecked = false;
                radioButton.IsChecked = true;
            }
        }
    }
}
