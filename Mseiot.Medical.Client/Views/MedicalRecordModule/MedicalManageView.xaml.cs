using MM.Medical.Client.Core;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// Medical.xaml 的交互逻辑
    /// </summary>
    public partial class MedicalManageView : UserControl
    {
        public Appointment Condition { get; private set; }

        public MedicalManageView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Condition = new Appointment { AppointmentTime = TimeHelper.ToUnixTime(DateTime.Now) };
                this.DataContext = this;
                this.Loaded += PatientManageView_Loaded;
            }
        }

        private void PatientManageView_Loaded(object sender, RoutedEventArgs e)
        {
            dg_examinations.LoadingRow += (o, ex) => ex.Row.Header = ex.Row.GetIndex() + 1;
            GetConditions();
            LoadExaminationInfos();
        }

        private void GetConditions()
        {
            var result = loading.AsyncWait("数据加载中,请稍后", SocketProxy.Instance.GetBaseWords(
                "收费类型",
                "送检医生",
                "送检科室",
                "检查部位",
                "检查类型",
                "性别"
            ));
            if (result.IsSuccess)
            {
                cb_chargeType.ItemsSource = result.SplitContent("收费类型");
                cb_checkBody.ItemsSource = result.SplitContent("检查部位");
                cb_checkType.ItemsSource = result.SplitContent("检查类型");
                cb_doctorName.ItemsSource = result.SplitContent("送检医生");
                cb_className.ItemsSource = result.SplitContent("送检科室");
                cb_sex.ItemsSource = result.SplitContent("性别");
            }
        }

        public void Refresh()
        {
            LoadExaminationInfos();
        }

        private void LoadExaminationInfos()
        {
            pager.SelectedCount = dg_examinations.GetFullCountWithoutScroll();
            var result = loading.AsyncWait("获取预约信息中,请稍后", SocketProxy.Instance.GetExaminations
            (
                pager.PageIndex + 1,
                pager.SelectedCount
            ));
            if (result.IsSuccess)
            {
                pager.TotalCount = result.Content.Total;
                dg_examinations.ItemsSource = result.Content.Results;
            }
            else Alert.ShowMessage(true, AlertType.Error, $"获取预约信息失败,{ result.Error }");
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            new Appointment().CopyTo(this.Condition);
            Condition.AppointmentTime = TimeHelper.ToUnixDate(DateTime.Now);
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            LoadExaminationInfos();
        }
    }
}
