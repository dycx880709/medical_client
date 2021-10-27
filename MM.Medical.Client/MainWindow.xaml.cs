using MM.Medical.Client.Core;
using MM.Medical.Client.Views.AppointmentModule;
using Ms.Controls;
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

namespace MM.Medical.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            Title = CacheHelper.APPTitle;

         
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;
            UpdateTime();
            SetChild();

        }

        #region 子页面

        private void SetChild()
        {
            borderContainer.Child = new AppointmentManage();
        }

        #endregion

        #region 导航栏

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

        #endregion

        private void Move_Click(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton== MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
