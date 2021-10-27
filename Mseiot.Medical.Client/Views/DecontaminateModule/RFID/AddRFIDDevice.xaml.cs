using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// AddRFIDDevice.xaml 的交互逻辑
    /// </summary>
    public partial class AddRFIDDevice : UserControl
    {
        public bool IsSuccess { get; set; }
        public RFIDDevice RFIDDevice { get; set; } = new RFIDDevice();

        public AddRFIDDevice()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var result = await SocketProxy.Instance.AddRFIDDevice(RFIDDevice);
            if (result.IsSuccess)
            {
                RFIDDevice.RFIDDeviceID = result.Content;
                IsSuccess = true;
                this.Close();
            }
            else
            {
                MsPrompt.ShowDialog("保存失败");
            }
        }
    }
}
