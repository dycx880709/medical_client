using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// RFIDDeviceManage.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDDeviceManage : UserControl
    {
        public ObservableCollection<RFIDDevice> RFIDDevices { get; set; } = new ObservableCollection<RFIDDevice>();

        public RFIDDeviceManage()
        {
            InitializeComponent();
            this.Loaded += RFIDDeviceManage_Loaded;
        }

        private void RFIDDeviceManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= RFIDDeviceManage_Loaded;
            lvDatas.ItemsSource = RFIDDevices;
            LoadRFIDDevices();
        }

        #region 数据

        private void LoadRFIDDevices()
        {
            RFIDDevices.Clear();
            var result = loading.AsyncWait("获取采集设备中,请稍后", SocketProxy.Instance.GetRFIDDevices());
            if (result.IsSuccess) RFIDDevices.AddRange(result.Content);
            else Alert.ShowMessage(true, AlertType.Error, $"获取采集设备失败,{ result.Error }");
        }

        #endregion

        #region 添加、删除

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var rfidDevice = new RFIDDevice();
            var addRFIDDevice = new AddRFIDDevice(rfidDevice, this.loading);
            if (child.ShowDialog("添加采集设备", addRFIDDevice))
                LoadRFIDDevices();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmWindow.Show("是否继续?"))
            {
                var rfidDevice = (sender as FrameworkElement).Tag as RFIDDevice;
                var result = loading.AsyncWait("删除采集设备中,请稍后", SocketProxy.Instance.RemoveRFIDDevices(new List<int> { rfidDevice.RFIDDeviceID }));
                if (result.IsSuccess) LoadRFIDDevices();
                else Alert.ShowMessage(true, AlertType.Error, $"删除采集设备失败,{ result.Error }");
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            var rfidDevice = (sender as FrameworkElement).Tag as RFIDDevice;
            var addRFIDDevice = new AddRFIDDevice(rfidDevice, this.loading);
            if (child.ShowDialog("修改采集设备", addRFIDDevice))
                LoadRFIDDevices();
        }

        #endregion
    }
}
