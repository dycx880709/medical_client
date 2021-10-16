using Ms.Controls;
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
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddRFIDDevice addRFIDDevice = new AddRFIDDevice();
            MsWindow.ShowDialog(addRFIDDevice);
        }
    }
}
