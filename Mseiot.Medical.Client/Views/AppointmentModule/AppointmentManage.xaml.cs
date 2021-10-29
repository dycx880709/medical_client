﻿using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Libs.SysLib;
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

namespace MM.Medical.Client.Views
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
            var startTime = TimeHelper.ToUnixDate(DateTime.Now);
            var today = TimeHelper.FromUnixTime(startTime);
            dtiTime.StartTime = today.AddDays(-14);
            dtiTime.EndTime = today.AddDays(14);
            LoadAppointments();
        }

        #region 数据

        private async void LoadAppointments()
        {
            pager.SelectedCount = dgAppointments.GetFullCountWithoutScroll();
            var result = await SocketProxy.Instance.GetAppointments(
                pager.PageIndex + 1,
                pager.SelectedCount,
                dtiTime.StartTime,
                dtiTime.EndTime,
                tbSearchName.Text
            );
            if (result.IsSuccess)
            { 
                dgAppointments.ItemsSource = new ObservableCollection<Appointment>(result.Content.Results);
                pager.TotalCount = result.Content.Total;
            }
            else
                Alert.ShowMessage(false, AlertType.Error, result.Error);
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
            if (child.ShowDialog("新增预约", view))
                LoadAppointments();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppointments.SelectedValue is Appointment appointment)
            {
                var result = loading.AsyncWait("删除预约中,请稍后", SocketProxy.Instance.RemoveAppointments(new List<int> { appointment.AppointmentID }));
                if (result.IsSuccess) LoadAppointments();
                else Alert.ShowMessage(false, AlertType.Error, "删除预约失败", result.Error);
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppointments.SelectedValue is Appointment appointment)
            {
                var view = new AddAppointment(appointment, this.loading);
                if (child.ShowDialog("修改预约", view))
                    LoadAppointments();
            }
        }

        #endregion

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAppointments();
        }

        private void PunchIn_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppointments.SelectedValue is Appointment appointment)
            {
                if (appointment.AppointmentStatus == AppointmentStatus.Reserved)
                {
                    var bakAppointment = appointment.Copy();
                    bakAppointment.AppointmentStatus = AppointmentStatus.PunchIn;
                    var result = loading.AsyncWait("签到预约中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(bakAppointment));
                    if (result.IsSuccess) LoadAppointments();
                    else Alert.ShowMessage(true, AlertType.Error, $"签到预约失败,{ result.Error }");
                }
            }
            else Alert.ShowMessage(true, AlertType.Warning, $"请选择签到项");
        }

        private void UnPunch_Click(object sender, RoutedEventArgs e)
        {
            if (dgAppointments.SelectedValue is Appointment appointment)
            {
                if (appointment.AppointmentStatus == AppointmentStatus.PunchIn)
                {
                    var bakAppointment = appointment.Copy();
                    bakAppointment.AppointmentStatus = AppointmentStatus.Reserved;
                    var result = loading.AsyncWait("取消签到预约中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(bakAppointment));
                    if (result.IsSuccess) LoadAppointments();
                    else Alert.ShowMessage(true, AlertType.Error, $"取消签到预约失败,{ result.Error }");
                }
            }
            else Alert.ShowMessage(true, AlertType.Warning, $"请选择取消签到项");
        }
    }
}
