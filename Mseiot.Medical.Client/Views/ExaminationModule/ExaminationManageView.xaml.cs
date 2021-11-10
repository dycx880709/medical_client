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
using System.Linq;
using System.Windows.Input;
using Ms.Libs.Models;
using System.Windows.Data;
using System.ComponentModel;
using MM.Libs.RFID;
using System.Windows.Media.Imaging;
using System.IO;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ExaminationManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ExaminationManageView : UserControl
    {
        public ExaminationMedia SelectedMedia
        {
            get { return (ExaminationMedia)GetValue(SelectedMediaProperty); }
            set { SetValue(SelectedMediaProperty, value); }
        }
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public bool IsDoctorVisit
        {
            get { return (bool)GetValue(IsDoctorVisitProperty); }
            set { SetValue(IsDoctorVisitProperty, value); }
        }
        public bool IsFullScreen
        {
            get { return (bool)GetValue(IsFullScreenProperty); }
            set { SetValue(IsFullScreenProperty, value); }
        }

        public static readonly DependencyProperty SelectedMediaProperty = DependencyProperty.Register("SelectedMedia", typeof(ExaminationMedia), typeof(ExaminationManageView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsDoctorVisitProperty = DependencyProperty.Register("IsDoctorVisit", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
       
        public ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();
        private ICollectionView CollectionView { get { return CollectionViewSource.GetDefaultView(Appointments); }}
        public IEnumerable<MedicalTemplate> MedicalTemplates { get; set; }
        public IEnumerable<MedicalWord> OriginMedicalWords { get; set; }
        public IEnumerable<MedicalWord> MedicalWords { get; set; }
        public List<string> BodyParts { get; set; }

        private int consultingRoomId;

        public ExaminationManageView()
        {
            InitializeComponent();
            this.Loaded += ExaminationManageView_Loaded;
            dg_appointments.ItemsSource = this.Appointments;
            CollectionView.SortDescriptions.Add(new SortDescription("AppointmentStatus", ListSortDirection.Ascending));
            CollectionView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            CollectionView.Filter = t => t is Appointment appointment && (appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Cancel || appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Exprire);
            this.IsEnabled = false;
        }

        private void ExaminationManageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ExaminationManageView_Loaded;
            LoadConsultingRoom();
            LoadRFIDProxy();
            ResetCheckingExamination();
            SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
        }

        private void LoadRFIDProxy()
        {
            if (string.IsNullOrEmpty(CacheHelper.RFIDCom))
            {
                Alert.ShowMessage(false, AlertType.Error, "设备读卡器未配置");
            }
        }

        private async void ResetCheckingExamination()
        {
            var condition = Appointments.FirstOrDefault(t => t.AppointmentStatus == AppointmentStatus.Checking);
            if (condition != null)
            {
                loading.Start("检查恢复中,请稍后");
                await Task.Delay(1000);
                this.Dispatcher.Invoke(() =>
                {
                    dg_appointments.SelectedValue = condition;
                    this.IsEditable = true;
                    loading.Stop();
                    epv.video.SetSource(CacheHelper.EndoscopeDeviceID, true, OpenCvSharp.VideoCaptureAPIs.DSHOW);
                });
            }
            this.IsEnabled = true;
        }

        private void LoadConsultingRoom()
        {
            if (!string.IsNullOrEmpty(CacheHelper.ConsultingRoomName))
            {
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.LoginConsultingRoom(CacheHelper.ConsultingRoomName));
                if (result.IsSuccess)
                {
                    this.consultingRoomId = result.Content.ConsultingRoomID;
                    this.IsDoctorVisit = result.Content.IsUsed;
                    LoadAppointments();
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
                    var appointment = Newtonsoft.Json.JsonConvert.DeserializeObject<Appointment>(System.Text.Encoding.UTF8.GetString(e.Content));
                    if (appointment.ConsultingRoomName.Equals(CacheHelper.ConsultingRoomName))
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            var condition = Appointments.FirstOrDefault(t => t.AppointmentID.Equals(appointment.AppointmentID));
                            if (condition == null)
                            {
                                Appointments.Add(appointment);
                                CollectionView.Refresh();
                            }
                            else condition.AppointmentStatus = appointment.AppointmentStatus;
                        });
                    }
                }
            }
        }

        private void LoadAppointments()
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
                //var startTime = 0;
                //var endTime = int.MaxValue;
                var startTime = TimeHelper.ToUnixDate(DateTime.Now);
                var endTime = startTime + 24 * 60 * 60 - 1;
                //pager.SelectedCount = dg_appointments.GetFullCountWithoutScroll();
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetAppointments(
                    0,      //pager.PageIndex
                    1000,   //pager.SelectedCount
                    TimeHelper.FromUnixTime(startTime),
                    TimeHelper.FromUnixTime(endTime),
                    userInfo: "",
                    consultingRoomID: consultingRoomId,
                    appointmentStatuses: new AppointmentStatus[] { AppointmentStatus.Checking, AppointmentStatus.Checked, AppointmentStatus.Waiting, AppointmentStatus.Reported }));
                if (result.IsSuccess)
                {
                    Appointments.Clear();
                    Appointments.AddRange(result.Content.Results);
                    CollectionView.Refresh();
                    //pager.TotalCount = result.Content.Total;
                }
                else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && dg_appointments.SelectedValue is Appointment appointment)
            {
                var commit = appointment.Copy();
                var commit_ex = appointment.Examination.Copy();
                if (tb.IsChecked.Value)
                {
                    void CommitExamination(int endoscopeID)
                    {
                        commit_ex.DoctorName = CacheHelper.CurrentUser.Name;
                        commit_ex.EndoscopeID = endoscopeID;
                        commit_ex.AppointmentID = appointment.AppointmentID;
                        commit_ex.ExaminationTime = TimeHelper.ToUnixTime(DateTime.Now);
                        var result1 = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.AddExamination(commit_ex));
                        if (result1.IsSuccess)
                        {
                            commit_ex.ExaminationID = result1.Content;
                            commit_ex.CopyTo(appointment.Examination);
                            commit.AppointmentStatus = AppointmentStatus.Checking;
                            var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(commit));
                            if (!result.IsSuccess)
                            {
                                Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result.Error }");
                                tb.IsChecked = !tb.IsChecked.Value;
                            }
                            else
                            {
                                Alert.ShowMessage(true, AlertType.Success, "检查已启动");
                                commit.CopyTo(appointment);
                                CollectionView.Refresh();
                                epv.video.SetSource(CacheHelper.EndoscopeDeviceID, true, OpenCvSharp.VideoCaptureAPIs.DSHOW);
                            }
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result1.Error }");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
                    }
                    if (CacheHelper.IsDebug)
                        CommitExamination(0);
                    else
                    {
                        loading.Start("读取内窥镜信息中,请稍后");
                        var rfidProxy = new RFIDProxy();
                        rfidProxy.NotifyEPCReceived += (_, device) =>
                        {
                            this.Dispatcher.Invoke(() => CommitExamination(device.EPC));
                            rfidProxy.Close();
                        };
                        rfidProxy.NotifyDeviceStatusChanged += (_, status) =>
                        {
                            if (!status)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    loading.Stop();
                                    Alert.ShowMessage(true, AlertType.Error, "读取内窥镜失败,开启检查失败");
                                    tb.IsChecked = !tb.IsChecked.Value;
                                });
                                rfidProxy.Close();
                            }
                        };
                        rfidProxy.Open(CacheHelper.RFIDCom);
                    }
                }
                else
                {
                    commit_ex.ReportTime = TimeHelper.ToUnixTime(DateTime.Now);
                    var result1 = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyExamination(commit_ex));
                    if (result1.IsSuccess)
                    {
                        commit_ex.CopyTo(appointment.Examination);
                        commit.AppointmentStatus = AppointmentStatus.Checked;
                        appointment.Examination.Appointment = appointment;
                        var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(commit));
                        if (!result.IsSuccess)
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{ result.Error }");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
                        else
                        {
                            if (commit_ex.EndoscopeID != 0)
                            {
                                var decontaminateTask = new DecontaminateTask
                                {
                                    DoctorUserID = CacheHelper.CurrentUser.UserID,
                                    EndoscopeID = commit_ex.EndoscopeID,
                                    PatientName = appointment.Name,
                                    PatientID = appointment.IDCard,
                                    PatientSI = appointment.SocialSecurityCode
                                };
                                var result2 = loading.AsyncWait("创建清洗任务中,请稍后", SocketProxy.Instance.AddDecontaminateTask(decontaminateTask));
                                if (result2.IsSuccess)
                                    Alert.ShowMessage(true, AlertType.Success, "检查已结束,报告已保存并生成清洗任务");
                            }
                            else Alert.ShowMessage(true, AlertType.Success, "检查已结束,报告已保存");
                            commit.CopyTo(appointment);
                            CollectionView.Refresh();
                            epv.video.Dispose();
                        }
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{ result1.Error }");
                        tb.IsChecked = !tb.IsChecked.Value;
                    }
                }
            }
        }

        private void AppointmentStatus_CheckChanged(object sender, RoutedEventArgs e)
        {
            LoadAppointments();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAppointments();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment info)
            {
                var view = new ReportPreviewView(info.AppointmentID);
                MsWindow.ShowDialog(view, "打印预览");
            }
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                if (!tb.IsChecked.Value && epv.SelectedExamination != null)
                {
                    var result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyExamination(epv.SelectedExamination));
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Success, "保存检查信息成功");
                        epv.video.Dispose();
                    }
                    else Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败,{ result.Error }");
                }
            }
        }

        private void Appointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment appointment)
            {
                Examination examination = null;
                if (appointment.AppointmentStatus != AppointmentStatus.Waiting)
                {
                    var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(appointment.AppointmentID));
                    if (result.IsSuccess) examination = result.Content;
                    else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
                }
                else examination = new Examination();
                if (examination.Images == null)
                    examination.Images = new ObservableCollection<ExaminationMedia>();
                if (examination.Videos == null)
                    examination.Videos = new ObservableCollection<ExaminationMedia>();
                appointment.Examination = examination;
                appointment.Examination.Appointment = appointment;
            }
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CaptureSetting_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
