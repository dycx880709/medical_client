using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// AddRFIDDevice.xaml 的交互逻辑
    /// </summary>
    public partial class AddRFIDDevice : UserControl
    {
        private readonly RFIDDevice rfidDevice;
        private readonly Loading loading;

        public AddRFIDDevice(RFIDDevice rfidDevice, Loading loading)
        {
            InitializeComponent();
        
            this.rfidDevice = rfidDevice.Copy();
   
            this.loading = loading;
            this.Loaded += AddRFIDDevice_Loaded;
        }

        private void AddRFIDDevice_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= AddRFIDDevice_Loaded;
            LoadSerialPorts();
            this.DataContext = this.rfidDevice;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            rfidDevice.Com = cbSerialPorts.SelectedItem as string;
            if (rfidDevice.RFIDDeviceID == 0)
            {
                var result = loading.AsyncWait("添加采集设备中,请稍后", SocketProxy.Instance.AddRFIDDevice(rfidDevice));
                if (result.IsSuccess)
                {
                    rfidDevice.RFIDDeviceID = result.Content;
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"添加采集设备失败,{ result.Error }");
            }
            else
            {
                var result = loading.AsyncWait("修改采集设备中,请稍后", SocketProxy.Instance.ModifyRFIDDevice(rfidDevice));
                if (result.IsSuccess)
                {
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"修改采集设备失败,{ result.Error }");
            }
        }

        #region 串口

        private void LoadSerialPorts()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            cbSerialPorts.ItemsSource = serialPorts;
            cbSerialPorts.SelectedItem = rfidDevice.Com;
        }

        private void SerialPorts_DropDownOpened(object sender, EventArgs e)
        {
            LoadSerialPorts();
        }

        #endregion
    }
}
