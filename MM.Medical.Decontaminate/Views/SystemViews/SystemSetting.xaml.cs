﻿using MM.Medical.Decontaminate.Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace MM.Medical.Decontaminate.Views.SystemViews
{
    /// <summary>
    /// SystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : UserControl
    {
        public SystemSetting()
        {
            InitializeComponent();
            this.Loaded += SystemSetting_Loaded;
        }

        private void SystemSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SystemSetting_Loaded;
            LoadSerialPorts();
        }

        #region 串口

        private void LoadSerialPorts()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            cbSerialPorts.ItemsSource = serialPorts;
            cbSerialPorts.SelectedItem = CacheHelper.LocalSetting.RFIDCom;
        }
        private void cbSerialPorts_DropDownOpened(object sender, EventArgs e)
        {

            LoadSerialPorts();
        }

        #endregion

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            CacheHelper.LocalSetting.RFIDCom = cbSerialPorts.SelectedItem as string;
            CacheHelper.SaveLocalSetting();
        }
    }
}
