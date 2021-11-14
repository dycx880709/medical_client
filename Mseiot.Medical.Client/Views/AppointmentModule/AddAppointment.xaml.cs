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
                Appointment.AppointmentTime = TimeHelper.ToUnixTime(DateTime.Now);
            DataContext = this;
            this.Loaded += AddAppointment_Loaded;
        }

        private void AddAppointment_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= AddAppointment_Loaded;
            GetAppointTypes();
        }

        private void GetAppointTypes()
        {
            var result = loading.AsyncWait("获取预约类型中,请稍后", SocketProxy.Instance.GetBaseWords("检查类型"));
            if (result.IsSuccess)
                cb_type.ItemsSource = result.SplitContent("检查类型");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Appointment.AppointmentID == 0)
            {
                var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddAppointment(Appointment));
                if (result.IsSuccess) this.Close(true);
                else Alert.ShowMessage(false, AlertType.Error, "新增预约失败", result.Error);
            }
            else
            {
                var result = loading.AsyncWait("修改预约中,请稍后", SocketProxy.Instance.ModifyAppointment(Appointment));
                if (result.IsSuccess) this.Close(true);
                else Alert.ShowMessage(false, AlertType.Error, "修改预约失败", result.Error);
            }
        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
