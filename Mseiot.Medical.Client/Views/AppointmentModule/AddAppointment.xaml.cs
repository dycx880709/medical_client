using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Controls.Core;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// AddBookingView.xaml 的交互逻辑
    /// </summary>
    public partial class AddAppointment : UserControl
    {
        public Appointment Appointment { get; set; } = new Appointment();
        private readonly Loading loading;

        public AddAppointment(Appointment rawAppointment, Loading loading)
        {
            InitializeComponent();
            this.loading = loading;
            Appointment = rawAppointment.Copy();
            if (rawAppointment.AppointmentID == 0)
            {
                var dateTime = DateTime.Now.AddHours(1);
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                Appointment.AppointmentTime = TimeHelper.ToUnixTime(dateTime);
            }
            this.Loaded += AddAppointment_Loaded;
        }

        private void AddAppointment_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= AddAppointment_Loaded;
            GetBaseWords();
            DataContext = this.Appointment;
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取预约类型中,请稍后", SocketProxy.Instance.GetBaseWords(
                "检查类型",
                "麻醉方法"
            ));
            if (result.IsSuccess)
            { 
                var checkTypes = result.SplitContent("检查类型");
                cb_type.ItemsSource = checkTypes;
                var anesthesias = result.SplitContent("麻醉方法");
                cb_anesthesia.ItemsSource = anesthesias;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Appointment.Name))
            {
                Alert.ShowMessage(true, AlertType.Error, "患者姓名不能为空");
                return;
            }
            if (string.IsNullOrEmpty(Appointment.AppointmentTypeStr))
            {
                Alert.ShowMessage(true, AlertType.Error, "检查类型不能为空");
                return;
            }
            //if (Appointment.Birthday == 0 && tb_birthday.Text != "0")
            //{
            //    Alert.ShowMessage(true, AlertType.Error, "患者年龄设置在0-150岁之间");
            //    return;
            //}
            if (!string.IsNullOrEmpty(tb_id.Text) && tb_id.Text.Length != 18)
            {
                Alert.ShowMessage(true, AlertType.Error, "患者身份证长度输入不为18位");
                return;
            }
            if (Appointment.AppointmentTime < TimeHelper.ToUnixTime(DateTime.Now))
            {
                Alert.ShowMessage(true, AlertType.Error, "预约时间输入不能早于当前时间");
                return;
            }
            if (string.IsNullOrEmpty(Appointment.Telephone))
            {
                Alert.ShowMessage(true, AlertType.Error, "患者电话号码长度输入不为11位");
                return;
            }
            if (string.IsNullOrEmpty(Appointment.Anesthesia))
            {
                Alert.ShowMessage(true, AlertType.Error, "患者麻醉方式不能为空");
                return;
            }
            if (cb_source.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(Appointment.HospitalID))
                {
                    Alert.ShowMessage(true, AlertType.Error, "住院号不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Appointment.Department))
                {
                    Alert.ShowMessage(true, AlertType.Error, "科室不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Appointment.RoomID))
                {
                    Alert.ShowMessage(true, AlertType.Error, "床号不能为空");
                    return;
                }
            }
            if (Appointment.AppointmentID == 0)
            {
                var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddAppointment(Appointment));
                if (result.IsSuccess) this.Close(true);
                else Alert.ShowMessage(true, AlertType.Error, "新增预约失败", result.Error);
            }
            else
            {
                var result = loading.AsyncWait("修改预约中,请稍后", SocketProxy.Instance.ModifyAppointment(Appointment));
                if (result.IsSuccess) this.Close(true);
                else Alert.ShowMessage(true, AlertType.Error, "修改预约失败", result.Error);
            }
        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PatientNumber_Changed(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Appointment.PatientNumber))
            {
                var result = await SocketProxy.Instance.GetPatients(Appointment.PatientNumber);
                if (result.IsSuccess)
                {
                    var list = result.Content.Select(t => new CustomTextBoxComplete { Label = t.PatientNumber, Value = t }).ToList();
                    patient_tb.ItemsSource = list;
                    patient_tb.UpdateLayout();
                    patient_tb.IsPopupOpen(list.Count > 0 && Appointment.PatientNumber.Length > 1);
                }
            }
        }

        private void Patient_Selected(object sender, Ms.Controls.Models.CustomEventArgs e)
        {
            if (e.PropertyValue is Patient patient)
            {
                patient.CopyTo(this.Appointment);
            }
        }

        private void Type_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox cb_type && cb_type.DataContext is ConsultingRoom room)
            {
                var selectedTypes = new List<string>();
                if (!string.IsNullOrEmpty(room.ExaminationTypes))
                {
                    selectedTypes = room.ExaminationTypes.Split(',').ToList();
                }
                void UpdateSelectedType()
                {
                    for (int i = 0; i < cb_type.Items.Count; i++)
                    {
                        var cbi = cb_type.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                        if (cbi != null)
                        {
                            var cb = ControlHelper.GetVisualChild<CheckBox>(cbi);
                            cb.IsChecked = selectedTypes.Any(t => t.Equals(cb_type.Items[i].ToString()));
                        }
                    }
                }
                if (cb_type.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    UpdateSelectedType();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        while (cb_type.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                        {
                            await Task.Delay(100);
                        }
                        this.Dispatcher.Invoke(() => UpdateSelectedType());
                    });
                }
            }
        }

        private void SelectedType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var item = ControlHelper.GetParentObject<ComboBoxItem>(element);
                var cb_type = ItemsControl.ItemsControlFromItemContainer(item) as ComboBox;
                cb_type.Text = string.Empty;
                for (int i = 0; i < cb_type.Items.Count; i++)
                {
                    var cbi = cb_type.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                    var cb = ControlHelper.GetVisualChild<CheckBox>(cbi);
                    if (cb.IsChecked.Value)
                    {
                        cb_type.Text += cb.Content.ToString() + ",";
                    }
                }
                if (!string.IsNullOrEmpty(cb_type.Text))
                {
                    cb_type.Text = cb_type.Text.Substring(0, cb_type.Text.Length - 1);
                }
            }
        }
    }
}
