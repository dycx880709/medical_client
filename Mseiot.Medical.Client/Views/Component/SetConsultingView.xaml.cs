using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Mseiot.Medical.Client.Views.Component
{
    /// <summary>
    /// SetConsultingView.xaml 的交互逻辑
    /// </summary>
    public partial class SetConsultingView : UserControl
    {
        private Loading loading;
        public SetConsultingView(Loading loading)
        {
            InitializeComponent();
            this.loading = loading;
            this.Loaded += SetConsultingView_Loaded;
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                var list = new List<ConsultingRoom>();
                list.Add(new ConsultingRoom { IsUsed = true, Name = "诊室一", UserName = "Doctor.Wang" });
                list.Add(new ConsultingRoom { Name = "诊室二", UserName = "Doctor.Li" });
                lb_consulting.ItemsSource = list;
            }
        }

        private void SetConsultingView_Loaded(object sender, RoutedEventArgs e)
        {
            GetConsultingRooms();
        }

        private void GetConsultingRooms()
        {
            var result = loading.AsyncWait("获取诊所列表中,请稍后", SocketProxy.Instance.GetConsultingRooms());
            if (result.IsSuccess) lb_consulting.ItemsSource = result.Content;
            else MsWindow.ShowDialog($"获取诊所列表失败,{ result.Error }", "软件提示");
        }

        private void ChangeRoomStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                var result = loading.AsyncWait("更新诊室状态中,请稍后", SocketProxy.Instance.ModifyConsultingRoom(room));
                if (!result.IsSuccess)
                {
                    MsWindow.ShowDialog($"获取诊所状态失败,{ result.Error }", "软件提示");
                    room.IsUsed = !room.IsUsed;
                }
            }
        }
    }
}
