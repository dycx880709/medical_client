using Ms.Controls;
using Mseiot.Medical.Client.Core;
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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// PatientManageView.xaml 的交互逻辑
    /// </summary>
    public partial class PatientManageView : UserControl
    {
        public PatientInfo Condition { get; } = new PatientInfo();

        public PatientInfo SelectedPatient
        {
            get { return (PatientInfo)GetValue(SelectedPatientProperty); }
            set { SetValue(SelectedPatientProperty, value); }
        }

        public static readonly DependencyProperty SelectedPatientProperty =
            DependencyProperty.Register("SelectedPatient", typeof(PatientInfo), typeof(PatientManageView), new PropertyMetadata(null));

        private Loading loading;

        public PatientManageView()
        {
            InitializeComponent();
            this.loading = loading2;
            this.DataContext = this;
            this.Loaded += PatientManageView_Loaded;
        }

        public PatientManageView(Loading loading) : this()
        { 
            this.loading = loading;
        }

        private void PatientManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }

        public void Refresh()
        {
            GetPatientInfos();
        }

        private void GetPatientInfos()
        {
            RefreshItemCount();
            var result = loading.AsyncWait("获取预约信息中,请稍后", SocketProxy.Instance.GePatients
            (
                pager.PageIndex + 1, 
                pager.SelectedCount
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
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            GetPatientInfos();
        }
    }
}
