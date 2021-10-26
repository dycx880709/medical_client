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

namespace MM.Medical.Client.Views.AppointmentModule
{
    /// <summary>
    /// AddBookingView.xaml 的交互逻辑
    /// </summary>
    public partial class AddAppointment : UserControl
    {
        public Appointment Appointment { get; set; } = new Appointment();
        Loading loading;

        public AddAppointment(Appointment rawAppointment, Loading loading)
        {
            InitializeComponent();
            this.loading = loading;
            Appointment = rawAppointment.Copy();
            LoadTimes();
            cbHours.SelectedIndex = 0;
            cbMinutes.SelectedIndex = 0;
            if (rawAppointment.AppointmentID == 0)
            {
                Appointment.AppointmentTime = (int)TimeHelper.ToUnixTime(DateTime.Now.Date);
            }
            else
            {
                DateTime time = TimeHelper.FromUnixTime(Appointment.AppointmentTime);
            }
            DataContext = this;
        }

        private void LoadTimes()
        {
            List<int> hours = new List<int>();
            for(int i = 9; i < 17; i++)
            {
                hours.Add(i);
            }
            cbHours.ItemsSource = hours;

            List<int> minutes = new List<int>() { 0, 30 };
            cbMinutes.ItemsSource = minutes;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Appointment.AppointmentTime = TimeHelper.ToUnixTime(((DateTime)dpAppointmentTime.SelectedDate).Date.AddHours((int)cbHours.SelectedItem).AddMinutes((int)cbMinutes.SelectedItem));
            if (Appointment.AppointmentID == 0)
            {
                var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddAppointment(Appointment));
                if (result.IsSuccess)
                {
                    this.Close(true);
                }
                else
                {
                    Alert.ShowMessage(false, AlertType.Error, "新增预约失败", result.Error);
                }
            }
            else
            {
                var result = loading.AsyncWait("编辑预约中,请稍后", SocketProxy.Instance.ModifyAppointment(Appointment));
                if (result.IsSuccess)
                {
                    this.Close(true);
                }
                else
                {
                    Alert.ShowMessage(false, AlertType.Error, "编辑预约失败", result.Error);
                }
            }
        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
