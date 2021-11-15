using LiveCharts;
using LiveCharts.Wpf;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
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
using System.Windows.Shapes;
using Separator = LiveCharts.Wpf.Separator;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// TextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextWindow : Window
    {
        public Func<double, string> Formatter { get; set; } = t =>
        {
            var dateTime = TimeHelper.FromUnixTime(Convert.ToInt64(t));
            var format = dateTime.ToShortDateString();
            return format;
        };

        public ChartType ChartType
        {
            get { return (ChartType)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }
        public static readonly DependencyProperty ChartTypeProperty = DependencyProperty.Register("ChartType", typeof(ChartType), typeof(SelfWorkStatisticsView), new PropertyMetadata(ChartType.LineSeries));

        public TextWindow()
        {
            InitializeComponent();
            this.Loaded += TextWindow_Loaded;
            this.DataContext = this;
        }
        private void TextWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var datas = new List<TimeResult>()
            {
                new TimeResult
                {
                    TimeStamp = (int)TimeHelper.ToUnixTime(DateTime.Now.AddDays(-1)),
                    Count = 1
                },
                new TimeResult
                {
                    TimeStamp = (int)TimeHelper.ToUnixTime(DateTime.Now),
                    Count = 5
                },
                new TimeResult
                {
                    TimeStamp = (int)TimeHelper.ToUnixTime(DateTime.Now.AddDays(1)),
                    Count = 3
                }
            };
            var xValues = new ChartValues<string>(datas.Select(t => t.TimeStamp.ToString()));
            var yValues = new ChartValues<double>(datas.Select(t => (double)t.Count));
            var values = new ChartValues<TimeResult>(datas);
            axisY.MaxValue = datas.Max(t => t.Count);
            axisY.MinValue = 0;
            switch (this.ChartType)
            {
                case ChartType.LineSeries:
                    axisX.Labels = null;
                    axisX.LabelFormatter = Formatter;
                    axisX.Separator = new Separator { Step = 24 * 60 * 60 };
                    chart.Series.Add(new LineSeries { Values = values });
                    break;
                case ChartType.Bar:
                    axisX.LabelFormatter = null;
                    axisX.Separator = null;
                    axisX.Labels = new ChartValues<string>(datas.Select(t => TimeHelper.FromUnixTime(t.TimeStamp).ToShortDateString()));
                    chart.Series.Add(new ColumnSeries { Values = yValues });
                    break;
            }
            //var yStep = (int)((axisY.MaxValue - axisY.MinValue) / yValues.Count);
            //if (yStep == 0) yStep = 1;
            //axisY.Separator = new LiveCharts.Wpf.Separator { Step = yStep };
        }

        private void ChartType_Click(object sender, RoutedEventArgs e)
        {
            if (chart.Series.Count > 0 && chart.Series[0] is Series series)
            {
                chart.Series.Clear();
                if (series.Values is ChartValues<double> yValues && chart.AxisX[0].Labels is IList<string> xValues)
                {

                }
                //switch (this.ChartType)
                //{
                //    case ChartType.LineSeries:
                //        chart.Series.Add(new LineSeries { Values = yValues });
                //        break;
                //    case ChartType.Bar:
                //        chart.Series.Add(new ColumnSeries { Values = yValues });
                //        break;
                //}
            }
        }
    }
}
