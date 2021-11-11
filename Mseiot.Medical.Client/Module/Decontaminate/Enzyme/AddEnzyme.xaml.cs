using MM.Libs.RFID;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// AddRFIDDevice.xaml 的交互逻辑
    /// </summary>
    public partial class AddEnzyme : UserControl
    {
        private readonly Enzyme enzyme;
        private readonly Enzyme enzyme_origin;
        private readonly Loading loading;

        public AddEnzyme(Enzyme enzyme, Loading loading)
        {
            InitializeComponent();
            this.enzyme_origin = enzyme;
            this.enzyme = enzyme.Copy();
            this.loading = loading;
            DataContext = this.enzyme;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (enzyme.EnzymeID == 0)
            {
                var result = loading.AsyncWait("添加清洗酶中,请稍后", SocketProxy.Instance.AddEnzyme(enzyme));
                if (result.IsSuccess)
                {
                    enzyme.EnzymeID = result.Content;
                    enzyme.CopyTo(enzyme_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"添加清洗酶失败,{ result.Error }");
            }
            else
            {
                var result = loading.AsyncWait("编辑清洗酶中,请稍后", SocketProxy.Instance.ModifyEnzyme(enzyme));
                if (result.IsSuccess)
                {
                    enzyme.CopyTo(enzyme_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"编辑清洗酶失败,{ result.Error }");
            }
        }
    }
}
