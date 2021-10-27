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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //lb_nav.ItemsSource = this.navCol;
            CacheHelper.GetConfig("");
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
                //if (this.IsLoaded)
                //{
                //    var popup = ControlHelper.GetLogicParent<Popup>(element);
                //    if (popup != null) popup.IsOpen = false;
                //}
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
