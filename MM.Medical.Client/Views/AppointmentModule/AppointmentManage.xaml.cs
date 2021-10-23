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
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddAppointment(new Appointment(), this.loading);
            if (child.ShowDialog("预约登记", view))
            {

            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (pmv.SelectedPatient != null)
            {
                var result = loading.AsyncWait("移除预约中,请稍后", SocketProxy.Instance.RemovePatientInfo(pmv.SelectedPatient.PatientInfoID));
                if (result.IsSuccess) pmv.Refresh();
                else MsWindow.ShowDialog($"删除预约失败,{ result.Error }", "软件提示");
            }
            else MsWindow.ShowDialog($"请选择移除预约项", "软件提示");
            */
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            //pmv.Refresh();
        }

        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (pmv.SelectedPatient != null)
            {
                var result = loading.AsyncWait("签到预约中,请稍后", SocketProxy.Instance.SignPatientInfo(pmv.SelectedPatient.PatientInfoID));
                if (result.IsSuccess) pmv.Refresh();
                else MsWindow.ShowDialog($"签到预约失败,{ result.Error }", "软件提示");
            }
            else MsWindow.ShowDialog($"请选择签到项", "软件提示");
            */
        }

        private void UnSign_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (pmv.SelectedPatient != null)
            {
                if (pmv.SelectedPatient.PatientStatus == PatientStatus.Regist)
                {
                    var result = loading.AsyncWait("取消签到预约中,请稍后", SocketProxy.Instance.UnSignPatientInfo(pmv.SelectedPatient.PatientInfoID));
                    if (result.IsSuccess) pmv.Refresh();
                    else MsWindow.ShowDialog($"取消签到预约失败,{ result.Error }", "软件提示");
                }
            }
            else MsWindow.ShowDialog($"请选择取消签到项", "软件提示");
            */
        }

        private void SetConsulting_Click(object sender, RoutedEventArgs e)
        {
     
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (pmv.SelectedPatient != null)
            {
                var view = new AddAppointment(pmv.SelectedPatient, this.loading);
                if (child.ShowDialog("编辑登记", view))
                {
                    var result = loading.AsyncWait("编辑预约中,请稍后", SocketProxy.Instance.ModifyPatientInfo(pmv.SelectedPatient));
                    if (result.IsSuccess) pmv.Refresh();
                    else MsWindow.ShowDialog($"编辑预约失败,{ result.Error }", "软件提示");
                }
            }
            else MsWindow.ShowDialog($"请选择编辑预约项", "软件提示");
            */
        }
    }
}
