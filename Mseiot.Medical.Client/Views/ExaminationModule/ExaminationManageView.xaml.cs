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
using Ms.Libs.TcpLib;
using System.Threading.Tasks;
using Mseiot.Medical.Service.Models;

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

        public bool IsDoctorVisit
        {
            get { return (bool)GetValue(IsDoctorVisitProperty); }
            set { SetValue(IsDoctorVisitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDoctorVisit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDoctorVisitProperty =
            DependencyProperty.Register("IsDoctorVisit", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));

        private int consultingRoomId;

        public ExaminationManageView()
        {
            InitializeComponent();
            this.Loaded += ExaminationManageView_Loaded;
            this.IsEnabled = false;
        }

        private void ExaminationManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
            SocketProxy.Instance.TcpProxy.ConnectStateChanged += TcpProxy_ConnectStateChanged;
            SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
            LoadConsultingRoom();
        }

        private void LoadConsultingRoom()
        {
            var consultingRoomName = CacheHelper.GetConfig("ConsultingRoom");
            if (!string.IsNullOrEmpty(consultingRoomName))
            {
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.LoginConsultingRoom(consultingRoomName));
                if (result.IsSuccess)
                {
                    this.IsEnabled = true;
                    this.consultingRoomId = result.Content.ConsultingRoomID;
                    this.IsDoctorVisit = result.Content.IsUsed;
                    LoadAppointmentInfos();
                }
                else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
            }
            else Alert.ShowMessage(false, AlertType.Error, $"检查诊室未配置,模块不可能");
        }

        private void TcpProxy_ReceiveMessaged(object sender, Message e)
        {
            if (e.Module == Command.Module_Appointment)
            {
                if (e.Method == Command.ChangeStatus_Appointment)
                {

                }
            }
        }

        private void TcpProxy_ConnectStateChanged(object sender, ConnectStateArgs e)
        {
            switch (e.ConnectState)
            {
                case ConnectState.Success:
                    SocketProxy.Instance.TcpProxy.ReceiveMessaged -= TcpProxy_ReceiveMessaged;
                    SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
                    this.Dispatcher.Invoke(() => LoadConsultingRoom());
                    break;
                case ConnectState.Faild:
                    SocketProxy.Instance.TcpProxy.ReceiveMessaged -= TcpProxy_ReceiveMessaged;
                    this.Dispatcher.Invoke(() => this.IsEnabled = false);
                    break;
            }
        }

        private void LoadAppointmentInfos()
        {
            if (this.IsLoaded)
            {
                if (this.consultingRoomId == 0)
                    return;
                var appointmentStatuses = new List<AppointmentStatus>();
                if (rb_all.IsChecked.Value)
                    appointmentStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Waiting, AppointmentStatus.Reported, AppointmentStatus.Checking, AppointmentStatus.Checked });
                else if (rb_waiting.IsChecked.Value)
                    appointmentStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Waiting, AppointmentStatus.Checking });
                else
                    appointmentStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Checked, AppointmentStatus.Reported });
                var startTime = TimeHelper.ToUnixDate(DateTime.Now);
                var endTime = startTime + 24 * 60 * 60 - 1;
                pager.SelectedCount = dg_appointments.GetFullCountWithoutScroll();
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetAppointments(
                    pager.PageIndex,
                    pager.SelectedCount, 
                    TimeHelper.FromUnixTime(startTime),
                    TimeHelper.FromUnixTime(endTime),
                    userInfo: "",
                    consultingRoomID: consultingRoomId,
                    appointmentStatuses: new AppointmentStatus[] { AppointmentStatus.Checking, AppointmentStatus.Checked, AppointmentStatus.Waiting, AppointmentStatus.Reported }));
                if (result.IsSuccess)
                {
                    dg_appointments.ItemsSource = new ObservableCollection<Appointment>(result.Content.Results);
                    pager.TotalCount = result.Content.Total;
                }
                else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
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
                if (info.Examination == null) info.Examination = new Examination();
                if (string.IsNullOrEmpty(info.Examination.DoctorName))
                    info.Examination.DoctorName = CacheHelper.CurrentUser.Name;
                if (tb.IsChecked.Value)
                {
                    info.AppointmentStatus = AppointmentStatus.Checking;
                    var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyAppointment(info));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result.Error }");
                        info.AppointmentStatus = oldAppointmentStatus;
                    }
                    else Alert.ShowMessage(true, AlertType.Success, "检查已启动");
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
            LoadAppointmentInfos();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAppointmentInfos();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment info)
            {
                if (info.Examination.ReportTime == 0)
                {
                    info.Examination.ReportTime = (int)TimeHelper.ToUnixTime(DateTime.Now);
                    var result = loading.AsyncWait("生成报告中,请稍后", SocketProxy.Instance.ModifyAppointment(info));
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

        private void StartCheck_Click(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("更新出诊状态中,请稍后", SocketProxy.Instance.AcceptConsultingRoom(this.consultingRoomId, this.IsDoctorVisit));
            if (!result.IsSuccess)
            {
                Alert.ShowMessage(true, AlertType.Error, $"设置出诊状态失败,{ result.Error }");
                this.IsDoctorVisit = !this.IsDoctorVisit;
            }
        }

        private void ExaminationText_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Shotcut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CaptureSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
