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
        public static readonly DependencyProperty IsCheckingProperty = DependencyProperty.Register("IsChecking", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsDoctorVisitProperty = DependencyProperty.Register("IsDoctorVisit", typeof(bool), typeof(ExaminationManageView), new PropertyMetadata(false));
        public ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();
        private ICollectionView collectionView { get { return CollectionViewSource.GetDefaultView(Appointments); }}
        public IEnumerable<MedicalTemplate> MedicalTemplates { get; set; }
        public IEnumerable<MedicalWord> OriginMedicalWords { get; set; }
        public IEnumerable<MedicalWord> MedicalWords { get; set; }

        private int consultingRoomId;

        public ExaminationManageView()
        {
            InitializeComponent();
            this.Loaded += ExaminationManageView_Loaded;
            dg_appointments.ItemsSource = this.Appointments;
            collectionView.SortDescriptions.Add(new SortDescription("AppointmentStatus", ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            collectionView.Filter = t => t is Appointment appointment && (appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Cancel || appointment.AppointmentStatus != AppointmentStatus.Cross || appointment.AppointmentStatus != AppointmentStatus.Exprire);
            this.IsEnabled = false;
        }

        private void ExaminationManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
            GetMedicalDatas();
            LoadConsultingRoom();
            ResetCheckingExamination();
            SocketProxy.Instance.TcpProxy.ReceiveMessaged += TcpProxy_ReceiveMessaged;
        }

        private void GetMedicalDatas()
        {
            var result1 = loading.AsyncWait("获取诊断模板中,请稍后", SocketProxy.Instance.GetMedicalTemplates());
            if (result1.IsSuccess) tv_template.ItemsSource = CacheHelper.SortMedicalTemplates(result1.Content, 0);
            else Alert.ShowMessage(false, AlertType.Error, $"获取诊断模板失败,{ result1.Error }");
            var result2 = loading.AsyncWait("获取医学词库中,请稍后", SocketProxy.Instance.GetMedicalWords());
            if (result2.IsSuccess)
            { 
                this.OriginMedicalWords = result2.Content;
                this.MedicalWords = CacheHelper.SortMedicalWords(this.OriginMedicalWords, 0);
            }
            else Alert.ShowMessage(false, AlertType.Error, $"获取医学词库失败,{ result2.Error }");
        }

        private async void ResetCheckingExamination()
        {
            var condition = Appointments.FirstOrDefault(t => t.AppointmentStatus == AppointmentStatus.Checking);
            if (condition != null)
            {
                loading.Start("检查恢复中,请稍后");
                await Task.Delay(2000);
                this.Dispatcher.Invoke(() =>
                {
                    dg_appointments.SelectedValue = condition;
                    this.IsChecking = true;
                    loading.Stop();
                });
            }
        }

        private void LoadConsultingRoom()
        {
            if (!string.IsNullOrEmpty(CacheHelper.ConsultingRoomName))
            {
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.LoginConsultingRoom(CacheHelper.ConsultingRoomName));
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
                    collectionView.Refresh();
                    //pager.TotalCount = result.Content.Total;
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
                    var result = loading.AsyncWait("启动检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(info));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"启动检查失败,{ result.Error }");
                        info.AppointmentStatus = oldAppointmentStatus;
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Success, "检查已启动");
                        collectionView.Refresh();
                    }
                }
                else
                {
                    info.AppointmentStatus = AppointmentStatus.Checked;
                    var result = loading.AsyncWait("结束检查中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(info));
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"结束检查失败,{ result.Error }");
                        info.AppointmentStatus = oldAppointmentStatus;
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Success, "检查已结束");
                        collectionView.Refresh();
                    }
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
            if (sender is FrameworkElement element)
            {
                var expander = ControlHelper.GetParentObject<Expander>(element);
                var title = expander.Header.ToString();
                var medicalWord = MedicalWords.FirstOrDefault(t => t.Name.Equals(title));
                if (title.Equals("内镜所见") || title.Equals("镜下诊断"))
                    ti_template.IsSelected = true;
                else
                {
                    if (medicalWord == null) tv_medicalWord.ItemsSource = null;
                    else tv_medicalWord.ItemsSource = medicalWord.MedicalWords;
                    ti_word.IsSelected = true;
                }
            }
        }

        private void MedicalWord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element && dg_appointments.SelectedValue is Appointment appointment && appointment.Examination != null)
            {
                if (element.DataContext is MedicalWord word && (word.MedicalWords == null || word.MedicalWords.Count == 0))
                {
                    var rootMedicalWord = CacheHelper.GetMedicalWordParent(this.OriginMedicalWords, word.MedicalWordID, 0);
                    var examination = appointment.Examination;
                    switch (rootMedicalWord.Name)
                    {
                        case "临床诊断":
                            examination.ClinicalDiagnosis = word.Name;
                            break;
                        case "内镜所见":
                            examination.EndoscopicFindings = word.Name;
                            break;
                        case "镜下诊断":
                            examination.MicroscopicDiagnosis = word.Name;
                            break;
                        case "活检部位":
                            examination.BiopsySite = word.Name;
                            break;
                        case "病理诊断":
                            examination.PathologicalDiagnosis = word.Name;
                            break;
                        case "医生建议":
                            examination.DoctorAdvice = word.Name;
                            break;
                    }
                }
            }
        }

        private void MedicalTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element && dg_appointments.SelectedValue is Appointment appointment && appointment.Examination != null)
            {
                if (element.DataContext is MedicalTemplate template && (!string.IsNullOrEmpty(template.Moddia) || !string.IsNullOrEmpty(template.Modsee)))
                {
                    var examination = appointment.Examination;
                    if (string.IsNullOrEmpty(examination.EndoscopicFindings))
                        examination.EndoscopicFindings = template.Modsee;
                    else if (!examination.EndoscopicFindings.Contains(template.Modsee))
                        examination.EndoscopicFindings += "\n" + template.Modsee;
                    if (string.IsNullOrEmpty(examination.MicroscopicDiagnosis))
                        examination.MicroscopicDiagnosis = template.Moddia;
                    else if (examination.MicroscopicDiagnosis.Contains(template.Moddia))
                        examination.MicroscopicDiagnosis += "\n" + template.Moddia;
                }
            }
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
            if (sender is ToggleButton tb && !tb.IsChecked.Value && dg_appointments.SelectedValue is Appointment appointment && appointment.Examination != null)
            {
                var result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyExamination(appointment.Examination));
                if (result.IsSuccess) Alert.ShowMessage(true, AlertType.Success, "保存检查信息成功");
                else Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败,{ result.Error }");
            }
        }

        private void Appointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_appointments.SelectedValue is Appointment appointment)
            {
                if (appointment.AppointmentStatus != AppointmentStatus.Waiting)
                {
                    var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(appointment.AppointmentID));
                    if (result.IsSuccess) appointment.Examination = result.Content;
                    else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
                }
                else appointment.Examination = new Examination();
            }
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
