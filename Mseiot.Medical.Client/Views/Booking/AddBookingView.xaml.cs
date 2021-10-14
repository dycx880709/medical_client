using Ms.Controls;
using Ms.Libs.SysLib;
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
    /// AddBookingView.xaml 的交互逻辑
    /// </summary>
    public partial class AddBookingView : UserControl
    {
        private PatientInfo patient;
        private PatientInfo originPatient;
        private Loading loading;
        public AddBookingView(PatientInfo patient, Loading loading)
        {
            InitializeComponent();
            this.originPatient = patient;
            this.patient = patient.Copy();
            this.loading = loading;
            LoadViewProperty();
            this.Loaded += AddBookingView_Loaded;
        }

        private void LoadViewProperty()
        {
            cb_sex.ItemsSource = EnumHelper.GetDescriptions<Sex>();
            cb_age.ItemsSource = EnumHelper.GetDescriptions<AgeRange>();
            cb_diagnose.ItemsSource = EnumHelper.GetDescriptions<DiagnoseType>();
        }

        private void AddBookingView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取基础信息中,请稍后", SocketProxy.Instance.GetBaseWords(
                "收费类型", 
                "检查部位", 
                "送检科室",
                "送检医生",
                "病区号",
                "婚姻状况",
                "职业",
                "民族"
            ));
            if (result.IsSuccess)
            {
                var chargeType = result.Content.FirstOrDefault(t => t.Title.Equals("收费类型"));
                cb_charge.ItemsSource = string.IsNullOrEmpty(chargeType.Content) ? new List<string>() : chargeType.Content.Split(',').ToList();
                var checkBody = result.Content.FirstOrDefault(t => t.Title.Equals("检查部位"));
                cb_body.ItemsSource = string.IsNullOrEmpty(checkBody.Content) ? new List<string>() : checkBody.Content.Split(',').ToList();
                var className = result.Content.FirstOrDefault(t => t.Title.Equals("送检科室"));
                cb_class.ItemsSource = string.IsNullOrEmpty(className.Content) ? new List<string>() : className.Content.Split(',').ToList();
                var doctorName = result.Content.FirstOrDefault(t => t.Title.Equals("送检医生"));
                cb_doctor.ItemsSource = string.IsNullOrEmpty(doctorName.Content) ? new List<string>() : doctorName.Content.Split(',').ToList();
                var checkArea = result.Content.FirstOrDefault(t => t.Title.Equals("病区号"));
                cb_area.ItemsSource = string.IsNullOrEmpty(checkArea.Content) ? new List<string>() : checkArea.Content.Split(',').ToList();
                var marryType = result.Content.FirstOrDefault(t => t.Title.Equals("婚姻状况"));
                cb_marry.ItemsSource = string.IsNullOrEmpty(marryType.Content) ? new List<string>() : marryType.Content.Split(',').ToList();
                var occupation = result.Content.FirstOrDefault(t => t.Title.Equals("职业"));
                cb_occupation.ItemsSource = string.IsNullOrEmpty(occupation.Content) ? new List<string>() : occupation.Content.Split(',').ToList();
                var nation = result.Content.FirstOrDefault(t => t.Title.Equals("民族"));
                cb_nation.ItemsSource = string.IsNullOrEmpty(nation.Content) ? new List<string>() : nation.Content.Split(',').ToList();
            }
        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddPatient(this.patient));
            if (result.IsSuccess)
            {
                patient.PatientInfoID = result.Content;
                patient.CopyTo(originPatient);
                this.Close();
            }
            else MsWindow.ShowDialog($"新增预约失败,{ result.Error }", "软件提示");
        }
    }
}
