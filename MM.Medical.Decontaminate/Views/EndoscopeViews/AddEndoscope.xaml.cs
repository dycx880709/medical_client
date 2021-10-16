using MM.Libs.RFID;
using MM.Medical.Decontaminate.Core;
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

namespace MM.Medical.Decontaminate.Views.EndoscopeViews
{
    /// <summary>
    /// AddRFIDDevice.xaml 的交互逻辑
    /// </summary>
    public partial class AddEndoscope : UserControl
    {
        public bool IsSuccess { get; set; }
        public Endoscope Endoscope { get; set; } = new Endoscope();

        public AddEndoscope(Endoscope endoscope = null)
        {
            InitializeComponent();
            if (endoscope != null)
            {
                Endoscope = endoscope;
            }
            DataContext = this;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {

            if (Endoscope.EndoscopeID == 0)
            {
                var result = await SocketProxy.Instance.AddEndoscope(Endoscope);
                if (result.IsSuccess)
                {
                    Endoscope.EndoscopeID = result.Content;
                    IsSuccess = true;
                    this.Close();
                }
                else
                {
                    MsPrompt.ShowDialog("保存失败");
                }
            }
            else
            {
                var result = await SocketProxy.Instance.ModifyEndoscope(Endoscope);
                if (result.IsSuccess)
                {
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
}
