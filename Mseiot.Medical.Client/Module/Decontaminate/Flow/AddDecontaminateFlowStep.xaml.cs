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
    /// AddDecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDecontaminateFlowStep : UserControl
    {
        private readonly DecontaminateFlowStep decontaminateFlowStep;
        private readonly DecontaminateFlowStep decontaminateFlowStep_orgin;
        private readonly Loading loading;
        public AddDecontaminateFlowStep(DecontaminateFlowStep decontaminateFlowStep, Loading loading)
        {
            InitializeComponent();
            this.decontaminateFlowStep = decontaminateFlowStep.Copy();
            this.decontaminateFlowStep_orgin = decontaminateFlowStep;
            this.loading = loading;
            LoadRFIDDevices();
            DataContext = this.decontaminateFlowStep;
        }

        private void LoadRFIDDevices()
        {
            var result = loading.AsyncWait("获取采集设备中,请稍后", SocketProxy.Instance.GetRFIDDevices());
            if (result.IsSuccess) cbRFIDDevices.ItemsSource = result.Content;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (decontaminateFlowStep.DecontaminateFlowID == 0)
            {
                var result = loading.AsyncWait("新建流程步骤中,请稍后", SocketProxy.Instance.AddDecontaminateFlowStep(decontaminateFlowStep));
                if (result.IsSuccess)
                {
                    decontaminateFlowStep.DecontaminateFlowID = result.Content;
                    decontaminateFlowStep.CopyTo(decontaminateFlowStep_orgin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "新建保存失败:"+result.Error);
            }
            else
            {
                var result = loading.AsyncWait("编辑流程步骤中,请稍后", SocketProxy.Instance.ModifyDecontaminateFlowStep(decontaminateFlowStep));
                if (result.IsSuccess)
                {
                    decontaminateFlowStep.CopyTo(decontaminateFlowStep_orgin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "编辑保存失败:" + result.Error);
            }
        }
    }
}
