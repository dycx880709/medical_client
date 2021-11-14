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
using System.Net.Sockets;
using MM.Medical.Client.Entities;
using System.Linq;
using MM.Medical.Client.Module.Decontaminate;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        private readonly Dictionary<string, UserControl> navigateItems 
            = new Dictionary<string, UserControl>();

        public MainWindow()
        {
            InitializeComponent();
            LoadSetting();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;
            LoadMenus();
            UpdateTime();
            LoadTcp();
        }

        private async void LoadTcp()
        {
            var levels = CacheHelper.CurrentUser.Authority.Split(',');
            if (levels.Contains("1") || levels.Contains("3") || CacheHelper.CurrentUser.LoginName.Equals("admin"))
                await ConnectServer();
        }

        private void LoadSetting()
        {
            var levels = CacheHelper.CurrentUser.Authority.Split(',');
            if (levels.Contains("2") || CacheHelper.CurrentUser.LoginName.Equals("admin"))
            {
                this.VisibilitySetting = Visibility.Visible;
                this.NotifySetting += (_, e) =>
                {
                    var decontaminateSetting = new DecontaminateSetting();
                    ContentWindow.Show(decontaminateSetting);
                };
            }
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
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
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

        #region 菜单

        public List<Entities.Menu> Menus { get; set; } = new List<Entities.Menu>();

        public void LoadMenus()
        {
            Menus.Clear();
            var levels = CacheHelper.CurrentUser.Authority.Split(',');
            var appLevels = AppLevel.Levels.Copy();
            foreach (var appLevel in appLevels)
                Menus.AddRange(LoadMenus(appLevel.Children, levels, CacheHelper.CurrentUser.LoginName.Equals("admin")));
            lvMenus.ItemsSource = Menus;
            lvMenus.SelectedIndex = 0;
        }

        public List<Entities.Menu> LoadMenus(List<AppLevel> appLevels, string[] levels, bool isAdmin = false)
        {
            List<Entities.Menu> menus = new List<Entities.Menu>();
            if (appLevels != null && appLevels.Count > 0)
            {
                foreach (var appLevel in appLevels)
                {
                    if (isAdmin || levels.Any(t => t.Equals(appLevel.Level)))
                    {
                        var menu = new Entities.Menu
                        {
                            Name = appLevel.Name,
                            Identify = appLevel.Identify,
                            Reusability = appLevel.Reusability,
                        };
                        menus.Add(menu);
                        if (appLevel.Children != null && appLevel.Children.Count > 0)
                            menu.Children = LoadMenus(appLevel.Children, levels, isAdmin);
                    }
                }
            }
            return menus;
        }

        #endregion

        private void Menu_Selected(object sender, SelectionChangedEventArgs e)
        {
            Entities.Menu menu = (sender as ListView).SelectedItem as Entities.Menu;
            if (menu != null)
            {
                if (sender != lvMenus)
                {
                    lvMenus.SelectedIndex = -1;
                }
                if (!string.IsNullOrEmpty(menu.Identify))
                {
                    if (menu.Reusability)
                    {
                        if (!navigateItems.ContainsKey(menu.Identify))
                        {
                            var type = Type.GetType(menu.Identify);
                            var uc = Activator.CreateInstance(type) as UserControl;
                            navigateItems.Add(menu.Identify, uc);
                        }
                        border.Child = navigateItems[menu.Identify];
                    }
                    else
                    {
                        var type = Type.GetType(menu.Identify);
                        var uc = Activator.CreateInstance(type) as UserControl;
                        uc.Unloaded += (s, _) => (s as IDisposable)?.Dispose();
                        border.Child = uc;
                    }
                }
            }
        }
    }
}
