using Ms.Controls;
using Mseiot.Medical.Client.Views.Component;
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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// ConsultingRoomManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ConsultingRoomManageView : UserControl
    {
        public ConsultingRoomManageView()
        {
            InitializeComponent();
            this.Loaded += ConsultingRoomManageView_Loaded;
        }

        private void ConsultingRoomManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetConsultingRooms();
        }

        private void GetConsultingRooms()
        {
            var result = loading.AsyncWait("获取诊所信息中,请稍后", SocketProxy.Instance.GetConsultingRooms());
            if (result.IsSuccess) lb_rooms.ItemsSource = result.Content;
            else MsWindow.ShowDialog($"获取诊所信息失败,{ result.Error }", "软件提示");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var room = new ConsultingRoom();
            var view = new AddConsultingView(room, this.loading);
            if (sp.ShowDialog("添加诊室", view))
                GetConsultingRooms();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                var view = new AddConsultingView(room, this.loading);
                if (sp.ShowDialog("编辑诊室", view))
                    GetConsultingRooms();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                var result = loading.AsyncWait("删除诊室中,请稍后", SocketProxy.Instance.RemoveConsultingRoom(room.ConsultingRoomID));
                if (result.IsSuccess) GetConsultingRooms();
                else MsWindow.ShowDialog($"删除诊室失败,{ result.Error }", "软件提示");
            }
        }
    }
}
