using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Client.Core;
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
    /// AddBookingView.xaml 的交互逻辑
    /// </summary>
    public partial class AddBookingView : UserControl
    {
        private PatientInfo patientInfo;
        private PatientInfo originPatientInfo;
        private Loading loading;
        public AddBookingView(PatientInfo patientInfo, Loading loading)
        {
            InitializeComponent();
            this.originPatientInfo = patientInfo;
            this.patientInfo = patientInfo.Copy();
            if (patientInfo.PatientInfoID == 0)
                patientInfo.CheckTime = (int)TimeHelper.ToUnixDate(DateTime.Now);
            else cp_date.IsEnabled = false;
            this.loading = loading;
            LoadViewProperty();
            this.Loaded += AddBookingView_Loaded;
        }

        private void LoadViewProperty()
        {
            cb_diagnose.ItemsSource = EnumHelper.GetDescriptions<DiagnoseType>();
        }

        private void AddBookingView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
            this.DataContext = this.patientInfo;
            if (patientInfo.PatientInfoID == 0)
            {
                patientInfo.CheckTime = (int)TimeHelper.ToUnixDate(DateTime.Now);
                tb_name.Focus();
            }
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取基础信息中,请稍后", SocketProxy.Instance.GetBaseWords(
                "收费类型",
                "检查部位",
                "检查类型",
                "送检科室",
                "送检医生",
                "病区号",
                "婚姻状况",
                "职业",
                "民族",
                "性别"
            ));
            if (result.IsSuccess)
            {
                cb_charge.ItemsSource = result.SplitContent("收费类型");
                cb_body.ItemsSource = result.SplitContent("检查部位");
                cb_class.ItemsSource = result.SplitContent("送检科室");
                cb_doctor.ItemsSource = result.SplitContent("送检医生");
                cb_area.ItemsSource = result.SplitContent("病区号");
                cb_marry.ItemsSource = result.SplitContent("婚姻状况");
                cb_occupation.ItemsSource = result.SplitContent("职业");
                cb_nation.ItemsSource = result.SplitContent("民族");
                cb_checkType.ItemsSource = result.SplitContent("检查类型");
                cb_sex.ItemsSource = result.SplitContent("性别");
            }
        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(patientInfo.Patient.Name))
            {
                MsWindow.ShowDialog("输入患者姓名不合法", "软件提示");
                return;
            }
            if (string.IsNullOrWhiteSpace(patientInfo.Patient.Sex))
            {
                MsWindow.ShowDialog("输入患者性别不能为空", "软件提示");
                return;
            }
            if (patientInfo.Patient.Age == 0)
            {
                MsWindow.ShowDialog("输入患者年龄不能为零", "软件提示");
                return;
            }
            if (patientInfo.CheckTime < TimeHelper.ToUnixDate(DateTime.Now))
            {
                MsWindow.ShowDialog("预约检查时间不能早于当天", "软件提示");
                return;
            }
            if (patientInfo.PatientInfoID == 0)
            {
                patientInfo.DiagnoseType = "初诊";
                patientInfo.Patient.PatientNumber = Guid.NewGuid().ToString("N");
                var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddPatientInfo(this.patientInfo));
                if (result.IsSuccess)
                {
                    patientInfo.PatientInfoID = result.Content;
                    patientInfo.CopyTo(originPatientInfo);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"新增预约失败,{ result.Error }", "软件提示");
            }
            else
            {
                var result = loading.AsyncWait("编辑预约中,请稍后", SocketProxy.Instance.ModifyPatientInfo(this.patientInfo));
                if (result.IsSuccess)
                {
                    patientInfo.CopyTo(originPatientInfo);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"编辑预约失败,{ result.Error }", "软件提示");
            }
        }

        private void GetPatient_Click(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("信息校验中,请稍后", SocketProxy.Instance.GetPatientByCondition(
                idCard: patientInfo.Patient.IdCard,
                socialId: patientInfo.Patient.SocialSecurityCode,
                telphoneNumber: patientInfo.Patient.TelphoneNumber
            ));
            if (result.IsSuccess && result.Content != null && result.Content.PatientID != 0)
                result.Content.CopyTo(patientInfo.Patient);
        }
    }
}
