using MM.Medical.Decontaminate.Core;
using MM.Medical.Decontaminate.Views.Decontaminate;
using MM.Medical.Decontaminate.Views.EndoscopeViews;
using MM.Medical.Decontaminate.Views.SystemViews;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MM.Medical.Decontaminate.Views
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            UpdateTime();
            LoadMenus();
        }

        #region 菜单栏

        public List<Entities.Menu> Menus { get; set; } = new List<Entities.Menu>();

        public void LoadMenus()
        {
            Menus.Add(new Entities.Menu
            {
                Name = "消洗中心",
                Identify = "DecontaminateCenter"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "内镜管理",
                Identify = "EndoscopeManage"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "设备管理",
                Identify = "RFIDManage"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "流程管理",
                Identify = "DecontaminateFlowManage"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "系统设置",
                Identify = "SystemSetting"
            });
            lvMenus.ItemsSource = Menus;
            lvMenus.SelectedIndex = 0;
        }

        DecontaminateTaskView decontaminateTaskView = new DecontaminateTaskView();

        private void Menu_Selected(object sender, SelectionChangedEventArgs e)
        {
            var menu = lvMenus.SelectedItem as Entities.Menu;
            switch (menu.Identify)
            {
                case "EndoscopeManage":
                    container.Child = new EndoscopeManage();
                    break;
                case "RFIDManage":
                    container.Child = new RFID.RFIDDeviceManage();
                    break;
                case "DecontaminateCenter":
                    container.Child = decontaminateTaskView;
                    break;
                case "SystemSetting":
                    container.Child = new SystemViews.SystemSetting();
                    break;
                case "DecontaminateFlowManage":
                    container.Child = new DecontaminateFlowView();
                    break;
            }
        }

        #endregion

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
            App.Current.Shutdown();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }

        private void ModifyUserPwd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

      
    }
}
