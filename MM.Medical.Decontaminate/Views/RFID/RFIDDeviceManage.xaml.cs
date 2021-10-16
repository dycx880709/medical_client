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

namespace MM.Medical.Decontaminate.Views.RFID
{
    /// <summary>
    /// RFIDDeviceManage.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDDeviceManage : UserControl
    {
        public RFIDDeviceManage()
        {
            InitializeComponent();
            this.Loaded += RFIDDeviceManage_Loaded;
        }

        private void RFIDDeviceManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= RFIDDeviceManage_Loaded;
            lvDatas.ItemsSource = RFIDDevices;
            LoadDatas();
        }

        #region 数据

        public ObservableCollection<RFIDDevice> RFIDDevices { get; set; } = new ObservableCollection<RFIDDevice>();

        private async void LoadDatas()
        {
            RFIDDevices.Clear();
            var result = await SocketProxy.Instance.GetRFIDDevices();
            if (result.IsSuccess)
            {
                if (result.Content != null)
                {
                    RFIDDevices.AddRange(result.Content);
                }
            }
        }

        #endregion

        #region 添加、删除

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddRFIDDevice addRFIDDevice = new AddRFIDDevice();
            MsWindow.ShowDialog(addRFIDDevice);
            if (addRFIDDevice.IsSuccess)
            {
                LoadDatas();
            }
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = await SocketProxy.Instance.RemoveRFIDDevices(RFIDDevices.Where(f => f.IsSelected).Select(f => f.RFIDDeviceID).ToList());
            if (result.IsSuccess)
            {
                LoadDatas();
            }
        }

        #endregion

        #region 选择

        private void AllDevice_Selected(object sender, RoutedEventArgs e)
        {
            foreach(var item in RFIDDevices)
            {
                item.IsSelected = (bool)(sender as CheckBox).IsChecked;
            }
        }

        #endregion
    }
}
