using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Controls.Core;
using Ms.Libs.SysLib;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// Medical.xaml 的交互逻辑
    /// </summary>
    public partial class MedicalManageView : UserControl
    {
        public MedicalManageView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += PatientManageView_Loaded;
            }
        }

        private void PatientManageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PatientManageView_Loaded;
            var startTime = TimeHelper.ToUnixDate(DateTime.Now);
            var today = TimeHelper.FromUnixTime(startTime);
            dti_examination.StartTime = today.AddDays(-7);
            dti_examination.EndTime = today.AddDays(7);
            dg_examinations.LoadingRow += (o, ex) => ex.Row.Header = ex.Row.GetIndex() + 1;
            LoadExaminationInfos();
            this.DataContext = this;
        }

        public void Refresh()
        {
            LoadExaminationInfos();
        }

        private void LoadExaminationInfos()
        {
            pager.SelectedCount = dg_examinations.GetFullCountWithoutScroll();
            var result = loading.AsyncWait("获取病历信息中,请稍后", SocketProxy.Instance.GetExaminations
            (
                pager.PageIndex,
                pager.SelectedCount,
                dti_examination.StartTime,
                dti_examination.EndTime,
                tb_user.Text.Trim()
            ));
            if (result.IsSuccess)
            {
                pager.TotalCount = result.Content.Total;
                dg_examinations.ItemsSource = result.Content.Results;
            }
            else Alert.ShowMessage(true, AlertType.Error, $"获取病历信息失败,{ result.Error }");
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            LoadExaminationInfos();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadExaminationInfos();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (dg_examinations.SelectedValue is Examination examination)
                ShowExaminationPartView(examination);
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (dg_examinations.SelectedValue is Examination examination && examination.Appointment != null)
            {
                var view = new ReportPreviewView(examination.Appointment.AppointmentID);
                MsWindow.ShowDialog(view, "打印预览", showInTaskbar: true, windowState: WindowState.Maximized);
            }
        }
        private void Examination_PageChanged(object sender, PageChangedEventArgs args)
        {
            LoadExaminationInfos();
        }

        private void Examination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_examinations.SelectedValue is Examination examination)
                ShowExaminationPartView(examination);
        }

        private void ShowExaminationPartView(Examination examination)
        {
            var result = loading.AsyncWait("获取病历信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(examination.AppointmentID));
            if (result.IsSuccess)
            {
                var view = new ExaminationPartView
                {
                    IsReadOnly = true,
                    Loading = this.loading,
                    SelectedExamination = result.Content,
                    Background = Brushes.White
                };
                MsWindow.ShowDialog(view, "病历记录", windowState: WindowState.Maximized);
            }
            else Alert.ShowMessage(true, AlertType.Error, $"获取病历信息失败,{ result.Error }");
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid datagrid)
            {
                Point aP = e.GetPosition(datagrid);
                var obj = datagrid.InputHitTest(aP);
                if (obj is FrameworkElement element)
                {
                    var dgr = ControlHelper.GetParentObject<DataGridRow>(element);
                    if (dgr != null && dgr.DataContext is Examination examination)
                        ShowExaminationPartView(examination);
                }
            }
        }
    }
}
