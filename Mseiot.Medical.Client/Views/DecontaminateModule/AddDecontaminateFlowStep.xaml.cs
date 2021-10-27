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
    /// AddDecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDecontaminateFlowStep : UserControl
    {
        public bool IsSuccess { get; set; }
        public DecontaminateFlowStep DecontaminateFlowStep { get; set; } = new DecontaminateFlowStep();

        public AddDecontaminateFlowStep(DecontaminateFlowStep decontaminateFlowStep = null)
        {
            InitializeComponent();
            if (DecontaminateFlowStep != null)
            {
                DecontaminateFlowStep = decontaminateFlowStep;
            }
            LoadRFIDDevices();
            DataContext = this;
        }

        private async void LoadRFIDDevices()
        {
            var result = await SocketProxy.Instance.GetRFIDDevices();
            if (result.IsSuccess)
            {
                cbRFIDDevices.ItemsSource = result.Content;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {

            if (DecontaminateFlowStep.DecontaminateFlowID == 0)
            {
                var result = await SocketProxy.Instance.AddDecontaminateFlowStep(DecontaminateFlowStep);
                if (result.IsSuccess)
                {
                    DecontaminateFlowStep.DecontaminateFlowID = result.Content;
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
                var result = await SocketProxy.Instance.ModifyDecontaminateFlowStep(DecontaminateFlowStep);
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
