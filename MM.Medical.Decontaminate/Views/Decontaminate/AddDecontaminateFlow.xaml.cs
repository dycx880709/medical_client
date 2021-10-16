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

namespace MM.Medical.Decontaminate.Views.Decontaminate
{
    /// <summary>
    /// AddDecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDecontaminateFlow : UserControl
    {
        public bool IsSuccess { get; set; }
        public DecontaminateFlow DecontaminateFlow { get; set; } = new DecontaminateFlow();

        public AddDecontaminateFlow(DecontaminateFlow decontaminateFlow = null)
        {
            InitializeComponent();
            if (decontaminateFlow != null)
            {
                DecontaminateFlow = decontaminateFlow;
            }
            DataContext = this;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {

            if (DecontaminateFlow.DecontaminateFlowID == 0)
            {
                var result = await SocketProxy.Instance.AddDecontaminateFlow(DecontaminateFlow);
                if (result.IsSuccess)
                {
                    DecontaminateFlow.DecontaminateFlowID = result.Content;
                    IsSuccess = true;
                    this.Close();
                }
                else
                {
                    MsPrompt.ShowDialog("保存失败:"+result.Error);
                }
            }
            else
            {
                var result = await SocketProxy.Instance.ModifyDecontaminateFlow(DecontaminateFlow);
                if (result.IsSuccess)
                {
                    IsSuccess = true;
                    this.Close();
                }
                else
                {
                    MsPrompt.ShowDialog("保存失败:" + result.Error);
                }
            }
        }
    }
}
