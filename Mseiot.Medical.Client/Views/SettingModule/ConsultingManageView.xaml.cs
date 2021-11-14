﻿using Ms.Controls;
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
using MM.Medical.Client.Core;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ConsultingManageView.xaml 的交互逻辑
    /// </summary>
    public partial class ConsultingManageView : UserControl
    {
        public List<string> ExaminationTypes { get; set; }
        public ConsultingManageView()
        {
            InitializeComponent();
            this.Loaded += ConsultingManageView_Loaded;
        }

        private void ConsultingManageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ConsultingManageView_Loaded;
            GetExaminationTypes();
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
                lb_rooms.UpdateLayout();
                var lbv = lb_rooms.ItemContainerGenerator.ContainerFromIndex(lb_rooms.Items.Count - 1) as ListBoxItem;
                if (lbv != null)
                {
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                    tb.Focus();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConsultingRoom room)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                var cb = ControlHelper.GetVisualChild<ComboBox>(parent);
                if (room.ConsultingRoomID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"新建诊室名称不能为空", "软件提示");
                        return;
                    }
                    var add = new ConsultingRoom { Name = tb.Text, ExaminationTypes = cb.Text };
                    var result = loading.AsyncWait("新建诊室中,请稍后", SocketProxy.Instance.AddConsultingRoom(add));
                    if (result.IsSuccess)
                    {
                        cb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
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
                        tb.GetBindingExpression(ComboBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(room.Name) || !cb.Text.Equals(room.ExaminationTypes))
                    {
                        var update = room.Copy();
                        update.Name = tb.Text;
                        update.ExaminationTypes = cb.Text;
                        var result = loading.AsyncWait("更新诊室中,请稍后", SocketProxy.Instance.ModifyConsultingRoom(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            cb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
                            room.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新诊室失败,{ result.Error }", "软件提示");
                            cb.GetBindingExpression(ComboBox.TextProperty).UpdateTarget();
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

        private void GetExaminationTypes()
        {
            var result = loading.AsyncWait("获取检查类型信息中,请稍后", SocketProxy.Instance.GetBaseWords("检查类型"));
            if (result.IsSuccess)
                this.ExaminationTypes = result.SplitContent("检查类型");
            else Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
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
                        cb_type.Text += cb.Content.ToString() + ",";
                }
                if (!string.IsNullOrEmpty(cb_type.Text))
                    cb_type.Text = cb_type.Text.Substring(0, cb_type.Text.Length - 1);
            }
        }
    }
}
