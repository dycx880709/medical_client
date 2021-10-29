using Ms.Controls;
using Ms.Libs.SysLib;
using MM.Medical.Client.Core;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ms.Controls.Core;
using System.Collections.ObjectModel;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ExaminationManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ExaminationManageView : UserControl
    {
        public bool IsChecking
        {
            get { return (bool)GetValue(IsCheckingProperty); }
            set { SetValue(IsCheckingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecking.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckingProperty =
            DependencyProperty.Register("IsChecking", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));

        private string consultingRoom = "";

        public ExaminationManageView()
        {
            InitializeComponent();
            this.consultingRoom = CacheHelper.GetConfig("ConsultingRoom");
            this.Loaded += ExaminationManageView_Loaded;
        }

        private void ExaminationManageView_Loaded(object sender, RoutedEventArgs e)
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
                    Alert.ShowMessage(true, AlertType.Error, "诊室信息未配置");
                    return;
                }
                var patientStatuses = new List<AppointmentStatus>();
                if (rb_all.IsChecked.Value)
                    patientStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Waiting, AppointmentStatus.Checking, AppointmentStatus.Checked });
                else if (rb_waiting.IsChecked.Value)
                    patientStatuses.Add(AppointmentStatus.Waiting);
                else
                    patientStatuses.Add(AppointmentStatus.Checked);
                //var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetAppointments(
                //    1,
                //    1000,
                //    checkTime: (int)TimeHelper.ToUnixDate(DateTime.Now),
                //    consultingRooms: new string[] { consultingRoom.Trim() },
                //    patientStatuses: new AppointmentStatus[] { AppointmentStatus.Waiting, AppointmentStatus.Checking, AppointmentStatus.Checked }
                //));
                var overTime = TimeHelper.ToUnixDate(DateTime.Now) % (24 * 60 * 60);
                var startTime = TimeHelper.ToUnixDate(DateTime.Now) - overTime;
                var endTime = startTime + 24 * 60 * 60 - 1;
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetAppointments(TimeHelper.FromUnixTime(startTime), TimeHelper.FromUnixTime(endTime), ""));
                if (result.IsSuccess) dg_appointments.ItemsSource = new ObservableCollection<Appointment>(result.Content);
                else Alert.ShowMessage(true, AlertType.Error, $"获取预约信息失败,{ result.Error }");
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
                "检查部位",
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
            cb_body.ItemsSource = result.SplitContent("检查部位");
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && dg_appointments.SelectedValue is Appointment info)
            {
                var oldAppointmentStatus = info.AppointmentStatus;
                if (tb.IsChecked.Value)
                {
                    info.AppointmentStatus = AppointmentStatus.Checking;
                    var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyAppointment(info));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result.Error }");
                        info.AppointmentStatus = oldAppointmentStatus;
                    }
                    else
                        Alert.ShowMessage(true, AlertType.Success, "检查已启动");
                }
                else
                {
                    info.AppointmentStatus = AppointmentStatus.Checked;
                    var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyAppointment(info));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{ result.Error }");
                        info.AppointmentStatus = oldAppointmentStatus;
                    }
                    else Alert.ShowMessage(true, AlertType.Success, "检查已结束");
                }
            }
        }

        private void AppointmentStatus_CheckChanged(object sender, RoutedEventArgs e)
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
            if (dg_appointments.SelectedValue is PatientInfo info)
            {
                if (info.CheckInfo.ReportTime == 0)
                {
                    info.CheckInfo.ReportTime = (int)TimeHelper.ToUnixTime(DateTime.Now);
                    var result = loading.AsyncWait("生成报告中,请稍后", SocketProxy.Instance.ModifyPatientInfo(info));
                    if (!result.IsSuccess) Alert.ShowMessage(true, AlertType.Error, $"生成报告失败,{ result.Error }");
                }
            }
        }

        private void SelectedBody_Click(object sender, RoutedEventArgs e)
        {
            cb_body.Text = string.Empty;
            for (int i = 0; i < cb_body.Items.Count; i++)
            {
                var cbi = cb_body.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                var cb = ControlHelper.GetVisualChild<CheckBox>(cbi);
                if (cb.IsChecked.Value)
                    cb_body.Text += cb.Content.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(cb_body.Text))
                cb_body.Text = cb_body.Text.Substring(0, cb_body.Text.Length - 1);
        }
    }
}
