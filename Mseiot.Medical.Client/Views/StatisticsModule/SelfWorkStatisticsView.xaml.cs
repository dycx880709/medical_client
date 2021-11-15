using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
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
    public enum ChartType
    {
        [Description("柱形图")]
        Bar,
        [Description("曲线图")]
        LineSeries,
        [Description("饼状图")]
        Pie
    }

    /// <summary>
    /// SelfWorkStatisticsView.xaml 的交互逻辑
    /// </summary>
    public partial class SelfWorkStatisticsView : UserControl
    {
        public StatisticsType StatisticsType
        {
            get { return (StatisticsType)GetValue(StatisticsTypeProperty); }
            set { SetValue(StatisticsTypeProperty, value); }
        }
        public static readonly DependencyProperty StatisticsTypeProperty = DependencyProperty.Register("StatisticsType", typeof(StatisticsType), typeof(SelfWorkStatisticsView), new PropertyMetadata(StatisticsType.CurrentDay));

        public ChartType ChartType
        {
            get { return (ChartType)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }
        public static readonly DependencyProperty ChartTypeProperty = DependencyProperty.Register("ChartType", typeof(ChartType), typeof(SelfWorkStatisticsView), new PropertyMetadata(ChartType.Bar));
       
        public Func<double, string> Formatter { get; set; } = t =>
        {
            var dateTime = TimeHelper.FromUnixTime(Convert.ToInt64(t));
            var format = dateTime.ToShortDateString();
            return format;
        };

        public SelfWorkStatisticsView()
        {
            InitializeComponent();
            this.Loaded += SelfWorkStatisticsView_Loaded;
            this.DataContext = this;
        }

        private void SelfWorkStatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SelfWorkStatisticsView_Loaded;
            GetDateDatas();
        }

        private void ChartType_Click(object sender, RoutedEventArgs e)
        {
            if (chart.Series.Count > 0 && chart.Series[0] is Series series && series.Values is IList<TimeResult> datas)
                ReloadChart(datas);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var count = dg_examinations.GetFullCountWithoutScroll();
            var timeInterval = GetStartEndTime();
            var result = loading.AsyncWait("获取数据中,请稍后", SocketProxy.Instance.GetExaminationCountByTime(
                timeInterval.Item1,
                timeInterval.Item2,
                this.StatisticsType,
                userInfo: "",
                doctorID: CacheHelper.CurrentUser.UserID,
                consultingName: CacheHelper.ConsultingRoomName));
            if (result.IsSuccess) 
                ReloadChart(result.Content);
            else 
                Alert.ShowMessage(true, AlertType.Error, $"获取数据失败,{ result.Error }");
        }

        private void ReloadChart(IList<TimeResult> datas)
        {
            chart.Series.Clear();
            var yValues = new ChartValues<double>(datas.Select(t => (double)t.Count));
            axisX.Labels = new ChartValues<string>(datas.Select(t => TimeHelper.FromUnixTime(t.TimeStamp).ToShortDateString()));
            axisY.MaxValue = datas.Max(t => t.Count);
            axisY.MinValue = 0;
            var yStep = (int)((axisY.MaxValue - axisY.MinValue) / yValues.Count);
            if (yStep == 0) yStep = 1;
            axisY.Separator = new LiveCharts.Wpf.Separator { Step = yStep };
            switch (this.ChartType)
            {
                case ChartType.LineSeries:
                    chart.Series.Add(new LineSeries { Values = yValues });
                    break;
                case ChartType.Bar:
                    chart.Series.Add(new ColumnSeries { Values = yValues });
                    break;
                case ChartType.Pie:
                    chart.Series.Add(new PieSeries { Values = yValues });
                    break;
            }
        }

        private (DateTime?, DateTime?) GetStartEndTime()
        {
            var endDate = DateTime.Now.Date.AddDays(1);
            DateTime? startDate = null;
            switch (this.StatisticsType)
            {
                case StatisticsType.CurrentDay:
                    startDate = endDate;
                    break;
                case StatisticsType.CurrenWeek:
                    if (endDate.DayOfWeek == DayOfWeek.Monday)
                        startDate = endDate.AddDays(-7);
                    else
                    {
                        startDate = endDate;
                        while (startDate.Value.DayOfWeek != DayOfWeek.Monday)
                            startDate = startDate.Value.AddDays(-1);
                    }
                    break;
                case StatisticsType.CurrentMonth:
                    startDate = new DateTime(endDate.Year, endDate.Month, 1);
                    break;
                case StatisticsType.CurrentYear:
                    startDate = new DateTime(endDate.Year, 1, 1);
                    break;
                case StatisticsType.Definition:
                    if (cb_day.SelectedIndex != 0)
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, cb_month.SelectedIndex, cb_day.SelectedIndex);
                        endDate = startDate.Value.AddDays(1);
                    }
                    else if (cb_month.SelectedIndex != 0)
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, cb_month.SelectedIndex, 1);
                        endDate = startDate.Value.AddMonths(1);
                    }
                    else
                    {
                        startDate = new DateTime((int)cb_year.SelectedValue, 1, 1);
                        endDate = startDate.Value.AddYears(1);
                    }
                    break;
            }
            return (startDate, endDate);
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
                else if(cb_year.SelectedValue is int year)
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
    }
}
