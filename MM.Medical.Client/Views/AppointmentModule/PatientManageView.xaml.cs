using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MM.Medical.Client.Views.AppointmentModule
{
    /// <summary>
    /// PatientManageView.xaml 的交互逻辑
    /// </summary>
    public partial class PatientManageView : UserControl
    {
        public PatientInfo Condition { get; private set; }

        public PatientInfo SelectedPatient
        {
            get { return (PatientInfo)GetValue(SelectedPatientProperty); }
            set { SetValue(SelectedPatientProperty, value); }
        }

        public static readonly DependencyProperty SelectedPatientProperty =
            DependencyProperty.Register("SelectedPatient", typeof(PatientInfo), typeof(PatientManageView), new PropertyMetadata(null));

        public PatientManageView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Condition = new PatientInfo { CheckTime = (int)TimeHelper.ToUnixTime(DateTime.Now), CreateTime = (int)TimeHelper.ToUnixTime(DateTime.Now) };
                this.DataContext = this;
                this.Loaded += PatientManageView_Loaded;
            }
        }

        private void PatientManageView_Loaded(object sender, RoutedEventArgs e)
        {
            dg_patient.LoadingRow += (o, ex) => ex.Row.Header = ex.Row.GetIndex() + 1;
            GetConditions();
            GetPatientInfos();
        }

        private void GetConditions()
        {
            var result = loading.AsyncWait("获取预约信息中,请稍后", SocketProxy.Instance.GetBaseWords(
                "收费类型",
                "送检医生",
                "送检科室",
                "检查部位",
                "检查类型",
                "性别"
            ));
            if (result.IsSuccess)
            {
   
            }
        }

        public void Refresh()
        {
            GetPatientInfos();
        }

        private void GetPatientInfos()
        {
            RefreshItemCount();
            var result = loading.AsyncWait("获取预约信息中,请稍后", SocketProxy.Instance.GetPatientInfos
            (
                pager.PageIndex + 1, 
                pager.SelectedCount,
                Condition.CreateTime ?? 0,
                Condition.CheckTime ?? 0,
                checkBody: Condition.CheckBody,
                checkType: Condition.CheckType,
                className: Condition.ClassName,
                doctorName: Condition.DoctorName,
                patientNumber: Condition.Patient.PatientNumber,
                patientName: Condition.Patient.Name,
                sex: Condition.Patient.Sex,
                idCard: Condition.Patient.IdCard,
                telphoneNumber: Condition.Patient.TelphoneNumber,
                diagnoseType: Condition.DiagnoseType,
                chargeType: Condition.ChargeType
            ));
            if (result.IsSuccess)
            {
                pager.TotalCount = result.Content.Total;
                dg_patient.ItemsSource = result.Content.Results;
            }
            else MsWindow.ShowDialog($"获取预约信息失败,{ result.Error }", "软件提示");
        }

        private int RefreshItemCount()
        {
            var columnHeight = CacheHelper.GetResource<int>("DataGrdiColumnHeight");
            var rowHeight = CacheHelper.GetResource<int>("DataGrdiRowHeight");
            var height = dg_patient.ActualHeight - columnHeight;
            var count = (int)(height / rowHeight);
            pager.SelectedCount = count;
            return count;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            new PatientInfo().CopyTo(this.Condition);
            Condition.CheckTime = (int)TimeHelper.ToUnixDate(DateTime.Now);
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }
    }
}
