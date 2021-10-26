using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MM.Medical.Client.Views.AppointmentModule
{
    /// <summary>
    /// BookingManageView.xaml 的交互逻辑
    /// </summary>
    public partial class AppointmentManage : UserControl
    {
        public AppointmentManage()
        {
            InitializeComponent();
            this.Loaded += AppointmentManage_Loaded;
        }

        private void AppointmentManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= AppointmentManage_Loaded;
            this.dgAppointments.ItemsSource = appointments;
            dtpTime.SelectedDateTime = DateTime.Now;
            LoadAppointments();
        }

        #region 数据

        ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();

        private async void LoadAppointments()
        {
            appointments.Clear();
            var result = await SocketProxy.Instance.GetAppointments();
            if (result.IsSuccess)
            {
                if (result.Content != null)
                {
                    appointments.AddRange(result.Content);
                }
            }
            else
            {
                Alert.ShowMessage(false, AlertType.Error, result.Error);
            }
        }

        #endregion

        #region 搜索

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadAppointments();
        }

        #endregion

        #region 添加、删除、修改

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddAppointment(new Appointment(), this.loading);
            if (child.ShowDialog("预约登记", view))
            {
                LoadAppointments();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = (sender as FrameworkElement).Tag as Appointment;
            var result = loading.AsyncWait("移除预约中,请稍后", SocketProxy.Instance.RemoveAppointments(new List<int> { appointment.AppointmentID }));
            if (result.IsSuccess) LoadAppointments();
            else Alert.ShowMessage(false, AlertType.Error, "删除预约失败", result.Error);
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = (sender as FrameworkElement).Tag as Appointment;

            var view = new AddAppointment(appointment, this.loading);
            if (child.ShowDialog("编辑登记", view))
            {
                LoadAppointments();
            }
        }

        #endregion

    }
}
