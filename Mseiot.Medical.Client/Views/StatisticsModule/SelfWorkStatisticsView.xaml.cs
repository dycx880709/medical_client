using LiveCharts.Geared;
using MM.Medical.Client.Core;
using Ms.Controls;
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
    /// SelfWorkStatisticsView.xaml 的交互逻辑
    /// </summary>
    public partial class SelfWorkStatisticsView : UserControl
    {
        public SelfWorkStatisticsView()
        {
            InitializeComponent();
            this.Loaded += SelfWorkStatisticsView_Loaded;
        }

        private void SelfWorkStatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TimeSpan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var count = dg_examinations.GetFullCountWithoutScroll();
            var result = loading.AsyncWait("获取数据中,请稍后", SocketProxy.Instance.GetExaminationCountByTime(
                pager.PageIndex,
                count,
                dp_start.SelectedDate,
                dp_end.SelectedDate,
                cb_type.SelectedIndex,
                "",
                CacheHelper.CurrentUser.UserID));
            if (result.IsSuccess) LoadExaminationDatas(result.Content);
            else Alert.ShowMessage(true, AlertType.Error, $"获取数据失败,{ result.Error }");
        }

        private void LoadExaminationDatas(TimeResultCollection content)
        {
            chart.Series.Clear();
            var datas = new GearedValues<TimeResult>(content);
            switch (cb_type.SelectedIndex)
            {
                case 0:
                    chart.Series.Add(new GLineSeries { Values = datas, Stroke = Brushes.Orange });
                    break;
                case 1:
                    chart.Series.Add(new GColumnSeries { Values = datas, Stroke = Brushes.Orange });
                    break;
                case 2:
                    chart.Series.Add(new GCandleSeries { Values = datas, Stroke = Brushes.Orange });
                    break;
            }
        }
    }
}
