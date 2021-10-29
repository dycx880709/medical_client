using Ms.Controls;
using MM.Medical.Client.Views;
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
using System.Collections.ObjectModel;
using Ms.Controls.Core;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ConsultingManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ConsultingManageView : UserControl
    {
        public ConsultingManageView()
        {
            InitializeComponent();
            this.Loaded += ConsultingManageView_Loaded;
        }

        private void ConsultingManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetConsultingRooms();
            this.MouseLeftButtonDown += (o, ex) => ResetConsultingRoom();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (lb_rooms.ItemsSource is ObservableCollection<ConsultingRoom> rooms)
            {
                ResetConsultingRoom();
                var room = new ConsultingRoom() { IsSelected = true };
                rooms.Add(room);
                lb_rooms.ScrollIntoView(room);
                lb_rooms.SelectedValue = room;
                var lbv = lb_rooms.ItemContainerGenerator.ContainerFromIndex(lb_rooms.Items.Count - 1) as ListBoxItem;
                var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                tb.Focus();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                if (room.ConsultingRoomID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"新建诊室名称不能为空", "软件提示");
                        return;
                    }
                    var add = new ConsultingRoom { Name = tb.Text };
                    var result = loading.AsyncWait("新建诊室中,请稍后", SocketProxy.Instance.AddConsultingRoom(add));
                    if (result.IsSuccess)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        room.ConsultingRoomID = result.Content;
                        room.IsSelected = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"编辑诊室名称不能为空", "软件提示");
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(room.Name))
                    {
                        var update = room.Copy();
                        update.Name = tb.Text;
                        var result = loading.AsyncWait("更新诊室中,请稍后", SocketProxy.Instance.ModifyConsultingRoom(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            room.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新诊室失败,{ result.Error }", "软件提示");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                ResetConsultingRoom();
                lb_rooms.SelectedValue = room;
                room.IsSelected = true;
                var grid = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(grid);
                tb.SelectionStart = tb.Text.Length;
                tb.Focus();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                ResetConsultingRoom();
                var lb = ControlHelper.GetParentObject<ListBox>(element);
                if (lb.ItemsSource is ObservableCollection<ConsultingRoom> rooms)
                {
                    if (room.IsUsed)
                    {
                        MsWindow.ShowDialog($"诊室使用中,删除诊室失败", "软件提示");
                        return;
                    }
                    var result = loading.AsyncWait("删除诊室中,请稍后", SocketProxy.Instance.RemoveConsultingRoom(room.ConsultingRoomID));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"删除诊室失败,{ result.Error }", "软件提示");
                    else rooms.Remove(room);
                }
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetConsultingRooms();
        }
        private void GetConsultingRooms()
        {
            var result = loading.AsyncWait("获取诊所信息中,请稍后", SocketProxy.Instance.GetConsultingRooms());
            if (result.IsSuccess) lb_rooms.ItemsSource = new ObservableCollection<ConsultingRoom>(result.Content);
            else MsWindow.ShowDialog($"获取诊所信息失败,{ result.Error }", "软件提示");
        }

        private void ResetConsultingRoom()
        {
            var room = lb_rooms.Items.OfType<ConsultingRoom>().FirstOrDefault(p => p.IsSelected);
            if (room != null)
            {
                if (room.ConsultingRoomID == 0)
                    (lb_rooms.ItemsSource as ObservableCollection<ConsultingRoom>).Remove(room);
                else
                {
                    var index = lb_rooms.Items.IndexOf(room);
                    var lbi = lb_rooms.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    room.IsSelected = false;
                }
            }
        }
    }
}
