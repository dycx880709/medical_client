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

namespace Mseiot.Medical.Client.Views.Component
{
    /// <summary>
    /// AddCosnsultingView.xaml 的交互逻辑
    /// </summary>
    public partial class AddConsultingView : UserControl
    {
        private ConsultingRoom room;
        private ConsultingRoom originRoom;
        private Loading loading;
        public AddConsultingView(ConsultingRoom room, Loading loading)
        {
            InitializeComponent();
            this.room = room.Copy();
            this.originRoom = room;
            this.loading = loading;
            this.DataContext = this.room;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(room.Name))
            {
                MsWindow.ShowDialog("诊室名称不能为空", "软件提示");
                return;
            }
            if (room.ConsultingRoomID == 0)
            {
                var result = loading.AsyncWait("新增诊所中,请稍后", SocketProxy.Instance.AddConsultingRoom(room));
                if (result.IsSuccess)
                {
                    room.ConsultingRoomID = result.Content;
                    room.CopyTo(originRoom);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"新增诊所失败,{ result.Error }", "软件提示");
            }
            else
            {
                var result = loading.AsyncWait("编辑诊所中,请稍后", SocketProxy.Instance.ModifyConsultingRoom(room));
                if (result.IsSuccess)
                {
                    room.CopyTo(originRoom);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"编辑诊所失败,{ result.Error }", "软件提示");
            }
        }
    }
}
