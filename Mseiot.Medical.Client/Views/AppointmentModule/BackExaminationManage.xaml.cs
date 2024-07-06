using Ms.Controls.Core;
using Ms.Controls;
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
using System.Runtime.CompilerServices;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// BackExaminationManage.xaml 的交互逻辑
    /// </summary>
    public partial class BackExaminationManage : UserControl
    {
        public BackExaminationManage()
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
            dti_back.StartTime = today;
            dti_back.EndTime = today.AddDays(7);
            dg_back.LoadingRow += (o, ex) => ex.Row.Header = ex.Row.GetIndex() + 1;
            LoadExaminationInfos();
            this.DataContext = this;
        }

        public void Refresh()
        {
            LoadExaminationInfos();
        }

        private async void LoadExaminationInfos()
        {
            pager.SelectedCount = dg_back.GetFullCountWithoutScroll();
            loading.Start("获取回访信息中,请稍后");
            var result = await SocketProxy.Instance.GetBackRecords
            (
                pager.PageIndex,
                pager.SelectedCount,
                dti_back.StartTime,
                dti_back.EndTime,
                tb_user.Text.Trim()
            );
            this.Dispatcher.Invoke(() =>
            {
                loading.Stop();
                if (result.IsSuccess)
                {
                    pager.TotalCount = result.Content.Total;
                    dg_back.ItemsSource = result.Content.Results;
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取回访信息失败,{result.Error}");
                }
            });
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
            if (dg_back.SelectedValue is Examination examination)
            {
                ShowExaminationPartView(examination);
            }
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (dg_back.SelectedValue is Examination examination && examination.Appointment != null)
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
            if (dg_back.SelectedValue is Examination examination)
            {
                ShowExaminationPartView(examination);
            }
        }

        private void ShowExaminationPartView(Examination examination, bool isReadOnly = true)
        {
            var result = loading.AsyncWait("获取回访信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(examination.AppointmentID));
            if (result.IsSuccess)
            {
                var window = new ExaminationPartWindow();
                window.epv.IsReadOnly = isReadOnly;
                window.epv.SelectedExamination = result.Content;
                window.epv.Background = Brushes.White;
                if (window.ShowDialog().Value)
                {
                    result.Content.CopyTo(examination);
                }
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"获取回访信息失败,{result.Error}");
            }
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
                    {
                        ShowExaminationPartView(examination);
                    }
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg_back.SelectedValue is Examination examination)
            {
                ShowExaminationPartView(examination, false);
            }
        }
    }
}
