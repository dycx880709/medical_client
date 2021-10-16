using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Client.Core;
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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// InspectionManageView.xaml 的交互逻辑
    /// </summary>
    public partial class InspectionManageView : UserControl
    {
        public InspectionManageView()
        {
            InitializeComponent();
            this.Loaded += InspectionManageView_Loaded;
        }

        private void InspectionManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
            GetPatientInfos();
        }

        private void GetPatientInfos()
        {
            if (this.IsLoaded)
            {
                var patientStatuses = new List<PatientStatus>();
                if (rb_all.IsChecked.Value)
                    patientStatuses.AddRange(new PatientStatus[] { PatientStatus.Regist, PatientStatus.Checking, PatientStatus.Checked });
                else if (rb_waiting.IsChecked.Value)
                    patientStatuses.Add(PatientStatus.Regist);
                else
                    patientStatuses.Add(PatientStatus.Checked);
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetPatients(
                    1,
                    1000,
                    checkTime: (int)TimeHelper.ToUnixDate(DateTime.Now),
                    patientStatuses: new PatientStatus[] { PatientStatus.Regist, PatientStatus.Checking, PatientStatus.Checked }
                ));
                if (result.IsSuccess) dg_patients.ItemsSource = result.Content.Results;
                else MsWindow.ShowDialog($"获取病人信息失败,{ result.Error }", "软件提示");
            }
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetBaseWords(
                "HIV",
                "HCV",
                "HBasg",
                "组织采取",
                "细胞采取",
                "插入途径",
                "检查体位",
                "麻醉方法",
                "术前用药"
            ));
            cb_bodyLoc.ItemsSource = result.SplitContent("检查体位");
            cb_anesthesia.ItemsSource = result.SplitContent("麻醉方法");
            cb_preoperative.ItemsSource = result.SplitContent("术前用药");
            cb_insert.ItemsSource = result.SplitContent("插入途径");
            cb_org.ItemsSource = result.SplitContent("组织采取");
            cb_cell.ItemsSource = result.SplitContent("细胞采取");
            cb_hiv.ItemsSource = result.SplitContent("HIV");
            cb_hcv.ItemsSource = result.SplitContent("HCV");
            cb_hbasg.ItemsSource = result.SplitContent("HBasg");
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PatientStatus_CheckChanged(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }
    }
}
