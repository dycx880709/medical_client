﻿using Ms.Controls;
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
using Ms.Libs;

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
        public static readonly DependencyProperty SelectedMediaProperty = 
            DependencyProperty.Register("SelectedMedia", typeof(ExaminationMedia), typeof(ExaminationManageView), new PropertyMetadata(null));
       
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty = 
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        
        public bool IsDoctorVisit
        {
            get { return (bool)GetValue(IsDoctorVisitProperty); }
            set { SetValue(IsDoctorVisitProperty, value); }
        }
        public static readonly DependencyProperty IsFullScreenProperty = 
            DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));

        public bool IsFullScreen
        {
            get { return (bool)GetValue(IsFullScreenProperty); }
            set { SetValue(IsFullScreenProperty, value); }
        }
        public static readonly DependencyProperty IsDoctorVisitProperty = 
            DependencyProperty.Register("IsDoctorVisit", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
       
        public ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();
        private ICollectionView CollectionView { get { return CollectionViewSource.GetDefaultView(Appointments); }}
        public IEnumerable<MedicalTemplate> MedicalTemplates { get; set; }
        public IEnumerable<MedicalWord> OriginMedicalWords { get; set; }
        public IEnumerable<MedicalWord> MedicalWords { get; set; }
        public List<string> BodyParts { get; set; }

        private bool isInited = false;
        private RFIDProxy rfidProxy;
        private int consultingRoomId;
        private Action<EPCInfo> rfidNotifyAction;
        private ExaminationVideoView examView;

        public ExaminationManageView()
        {
            InitializeComponent();
            dg_appointments.ItemsSource = this.Appointments;
            CollectionView.SortDescriptions.Add(new SortDescription("AppointmentStatus", ListSortDirection.Ascending));
            CollectionView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            CollectionView.Filter = t => t is Appointment appointment && (appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Cancel || appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Exprire);
            this.IsEnabled = false;
            this.Loaded += ExaminationManageView_Loaded;
            this.Unloaded += ExaminationManageView_Unloaded;
            this.examView = new ExaminationVideoView();
            examView.Show();
        }

        private void ExaminationManageView_Unloaded(object sender, RoutedEventArgs e)
        {
            StopRFIDProxy();
        }

        private void ExaminationManageView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.isInited)
            {
                LoadConsultingRoom();
                ResetChecking();
                SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
                this.isInited = true;
            }
            StartRFIDProxy();
        }

        private void StopRFIDProxy()
        {
            if (this.rfidProxy != null)
            {
                this.rfidNotifyAction = null;
                rfidProxy.Close();
                rfidProxy = null;
            }
        }

        private void StartRFIDProxy()
        {
            if (!CacheHelper.IsDebug)
            {
                if (!string.IsNullOrEmpty(CacheHelper.RFIDCom))
                {
                    loading.Start("读取内窥镜中,请稍后");
                    this.rfidProxy = new RFIDProxy();
                    rfidProxy.NotifyDeviceStatusChanged += (_, status) =>
                    {
                        if (!status)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                loading.Stop();
                                Alert.ShowMessage(true, AlertType.Error, "读取内窥镜失败,开启检查失败");
                            });
                            rfidProxy.Close();
                        }
                        this.Dispatcher.Invoke(() => loading.Stop());
                    };
                    rfidProxy.NotifyEPCReceived += (_, e) =>
                    {
                        if (this.rfidNotifyAction != null)
                        {
                            rfidNotifyAction.Invoke(e);
                        }
                    };
                    rfidProxy.Open(CacheHelper.RFIDCom);
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, "设备读卡器未配置");
                    this.IsEnabled = false;
                }
            }
        }

        private async void ResetChecking()
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
                    examView.SetExam(condition.Examination);
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
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取检查诊室信息失败,{ result.Error }");
                    tb_check.IsEnabled = false;
                }
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"检查诊室未配置,模块不可用");
                tb_check.IsEnabled = false;
            }
        }

        private void TcpProxy_ReceiveMessaged(object sender, Message e)
        {
            if (e.Module == Command.Module_Appointment)
            {
                if (e.Method == Command.Change_Appointment)
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
                            else
                            {
                                condition.AppointmentStatus = appointment.AppointmentStatus;
                            }
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
                if (!rb_waiting.IsChecked.Value && !rb_checked.IsChecked.Value)
                {
                    Appointments.Clear();
                    return;
                }
                var appointmentStatuses = new List<AppointmentStatus>();
                if (rb_waiting.IsChecked.Value)
                    appointmentStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Waiting, AppointmentStatus.Checking });
                if (rb_checked.IsChecked.Value)
                    appointmentStatuses.AddRange(new AppointmentStatus[] { AppointmentStatus.Checked, AppointmentStatus.Reported });
                var startTime = TimeHelper.ToUnixDate(DateTime.Now);
                var endTime = TimeHelper.ToUnixDate(DateTime.Now.AddDays(1)) - 1;
                //pager.SelectedCount = dg_appointments.GetFullCountWithoutScroll();
                var result = loading.AsyncWait("获取检查预约信息中,请稍后", SocketProxy.Instance.GetAppointments(
                    0,      //pager.PageIndex
                    1000,   //pager.SelectedCount
                    TimeHelper.FromUnixTime(startTime),
                    TimeHelper.FromUnixTime(endTime),
                    userInfo: "",
                    consultingRoomName: CacheHelper.ConsultingRoomName,
                    appointmentStatuses: appointmentStatuses.ToArray()));
                if (result.IsSuccess)
                {
                    Appointments.Clear();
                    Appointments.AddRange(result.Content.Results);
                    CollectionView.Refresh();
                    //pager.TotalCount = result.Content.Total;
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取检查预约信息失败,{ result.Error }");
                }
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
                    void AddExamination(Endoscope endoscope)
                    {
                        commit_ex.DoctorName = CacheHelper.CurrentUser.Name;
                        commit_ex.DoctorID = CacheHelper.CurrentUser.UserID;
                        commit_ex.Endoscope = endoscope;
                        commit_ex.EndoscopeID = endoscope.EndoscopeID;
                        commit_ex.AppointmentID = appointment.AppointmentID;
                        commit_ex.ExaminationTime = TimeHelper.ToUnixTime(DateTime.Now);
                        var result1 = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.AddExamination(commit_ex));
                        if (result1.IsSuccess)
                        {
                            commit_ex.ExaminationID = result1.Content;
                            commit_ex.CopyTo(appointment.Examination);
                            if (appointment.Examination.Videos == null)
                                appointment.Examination.Videos = new ObservableCollection<ExaminationMedia>();
                            if (appointment.Examination.Images == null)
                                appointment.Examination.Images = new ObservableCollection<ExaminationMedia>();
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
                                if (!string.IsNullOrEmpty(appointment.WatchInfo))
                                {
                                    SpeechHelper.Add($"患者{appointment.Name}存在关注事项,{appointment.WatchInfo}");
                                }
                                commit.CopyTo(appointment);
                                CollectionView.Refresh();
                                examView.SetExam(appointment.Examination);
                            }
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result1.Error }");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
                    }
                    if (CacheHelper.IsDebug)
                    {
                        var result = loading.AsyncWait("读取内窥镜中,请稍后", SocketProxy.Instance.GetEndoscopeById(1));
                        if (result.IsSuccess)
                            AddExamination(result.Content);
                        else
                            Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{ result.Error }");
                    }
                    else
                    {
                        loading.Start("读取内窥镜信息中,请稍后");
                        this.rfidNotifyAction = async device =>
                        {
                            Console.WriteLine($"读取内窥镜信息,{device.DeviceID} {device.EPC}");
                            this.rfidNotifyAction = null;
                            var result = await SocketProxy.Instance.GetEndoscopeById(device.EPC);
                            this.Dispatcher.Invoke(() =>
                            {
                                if (result.IsSuccess)
                                {
                                    switch (result.Content.State)
                                    {
                                        case EndoscopeState.Waiting:
                                        case EndoscopeState.Using:
                                            AddExamination(result.Content);
                                            break;
                                        case EndoscopeState.Decontaminating:
                                            Alert.ShowMessage(true, AlertType.Error, "内窥镜正在清洗中,请勿重复使用");
                                            tb.IsChecked = !tb.IsChecked.Value;
                                            break;
                                        case EndoscopeState.Disabled:
                                            Alert.ShowMessage(true, AlertType.Error, "内窥镜已禁止使用");
                                            tb.IsChecked = !tb.IsChecked.Value;
                                            break;
                                    }
                                }
                                else
                                {
                                    Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{ result.Error }");
                                    tb.IsChecked = !tb.IsChecked.Value;
                                }
                                loading.Stop();
                            });
                        };
                    }
                }
                else
                {
                    if (appointment.Examination.Videos.Any(t => !string.IsNullOrEmpty(t.LocalVideoPath)))
                    {
                        Alert.ShowMessage(true, AlertType.Warning, $"结束检查失败,存在未结束的录像任务");
                        tb.IsChecked = !tb.IsChecked.Value;
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
                            var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyAppointment(commit));
                            if (!result.IsSuccess)
                            {
                                Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{result.Error}");
                                tb.IsChecked = !tb.IsChecked.Value;
                            }
                            else
                            {
                                // if (!CacheHelper.IsDebug)
                                {
                                    var decontaminateTask = new DecontaminateTask
                                    {
                                        EndoscopeID = commit_ex.EndoscopeID,
                                        ExaminationID = commit_ex.ExaminationID,
                                        DoctorUserID = commit_ex.DoctorID,
                                        //PatientName = commit_ex.Appointment.Name,
                                        //PatientSI = commit_ex.Appointment.IDCard,
                                        //StartExamineTime = commit_ex.ExaminationTime,
                                        //EndExamineTime = TimeHelper.ToUnixTime(DateTime.Now),
                                        //PatientBirthday = commit_ex.Appointment.Birthday,
                                    };
                                    var result2 = loading.AsyncWait("创建清洗任务中,请稍后", SocketProxy.Instance.AddDecontaminateTask(decontaminateTask));
                                    if (!result2.IsSuccess)
                                        Alert.ShowMessage(false, AlertType.Warning, $"检查已结束,报告已保存但生成清洗任务失败,{result2.Error}");
                                    else
                                        Alert.ShowMessage(true, AlertType.Success, $"检查已结束,报告已保存并生成清洗任务");
                                }
                                //   else 
                                //       Alert.ShowMessage(true, AlertType.Success, "检查已结束,报告已保存");
                                commit.CopyTo(appointment);
                                CollectionView.Refresh();
                                examView.SetExam(null);
                            }
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{result1.Error}");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
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
                MsWindow.ShowDialog(view, "打印预览", showInTaskbar: true, windowState: WindowState.Maximized);
            }
        }

        private void StartCheck_Click(object sender, RoutedEventArgs e)
        {
            AcceptConsultingRoom();
        }

        private void AcceptConsultingRoom()
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
                    if (epv.SelectedExamination.ReportTime == 0)
                        epv.SelectedExamination.ReportTime = TimeHelper.ToUnixTime(DateTime.Now);
                    var result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyExamination(epv.SelectedExamination));
                    if (result.IsSuccess)
                    {
                        result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyAppointment(epv.SelectedExamination.Appointment));
                        if (result.IsSuccess)
                        {
                            Alert.ShowMessage(true, AlertType.Success, "保存检查信息成功");
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败2,{ result.Error }");
                        }
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败1,{ result.Error }");
                    }
                }
            }
        }

        private void Appointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment appointment)
            {
                if (appointment.AppointmentStatus != AppointmentStatus.Waiting)
                {
                    if (appointment.Examination == null)
                    {
                        var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(appointment.AppointmentID));
                        if (result.IsSuccess)
                            appointment.Examination = result.Content;
                        else
                            Alert.ShowMessage(true, AlertType.Error, $"获取检查详细信息失败,{result.Error}");
                    }
                }
                if (appointment.Examination == null)
                {
                    appointment.Examination = new Examination();
                }
                if (appointment.Examination.Images == null)
                    appointment.Examination.Images = new ObservableCollection<ExaminationMedia>();
                if (appointment.Examination.Videos == null)
                    appointment.Examination.Videos = new ObservableCollection<ExaminationMedia>();
                appointment.Examination.Appointment = appointment;
            }
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment appointment)
            {
                SpeechHelper.Add($"请患者{appointment.Name}前来检查");
            }
        }
        private void CaptureSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ReConnect()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.IsDoctorVisit)
                    AcceptConsultingRoom();
                LoadConsultingRoom();
            });
        }
    }
}
