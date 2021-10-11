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
            cb_marry.ItemsSource = EnumHelper.GetDescriptions<MarryType>();
            cb_charge.ItemsSource = EnumHelper.GetDescriptions<ChargeType>();
        }

        private void AddBookingView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Rescan_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("新增预约中,请稍后", SocketProxy.Instance.AddPatient(this.patient));
            if (result.IsSuccess)
            {
                patient.PatientID = result.Content;
                patient.CopyTo(originPatient);
                this.Close();
            }
            else MsWindow.ShowDialog($"新增预约失败,{ result.Error }", "软件提示");
        }
    }
}
