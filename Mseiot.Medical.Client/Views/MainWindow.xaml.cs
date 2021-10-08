using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Client.Core;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
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

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&  element.Tag is string viewName)
            {
                if (pop_clean != null)
                    pop_clean.IsOpen = viewName.Equals("Mseiot.Medical.Client.Views.CleanCenterView");
                if (!string.IsNullOrEmpty(viewName) && !navigateItems.ContainsKey(viewName))
                {
                    var type = Type.GetType(viewName);
                    var uc = Activator.CreateInstance(type) as UserControl;
                    var headerHeight = Convert.ToDouble(App.Current.Resources["Title_Header_Height"]);
                    uc.Margin = new Thickness(0, -headerHeight, 0, 0);
                    navigateItems.Add(viewName, uc);
                }
                border.Child = navigateItems[viewName];
            }
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
