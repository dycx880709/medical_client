using Mseiot.Medical.Service.Entities;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// DecontaminateTask.xaml 的交互逻辑
    /// </summary>
    public partial class DecontaminateTaskView : UserControl
    {
        public DecontaminateTaskView()
        {
            InitializeComponent();
            this.Loaded += DecontaminateTask_Loaded;
        }

        private void DecontaminateTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateTask_Loaded;
            lvTasks.ItemsSource = DecontaminateTasks;
            CreateTestDatas();
        }

        #region 数据

        public ObservableCollection<DecontaminateTask> DecontaminateTasks { get; set; } = new ObservableCollection<DecontaminateTask>();

        public void CreateTestDatas()
        {
            for(int i = 0; i < 10; i++)
            {
                DecontaminateTask decontaminateTask = new DecontaminateTask
                {
                    DecontaminateTaskID = i,
                    EndoscopeID = i,
                    EndTime = 0,
                    StartTime = 0,
                    UserID = i
                };
                for(int j = 0; j < 5; j++)
                {
                    decontaminateTask.DecontaminateSteps.Add(new DecontaminateStep
                    {
                        DecontaminateStepID = j,
                        Name = "步骤" + j.ToString(),
                        DecontaminateStepStatus=(DecontaminateStepStatus)j
                    });
                }
                DecontaminateTasks.Add(decontaminateTask);
            }
        }

        #endregion

        #region 列数


        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(DecontaminateTaskView), new PropertyMetadata(0));


        private void lvTasks_Loaded(object sender, RoutedEventArgs e)
        {
            ColumnCount = (int)(Math.Floor(lvTasks.ActualWidth / 120));

        }

        #endregion+
    }
}
