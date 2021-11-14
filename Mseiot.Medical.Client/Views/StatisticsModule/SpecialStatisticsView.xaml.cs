using Ms.Libs.SysLib;
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


        public SpecialStatisticsView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ChartType_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
