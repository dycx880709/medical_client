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
    public partial class AddEndoscope : UserControl
    {
        private readonly Endoscope endoscope;
        private readonly Endoscope endoscope_origin;
        private readonly Loading loading;

        public AddEndoscope(Endoscope endoscope, Loading loading)
        {
            InitializeComponent();
            this.endoscope_origin = endoscope;
            this.endoscope = endoscope.Copy();
            this.loading = loading;
            DataContext = this.endoscope;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (endoscope.EndoscopeID == 0)
            {
                var result = loading.AsyncWait("添加内窥镜中,请稍后", SocketProxy.Instance.AddEndoscope(endoscope));
                if (result.IsSuccess)
                {
                    endoscope.EndoscopeID = result.Content;
                    endoscope.CopyTo(endoscope_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"添加内窥镜失败,{ result.Error }");
            }
            else
            {
                var result = loading.AsyncWait("编辑内窥镜中,请稍后", SocketProxy.Instance.ModifyEndoscope(endoscope));
                if (result.IsSuccess)
                {
                    endoscope.CopyTo(endoscope_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"编辑内窥镜失败,{ result.Error }");
            }
        }
    }
}
