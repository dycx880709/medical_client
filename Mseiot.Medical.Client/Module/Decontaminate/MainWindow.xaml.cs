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
using System.Windows.Input;
using System.Threading.Tasks;

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTime();
            LoadMenus();
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
            //var view = new ResetPwdView(this.loading);
            //child.ShowDialog("修改密码", view);
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
                Name = "采集设备",
                Identify = "RFIDManage"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "流程管理",
                Identify = "DecontaminateFlowManage"
            });
            Menus.Add(new Entities.Menu
            {
                Name = "清洗记录",
                Identify = "DecontaminateTaskManage"
            });
            Menus.Add(new Entities.Menu { Name = "清洗酶", Identify = "EnzymeManage" });
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
                    container.Child = new RFIDDeviceManage();
                    break;
                case "DecontaminateCenter":
                    container.Child = decontaminateTaskView;
                    break;
                case "DecontaminateFlowManage":
                    container.Child = new DecontaminateFlowView();
                    break;
                case "DecontaminateTaskManage":
                    container.Child = new DecontaminateTaskManage();
                    break;
                case "EnzymeManage":
                    container.Child = new EnzymeManage();
                    break;
            }
        }

        #endregion

        #region 设置

        private void ShowSetting(object sender, EventArgs e)
        {
            DecontaminateSetting decontaminateSetting = new DecontaminateSetting();
            ContentWindow.Show(decontaminateSetting);
        }

        #endregion
    }
}
