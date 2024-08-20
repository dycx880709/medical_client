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

        public Examination SelectedExamination
        {
            get { return (Examination)GetValue(SelectedExaminationProperty); }
            set { SetValue(SelectedExaminationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedExamination.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedExaminationProperty =
            DependencyProperty.Register("SelectedExamination", typeof(Examination), typeof(ExaminationManageView), new PropertyMetadata(null));

        private bool isInited = false;
        private RFIDProxy rfidProxy;
        private int consultingRoomId;
        private Action<EPCInfo> rfidNotifyAction;
        private ExaminationVideoView examView;

        public ExaminationManageView()
        {
            InitializeComponent();
            //dg_appointments.ItemsSource = this.Appointments;
            lb_appointments.ItemsSource = this.Appointments;
            CollectionView.SortDescriptions.Add(new SortDescription("AppointmentStatus", ListSortDirection.Ascending));
            CollectionView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Descending));
            CollectionView.Filter = t => t is Appointment appointment && 
            (appointment.AppointmentStatus != AppointmentStatus.Cross || 
            appointment.AppointmentStatus != AppointmentStatus.Cancel || 
            appointment.AppointmentStatus != AppointmentStatus.Cross || 
            appointment.AppointmentStatus != AppointmentStatus.Exprire);
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
                //SocketProxy.Instance.TcpProxy.ConnectStateChanged += TcpProxy_ConnectStateChanged;
                this.isInited = true;
            }
            StartRFIDProxy();
        }

        private async void TcpProxy_ConnectStateChanged(object sender, ConnectStateArgs e)
        {
            if (e.ConnectState == ConnectState.Success)
            {
                await SocketProxy.Instance.LoginConsultingRoom(CacheHelper.ConsultingRoomName);
            }
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
                    var examination = condition.Examinations.FirstOrDefault(t => t.ExaminationState == ExaminationState.Running);
                    if (examination != null)
                    {
                        this.IsEditable = true;
                        loading.Stop();
                        this.SelectedExamination = examination;
                        if (SelectedExamination.Videos == null)
                            SelectedExamination.Videos = new ObservableCollection<ExaminationMedia>();
                        if (SelectedExamination.Images == null)
                            SelectedExamination.Images = new ObservableCollection<ExaminationMedia>();
                        examView.SetExam(examination);
                    }
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
                                appointment.Examinations.Where(t => t.AppointmentID == appointment.AppointmentID).ForEach(t => t.Appointment = appointment);
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
                var result = loading.AsyncWait("获取检查预约信息中,请稍后", SocketProxy.Instance.GetTodayAppointmentRecords(CacheHelper.ConsultingRoomName));
                if (result.IsSuccess)
                {
                    Appointments.Clear();
                    foreach (var appointment in result.Content)
                    {
                        foreach (var examination in appointment.Examinations)
                        {
                            if (examination.AppointmentID == appointment.AppointmentID)
                                examination.Appointment = appointment;
                        }
                    }
                    Appointments.AddRange(result.Content);
                    CollectionView.Refresh();
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取检查预约信息失败,{ result.Error }");
                }
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && SelectedExamination != null)
            {
                if (tb.IsChecked.Value)
                {
                    void AddExamination(Endoscope endoscope)
                    {
                        var result1 = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.StartExamination(SelectedExamination.ExaminationID, CacheHelper.CurrentUser.UserID, endoscope.EndoscopeID));
                        if (result1.IsSuccess)
                        {
                            SelectedExamination.DoctorName = result1.Content.DoctorName;
                            SelectedExamination.DoctorID = result1.Content.DoctorID;
                            SelectedExamination.EndoscopeID = result1.Content.EndoscopeID;
                            SelectedExamination.ExaminationTime = result1.Content.ExaminationTime;
                            SelectedExamination.ExaminationState = ExaminationState.Running;
                            SelectedExamination.Endoscope = endoscope;
                            if (SelectedExamination.Videos == null)
                                SelectedExamination.Videos = new ObservableCollection<ExaminationMedia>();
                            if (SelectedExamination.Images == null)
                                SelectedExamination.Images = new ObservableCollection<ExaminationMedia>();
                            var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(SelectedExamination.AppointmentID, AppointmentStatus.Checking));
                            if (!result.IsSuccess)
                            {
                                Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{result.Error}");
                                tb.IsChecked = !tb.IsChecked.Value;
                            }
                            else
                            {
                                SelectedExamination.Appointment.AppointmentStatus = AppointmentStatus.Checking;
                                Alert.ShowMessage(true, AlertType.Success, "检查已启动");
                                if (!string.IsNullOrEmpty(SelectedExamination.Appointment.WatchInfo))
                                {
                                    SpeechHelper.Add($"患者{SelectedExamination.Appointment.Name}存在关注事项,{SelectedExamination.Appointment.WatchInfo}");
                                }
                                //CollectionView.Refresh();
                                examView.SetExam(SelectedExamination);
                            }
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{result1.Error}");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
                    }
                    if (CacheHelper.IsDebug)
                    {
                        var result = loading.AsyncWait("读取内窥镜中,请稍后", SocketProxy.Instance.GetEndoscopeById(1));
                        if (result.IsSuccess)
                            AddExamination(result.Content);
                        else
                        { 
                            Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{result.Error}");
                            tb.IsChecked = !tb.IsChecked.Value;
                        }
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
                                    Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{result.Error}");
                                    tb.IsChecked = !tb.IsChecked.Value;
                                }
                                loading.Stop();
                            });
                        };
                    }
                }
                else
                {
                    if (SelectedExamination.Videos != null && SelectedExamination.Videos.Any(t => !string.IsNullOrEmpty(t.LocalVideoPath)))
                    {
                        Alert.ShowMessage(true, AlertType.Warning, $"结束检查失败,存在未结束的录像任务");
                        tb.IsChecked = !tb.IsChecked.Value;
                    }
                    else
                    {
                        var commit = new Examination();
                        SelectedExamination.CopyTo(commit);
                        commit.Appointment = null;
                        var result1 = loading.AsyncWait("结束检查中,请稍后1", SocketProxy.Instance.ModifyExamination(commit));
                        if (result1.IsSuccess)
                        {
                            var result3 = loading.AsyncWait("结束检查中,请稍后2", SocketProxy.Instance.StopExamination(SelectedExamination.ExaminationID));
                            if (result3.IsSuccess)
                            {
                                SelectedExamination.ReportTime = result3.Content;
                                SelectedExamination.ExaminationState = ExaminationState.Complete;
                                var decontaminateTask = new DecontaminateTask
                                {
                                    EndoscopeID = SelectedExamination.EndoscopeID,
                                    ExaminationID = SelectedExamination.ExaminationID,
                                    DoctorUserID = SelectedExamination.DoctorID,
                                };
                                var result2 = loading.AsyncWait("创建清洗任务中,请稍后", SocketProxy.Instance.AddDecontaminateTask(decontaminateTask));
                                if (!result2.IsSuccess) Alert.ShowMessage(true, AlertType.Warning, $"检查已结束,报告已保存但生成清洗任务失败,{result2.Error}");
                                else Alert.ShowMessage(true, AlertType.Success, $"检查已结束,报告已保存并生成清洗任务");
                                if (SelectedExamination.Appointment.Examinations.All(t => t.ExaminationTime != 0))
                                {
                                    var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(SelectedExamination.AppointmentID, AppointmentStatus.Checked));
                                    if (!result.IsSuccess)
                                    {
                                        Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{result.Error}");
                                        tb.IsChecked = !tb.IsChecked.Value;
                                    }
                                    else
                                    {
                                        SelectedExamination.Appointment.AppointmentStatus = AppointmentStatus.Checked;
                                        CollectionView.Refresh();
                                    }
                                }
                                examView.SetExam(null);
                            }
                            else
                            {
                                Alert.ShowMessage(true, AlertType.Error, $"结束检查失败2,{result3.Error}");
                                tb.IsChecked = !tb.IsChecked.Value;
                            }
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"结束检查失败1,{result1.Error}");
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
            if (this.SelectedExamination != null)
            {
                var view = new ReportPreviewView(SelectedExamination.AppointmentID);
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

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamination != null)
            {
                SpeechHelper.Add($"请D{ SelectedExamination.Appointment.Number.ToString("D3") }患者{SelectedExamination.Appointment.Name}来{ CacheHelper.ConsultingRoomName }前来检查");
            }
        }
        private async void InsteadEndoscope_Click(object sender, RoutedEventArgs e)
        {
            if (MsPrompt.ShowDialog("确定替换当前内镜?"))
            {
                int endoscopeID = 0;
                if (CacheHelper.IsDebug)
                {
                    var result = loading.AsyncWait("读取内窥镜中,请稍后", SocketProxy.Instance.GetEndoscopeById(1));
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!result.IsSuccess)
                            Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{result.Error}");
                        else 
                            endoscopeID = result.Content.EndoscopeID;
                    });
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
                                        endoscopeID = result.Content.EndoscopeID;
                                        break;
                                    case EndoscopeState.Decontaminating:
                                        Alert.ShowMessage(true, AlertType.Error, "内窥镜正在清洗中,请勿重复使用");
                                        break;
                                    case EndoscopeState.Disabled:
                                        Alert.ShowMessage(true, AlertType.Error, "内窥镜已禁止使用");
                                        break;
                                }
                            }
                            else
                            {
                                Alert.ShowMessage(true, AlertType.Error, $"读取内窥镜信息失败,{result.Error}");
                            }
                            loading.Stop();
                        });
                    };
                }
                if (endoscopeID != 0)
                {
                    var result = await SocketProxy.Instance.ChangeEndoscope(SelectedExamination.ExaminationID, endoscopeID);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (result.IsSuccess)
                        {
                            Alert.ShowMessage(true, AlertType.Success, $"更换内窥镜成功");
                            SelectedExamination.EndoscopeID = endoscopeID;
                        }
                        else
                            Alert.ShowMessage(true, AlertType.Error, $"更换内窥镜失败,{result.Error}");
                    });
                }
            }
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

        private void Skip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Examination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedExamination != null)
            {
                if (SelectedExamination.Appointment == null)
                {
                    var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetAppointment(SelectedExamination.AppointmentID));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败:{result.Error}");
                        return;
                    }
                    SelectedExamination.Appointment = result.Content;
                }
            }
        }
    }
}
