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
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;
            LoadMenus();
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

            Menus.Add(new Entities.Menu
            {
                Name = "预约登记",
                Identify = "MM.Medical.Client.Views.AppointmentManage",
            });

            Menus.Add(new Entities.Menu
            {
                Name = "检查中心",
                Identify = "MM.Medical.Client.Views.ExaminationManageView",
            });

            Menus.Add(new Entities.Menu
            {
                Name = "病历中心",
                Identify = "MM.Medical.Client.Views.MedicalManageView",
            });

            Menus.Add(new Entities.Menu
            {
                Name = "主任管理",
                Identify = "",
                Children = new List<Entities.Menu>
                {
                    new Entities.Menu{
                        Name = "条件统计",
                        Identify = "MM.Medical.Client.Views.ConditionStatisticsView",
                        },
                     new Entities.Menu{
                        Name = "专项统计",
                        Identify = "MM.Medical.Client.Views.SpecialStatisticsView",
                        },
                }
            });


            Menus.Add(new Entities.Menu
            {
                Name = "系统管理",
                Identify = "",
                Children = new List<Entities.Menu>
                {
                    new Entities.Menu{
                        Name = "用户管理",
                        Identify = "MM.Medical.Client.Views.UserManageView",
                        },
                     new Entities.Menu{
                        Name = "角色管理",
                        Identify = "MM.Medical.Client.Views.RoleManageView",
                        },
                      new Entities.Menu{
                        Name = "系统设置",
                        Identify = "MM.Medical.Client.Views.SystemSettingView",
                        },
                       new Entities.Menu{
                        Name = "基础词库",
                        Identify = "MM.Medical.Client.Views.BaseWordView",
                        },
                        new Entities.Menu{
                        Name = "诊断模板",
                        Identify = "MM.Medical.Client.Views.DiagnosticTemplateView",
                        },
                        new Entities.Menu{
                        Name = "医学词库",
                        Identify = "MM.Medical.Client.Views.MedicalWordView",
                        },
                        new Entities.Menu{
                        Name = "诊室管理",
                        Identify = "MM.Medical.Client.Views.ConsultingManageView",
                        },
                        new Entities.Menu{
                        Name = "数据备份",
                        Identify = "MM.Medical.Client.Views.DataBackingView",
                        },
                }
            });


            
            lvMenus.ItemsSource = Menus;
            lvMenus.SelectedIndex = 0;
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
                if (!string.IsNullOrEmpty(menu.Identify) && !navigateItems.ContainsKey(menu.Identify))
                {
                    var type = Type.GetType(menu.Identify);
                    var uc = Activator.CreateInstance(type) as UserControl;
                    navigateItems.Add(menu.Identify, uc);
                }
                if (!string.IsNullOrEmpty(menu.Identify))
                {
                    border.Child = navigateItems[menu.Identify];
                }
             
            }
        }
    }
}
