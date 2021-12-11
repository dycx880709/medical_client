using MM.Medical.Client.Core;
using Ms.Controls;
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
                Alert.ShowMessage(true, AlertType.Error, "患者姓名输入不合法");
                return;
            }
            if (string.IsNullOrEmpty(Appointment.AppointmentType))
            {
                Alert.ShowMessage(true, AlertType.Error, "检查类型不能为空");
                return;
            }
            if (Appointment.Birthday == 0 && tb_birthday.Text != "0")
            {
                Alert.ShowMessage(true, AlertType.Error, "患者年龄输入不合法");
                return;
            }
            if (!string.IsNullOrEmpty(tb_id.Text) && tb_id.Text.Length != 18)
            {
                Alert.ShowMessage(true, AlertType.Error, "患者身份证长度输入不合法");
                return;
            }
            if (Appointment.AppointmentTime < TimeHelper.ToUnixTime(DateTime.Now))
            {
                Alert.ShowMessage(true, AlertType.Error, "预约时间输入不能早于当前时间");
                return;
            }
            if (string.IsNullOrEmpty(Appointment.Telephone))
            {
                Alert.ShowMessage(true, AlertType.Error, "患者电话号码输入不合法");
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
    }
}
