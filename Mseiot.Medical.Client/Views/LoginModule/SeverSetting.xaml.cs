using Ms.Controls;
using Ms.Controls.Models;
using MM.Medical.Client.Core;
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
    /// SeverSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SeverSetting : UserControl
    {
        public SeverSetting()
        {
            InitializeComponent();
            this.Loaded += SeverSetting_Loaded;
        }

        private void SeverSetting_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProperty();
        }

        private void LoadProperty()
        {
            var localSetting = CacheHelper.LocalSetting;
            tb_address.ItemsSource = localSetting.ServerSettingRecords.Select(t => t.Address).ToList();
            var serverSetting = localSetting.ServerSetting;
            tb_address.Text = serverSetting.Address ?? "";
            tb_httpPort.Text = serverSetting.HttpPort.ToString();
            tb_tcpPort.Text = serverSetting.TcpPort.ToString();
            tb_address.Focus();
        }

        private void tbAddress_Selected(object sender, CustomEventArgs e)
        {
            var localSetting = CacheHelper.LocalSetting;
            var serverSetting = localSetting.ServerSettingRecords.FirstOrDefault(t => t.Address.Equals(e.PropertyValue));
            tb_httpPort.Text = serverSetting.HttpPort.ToString();
            tb_tcpPort.Text = serverSetting.TcpPort.ToString();
            localSetting.ServerSetting = serverSetting;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var address = tb_address.Text.Trim();
            var httpPort = Convert.ToInt32(tb_httpPort.Text.Trim());
            var tcpPort = Convert.ToInt32(tb_tcpPort.Text.Trim());
            if (string.IsNullOrEmpty(address))
            {
                Alert.ShowMessage(true, AlertType.Warning, "服务地址不能为空");
                return;
            }
            if (httpPort == 0)
            {
                Alert.ShowMessage(true, AlertType.Warning, "Http端口不能为0");
                return;
            }
            if (tcpPort == 0)
            {
                Alert.ShowMessage(true, AlertType.Warning, "Tcp端口不能为0");
                return;
            }
            var localSetting = CacheHelper.LocalSetting;
            var serverSetting = localSetting.ServerSetting;
            serverSetting.Address = address;
            serverSetting.HttpPort = httpPort;
            serverSetting.TcpPort = tcpPort;
            var condition = localSetting.ServerSettingRecords.FirstOrDefault(t => t.Address.Equals(address));
            if (condition == null) localSetting.ServerSettingRecords.Add(serverSetting.Copy());
            else serverSetting.CopyTo(condition);
            CacheHelper.SaveLocalSetting();
            this.Close(true);
        }

        private void server_TextChanged(object sender, TextChangedEventArgs e)
        {
            btSave.IsEnabled = !string.IsNullOrEmpty(tb_address.Text.Trim()) &&
                !string.IsNullOrEmpty(tb_httpPort.Text.Trim()) &&
                !string.IsNullOrEmpty(tb_tcpPort.Text.Trim());
        }
    }
}
