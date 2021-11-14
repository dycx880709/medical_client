using LiveCharts.Geared;
using LiveCharts.Wpf;
using MM.Medical.Client.Core;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// SpecialStatisticsView.xaml 的交互逻辑
    /// </summary>
    public partial class SpecialStatisticsView : UserControl
    {
        #region DP
        public StatisticsType StatisticsType
        {
            get { return (StatisticsType)GetValue(StatisticsTypeProperty); }
            set { SetValue(StatisticsTypeProperty, value); }
        }
        public static readonly DependencyProperty StatisticsTypeProperty = DependencyProperty.Register("StatisticsType", typeof(StatisticsType), typeof(SpecialStatisticsView), new PropertyMetadata(StatisticsType.CurrentDay));

        public ChartType ChartType
        {
            get { return (ChartType)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }
        public static readonly DependencyProperty ChartTypeProperty = DependencyProperty.Register("ChartType", typeof(ChartType), typeof(SpecialStatisticsView), new PropertyMetadata(ChartType.LineSeries));

        public string ExaminationResult
        {
            get { return (string)GetValue(ExaminationResultProperty); }
            set { SetValue(ExaminationResultProperty, value); }
        }
        public static readonly DependencyProperty ExaminationResultProperty = DependencyProperty.Register("ExaminationResult", typeof(string), typeof(SpecialStatisticsView), new PropertyMetadata(string.Empty));

        public string DoctorName
        {
            get { return (string)GetValue(DoctorNameProperty); }
            set { SetValue(DoctorNameProperty, value); }
        }
        public static readonly DependencyProperty DoctorNameProperty = DependencyProperty.Register("DoctorName", typeof(string), typeof(SpecialStatisticsView), new PropertyMetadata(string.Empty));

        public string CosultingName
        {
            get { return (string)GetValue(CosultingNameProperty); }
            set { SetValue(CosultingNameProperty, value); }
        }
        public static readonly DependencyProperty CosultingNameProperty = DependencyProperty.Register("CosultingName", typeof(string), typeof(SpecialStatisticsView), new PropertyMetadata(string.Empty));

        #endregion

        public SpecialStatisticsView()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += SpecialStatisticsView_Loaded;
        }

        private void SpecialStatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SpecialStatisticsView_Loaded;
            GetDateDatas();
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var count = dg_examinations.GetFullCountWithoutScroll();
            var timeInterval = GetStartEndTime();
            var result = loading.AsyncWait("获取数据中,请稍后", SocketProxy.Instance.GetExaminationCountByTime(
                timeInterval.Item2,
                timeInterval.Item3,
                this.StatisticsType,
                "",
                CacheHelper.CurrentUser.UserID,
                this.ExaminationResult,
                this.DoctorName,
                this.CosultingName));
            if (result.IsSuccess) LoadExaminationDatas(result.Content);
            else Alert.ShowMessage(true, AlertType.Error, $"获取数据失败,{ result.Error }");
        }

        private void ChartType_Click(object sender, RoutedEventArgs e)
        {
            if (chart.Series[0] is Series series && series.Values is GearedValues<TimeResult> datas)
            {
                chart.Series.Clear();
                LoadChartSeries(datas);
            }
        }

        private void LoadExaminationDatas(TimeResultCollection content)
        {
            chart.Series.Clear();
            var datas = new GearedValues<TimeResult>(content);
            LoadChartSeries(datas);
        }

        private (int, DateTime?, DateTime?) GetStartEndTime()
        {
            var endDate = DateTime.Now.Date;
            DateTime? startDate = null;
            int timeType = 0;
            switch (this.StatisticsType)
            {
                case StatisticsType.CurrentDay:
                    startDate = endDate.AddDays(-1);
                    timeType = -1;
                    break;
                case StatisticsType.CurrenWeek:
                    startDate = endDate;
                    while (endDate.DayOfWeek != DayOfWeek.Monday)
                        endDate.AddDays(-1);
                    timeType = 0;
                    break;
                case StatisticsType.CurrentMonth:
                    startDate = new DateTime(endDate.Year, endDate.Month, 1);
                    timeType = 0;
                    break;
                case StatisticsType.CurrentYear:
                    startDate = new DateTime(endDate.Year, 1, 1);
                    timeType = 1;
                    break;
                case StatisticsType.Definition:
                    if (cb_day.SelectedIndex != 0)
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, cb_month.SelectedIndex, cb_day.SelectedIndex);
                        endDate = startDate.Value.AddDays(1);
                        timeType = 0;
                    }
                    else if (cb_month.SelectedIndex != 0)
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, cb_month.SelectedIndex, 1);
                        endDate = startDate.Value.AddMonths(1);
                        timeType = 0;
                    }
                    else
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, 1, 1);
                        endDate = startDate.Value.AddYears(1);
                        timeType = 1;
                    }
                    break;
            }
            return (timeType, startDate, endDate);
        }

        private void GetDateDatas()
        {
            var years = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++)
                years.Add(i);
            cb_year.ItemsSource = years;
            cb_year.SelectedIndex = 0;
            cb_month.ItemsSource = new string[] { "全部", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
            cb_month.SelectedIndex = 0;
            cb_month.SelectionChanged += (_, e) =>
            {
                if (cb_month.SelectedIndex == 0)
                    cb_day.ItemsSource = new string[] { "全部" };
                else if (cb_year.SelectedValue is int year)
                {
                    var currentMonth = new DateTime(year, cb_month.SelectedIndex, 1);
                    var lastMonth = currentMonth.AddMonths(1);
                    var totalDays = (lastMonth - currentMonth).TotalDays;
                    var days = new List<string>() { "全部" };
                    for (int i = 1; i <= totalDays; i++)
                        days.Add(i.ToString());
                    cb_day.ItemsSource = days;
                    cb_day.SelectedIndex = 0;
                }
            };
        }

        private void LoadChartSeries(GearedValues<TimeResult> datas)
        {
            switch (this.ChartType)
            {
                case ChartType.LineSeries:
                    chart.Series.Add(new GLineSeries { Values = datas });
                    break;
                case ChartType.Bar:
                    chart.Series.Add(new GColumnSeries { Values = datas });
                    break;
                case ChartType.Pie:
                    chart.Series.Add(new GCandleSeries { Values = datas });
                    break;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.ExaminationResult = string.Empty;
            this.DoctorName = string.Empty; 
            this.CosultingName = string.Empty;
        }
    }
}
