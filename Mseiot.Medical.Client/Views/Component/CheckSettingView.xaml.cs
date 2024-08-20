using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// CheckSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class CheckSettingView : UserControl
    {
        public Appointment Appointment { get; set; }
        private readonly Loading loading;

        public CheckSettingView(Appointment appointment, Loading loading)
        {
            InitializeComponent();
            this.Appointment = appointment.Copy();
            this.loading = loading;
            this.Loaded += CheckSettingView_Loaded;
        }

        private void CheckSettingView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CheckSettingView_Loaded;
            GetAppointInfos();
            this.DataContext = this.Appointment;
        }

        private void GetAppointInfos()
        {
            var result = loading.AsyncWait("获取预约诊室中,请稍后", SocketProxy.Instance.GetConsultingRooms());
            if (result.IsSuccess)
            {
                cb_room.ItemsSource = result.Content.Where(t => Appointment.AppointmentType.All(s => t.ExaminationTypes.Split(',').Contains(s)));
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"获取预约诊室失败,{ result.Error }");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Appointment.AppointmentStatus = AppointmentStatus.PunchIn;
            var result = loading.AsyncWait("签到预约中,请稍后", SocketProxy.Instance.ModifyAppointment(this.Appointment));
            if (result.IsSuccess)
            {
                this.Close(true);
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"签到预约失败,{ result.Error }");
            }
        }
    }
}
