using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Client.Core;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MsWindow
    {
        private readonly Dictionary<string, UserControl> navigateItems 
            = new Dictionary<string, UserControl>();
        private ObservableCollection<string> navCol;

        public MainWindow()
        {
            InitializeComponent();
            this.navCol = new ObservableCollection<string>() { "首页" };
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //lb_nav.ItemsSource = this.navCol;
            UpdateTime();
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
            Restart();
        }

        private void ModifyUserPwd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&  element.Tag is string viewName)
            {
                //pop_system.IsOpen = pop_director.IsOpen = pop_clean.IsOpen = false;
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

        private void CleanCenter_Checked(object sender, RoutedEventArgs e)
        {
            //pop_clean.IsOpen = true;
        }

        private void DirectorManage_Checked(object sender, RoutedEventArgs e)
        {
            //pop_director.IsOpen = true;
        }
    }
}
