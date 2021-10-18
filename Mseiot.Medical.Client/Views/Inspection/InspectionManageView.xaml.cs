using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Client.Core;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// InspectionManageView.xaml 的交互逻辑
    /// </summary>
    public partial class InspectionManageView : UserControl
    {
        public bool IsChecking
        {
            get { return (bool)GetValue(IsCheckingProperty); }
            set { SetValue(IsCheckingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecking.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckingProperty =
            DependencyProperty.Register("IsChecking", typeof(bool), typeof(InspectionManageView), new PropertyMetadata(false));

        private string consultingRoom = "";

        public InspectionManageView()
        {
            InitializeComponent();
            this.consultingRoom = CacheHelper.GetConfig("ConsultingRoom");
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
                if (string.IsNullOrEmpty(consultingRoom.Trim()))
                {
                    MsWindow.ShowDialog("诊室信息未配置", "软件提示");
                    return;
                }
                var patientStatuses = new List<PatientStatus>();
                if (rb_all.IsChecked.Value)
                    patientStatuses.AddRange(new PatientStatus[] { PatientStatus.Regist, PatientStatus.Checking, PatientStatus.Checked });
                else if (rb_waiting.IsChecked.Value)
                    patientStatuses.Add(PatientStatus.Regist);
                else
                    patientStatuses.Add(PatientStatus.Checked);
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetPatientInfos(
                    1,
                    1000,
                    checkTime: (int)TimeHelper.ToUnixDate(DateTime.Now),
                    consultingRooms: new string[] { consultingRoom.Trim() },
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

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && dg_patients.SelectedValue is PatientInfo info)
            {
                if (tb.IsChecked.Value)
                {
                    info.PatientStatus = PatientStatus.Checking;
                    var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyPatientInfo(info));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"启动检查失败,{ result.Error }", "软件提示");
                    else info.CheckInfo.CheckTime = info.CheckTime = (int)TimeHelper.ToUnixTime(DateTime.Now);
                }
                else
                {
                    info.PatientStatus = PatientStatus.Checked;
                    var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyPatientInfo(info));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"结束检查失败,{ result.Error }", "软件提示");
                }
            }
        }

        private void PatientStatus_CheckChanged(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (dg_patients.SelectedValue is PatientInfo info)
            {
                if (info.CheckInfo.ReportTime == 0)
                {
                    info.CheckInfo.ReportTime = (int)TimeHelper.ToUnixTime(DateTime.Now);
                    var result = loading.AsyncWait("生成报告中,请稍后", SocketProxy.Instance.ModifyPatientInfo(info));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"生成报告失败,{ result.Error }", "软件提示");
                }
            }
        }
    }
}
