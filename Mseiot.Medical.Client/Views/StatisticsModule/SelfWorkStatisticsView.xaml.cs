using LiveCharts;
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
using System.Windows;
using System.Windows.Controls;

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
       
        public Func<double, string> Formatter { get; set; } = t => TimeHelper.FromUnixTime(Convert.ToInt64(t)).ToShortDateString();

        public SelfWorkStatisticsView()
        {
            InitializeComponent();
            this.Loaded += SelfWorkStatisticsView_Loaded;
            this.DataContext = this;
        }

        private void SelfWorkStatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SelfWorkStatisticsView_Loaded;
            dg_examinations.LoadingRow += (o, ex) => ex.Row.Header = ex.Row.GetIndex() + 1;
            GetDateDatas();
        }

        private void ChartType_Click(object sender, RoutedEventArgs e)
        {
            if (chart.Series.Count > 0 && chart.Series[0] is Series series && series.Values is IList<TimeResult> datas)
                ReloadChart(datas);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var timeInterval = GetStartEndTime();
            var statisticsType = this.StatisticsType;
            if (statisticsType == StatisticsType.Definition)
            {
                if (cb_day.SelectedIndex > 0)
                    statisticsType = StatisticsType.CurrentDay;
                else if (cb_month.SelectedIndex > 0)
                    statisticsType = StatisticsType.CurrentMonth;
                else
                    statisticsType = StatisticsType.CurrentYear;
            }
            var result = loading.AsyncWait("获取数据中,请稍后", SocketProxy.Instance.GetExaminationCountByTime(
                timeInterval.Item1,
                timeInterval.Item2,
                statisticsType,
                doctorID: CacheHelper.CurrentUser.UserID));
            if (result.IsSuccess)
            {
                if (result.Content.Count > 0)
                {
                    ReloadChart(result.Content);
                    GetExaminationDatas(timeInterval.Item1, timeInterval.Item2);
                }
                else Alert.ShowMessage(true, AlertType.Warning, $"查询无数据");
            }
            else Alert.ShowMessage(true, AlertType.Error, $"获取数据失败,{ result.Error }");
        }
        private void Pager_PageChanged(object sender, PageChangedEventArgs args)
        {
            var timeInterval = GetStartEndTime();
            GetExaminationDatas(timeInterval.Item1, timeInterval.Item2);
        }

        private void GetExaminationDatas(DateTime? startTime, DateTime? endTime)
        {
            pager.SelectedCount = dg_examinations.GetFullCountWithoutScroll();
            var result2 = loading.AsyncWait("获取数据中,请稍后", SocketProxy.Instance.GetExaminations(
              pager.PageIndex,
              pager.SelectedCount,
              startTime,
              endTime,
              doctorID: CacheHelper.CurrentUser.UserID));
            if (result2.IsSuccess)
            {
                pager.TotalCount = result2.Content.Total;
                dg_examinations.ItemsSource = result2.Content.Results;
            }
        }

        private void ReloadChart(IList<TimeResult> datas)
        {
            axisX.ShowLabels = false;
            axisY.ShowLabels = false;
            chart.Series.Clear();
            if (datas.Count > 0)
            {
                axisX.ShowLabels = true;
                axisY.ShowLabels = true;
                switch (this.StatisticsType)
                {
                    case StatisticsType.CurrentMonth:
                    case StatisticsType.CurrenWeek:
                    case StatisticsType.CurrentDay:
                        axisX.Labels = new ChartValues<string>(datas.Select(t => TimeHelper.FromUnixTime(t.TimeStamp).ToShortDateString()));
                        break;
                    case StatisticsType.CurrentQuarter:
                    case StatisticsType.CurrentYear:
                        axisX.Labels = new ChartValues<string>(datas.Select(t => $"{ TimeHelper.FromUnixTime(Convert.ToInt64(t.TimeStamp)).Month }月"));
                        break;
                    case StatisticsType.Definition:
                        if (cb_day.SelectedIndex > 0 || cb_month.SelectedIndex > 0)
                            axisX.Labels = new ChartValues<string>(datas.Select(t => TimeHelper.FromUnixTime(t.TimeStamp).ToShortDateString()));
                        else
                            axisX.Labels = new ChartValues<string>(datas.Select(t => $"{ TimeHelper.FromUnixTime(Convert.ToInt64(t.TimeStamp)).Month }月"));
                        break;
                }
                axisY.MaxValue = datas.Count > 0 ? datas.Max(t => t.Count) : 0;
                axisY.MinValue = 0;
                var yValues = new ChartValues<double>(datas.Select(t => (double)t.Count));
                var yStep = (int)((axisY.MaxValue - axisY.MinValue) / yValues.Count);
                if (yStep == 0) yStep = 1;
                axisY.Separator = new LiveCharts.Wpf.Separator { Step = yStep };
                switch (this.ChartType)
                {
                    case ChartType.LineSeries:
                        chart.Series.Add(new LineSeries { Values = yValues });
                        break;
                    case ChartType.Bar:
                        chart.Series.Add(new ColumnSeries { Values = yValues, Title = "" });
                        break;
                    case ChartType.Pie:
                        chart.Series.Add(new PieSeries { Values = yValues });
                        break;
                }
            }
        }

        private (DateTime?, DateTime?) GetStartEndTime()
        {
            var endDate = DateTime.Now.Date.AddDays(1);
            DateTime? startDate = null;
            switch (this.StatisticsType)
            {
                case StatisticsType.CurrentDay:
                    startDate = endDate.AddDays(-1);
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
            for (int i = 2021; i <= DateTime.Now.Year; i++)
                years.Add(i);
            cb_year.ItemsSource = years;
            cb_year.SelectedIndex = 0;
            cb_month.ItemsSource = new string[] { "全部", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
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
                }
                cb_day.SelectedIndex = 0;
            };
            cb_month.SelectedIndex = 0;
        }
    }
}
