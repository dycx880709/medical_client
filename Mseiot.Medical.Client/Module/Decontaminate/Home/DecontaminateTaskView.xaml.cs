using MM.Medical.Client.Core;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MM.Medical.Client.Module.Decontaminate
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
            this.SizeChanged += DecontaminateTaskView_SizeChanged;
        }

        private void DecontaminateTaskView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GetColumnCount();
            DrawGrid();
            Test();
        }

        private void DecontaminateTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateTask_Loaded;
           
            lvTasks.ItemsSource = DecontaminateTasks;
            timer = new Timer(GetDatas, null, 0, 3000);
        }

        #region 数据

        private void Test()
        {
              int iSeed = 10;
            Random ro = new Random(10);
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            for (int i = 0; i < 50; i++)
            {
                DecontaminateTask decontaminateTask = new DecontaminateTask();
                decontaminateTask.DecontaminateSteps =new List<DecontaminateStep>();

              

                int iResult;
                int iUp = 7;
                iResult = ro.Next(1,iUp);

                for (int j = 0; j < iResult; j++)
                {
                    var decontaminateStep = new DecontaminateStep
                    {
                      Index=j+1
                    };

                    iResult = ro.Next(1,iUp);
                    if (iResult <= 1)
                    {
                        decontaminateStep.DecontaminateStepStatus = DecontaminateStepStatus.Wait;
                        decontaminateStep.Name = "初洗";
                    }
                    else if (iResult <= 4)
                    {
                        decontaminateStep.DecontaminateStepStatus = DecontaminateStepStatus.Normal;
                        decontaminateStep.Name = "酶洗";
                    }
                    else 
                    {
                        decontaminateStep.DecontaminateStepStatus = DecontaminateStepStatus.Complete;
                        decontaminateStep.Name = "烘干";
                    }
                    decontaminateTask.DecontaminateSteps.Add(decontaminateStep);
                }
                DecontaminateTasks.Add(decontaminateTask);
            }
        }

        Timer timer;

        public ObservableCollection<DecontaminateTask> DecontaminateTasks { get; set; } = new ObservableCollection<DecontaminateTask>();

        public async void GetDatas(object obj)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            List<DecontaminateTaskStatus> decontaminateTaskStatuss = new List<DecontaminateTaskStatus> {
                DecontaminateTaskStatus.Complete,
                DecontaminateTaskStatus.Wait
            };
            var result = await SocketProxy.Instance.GetDecontaminateTasks(decontaminateTaskStatuss);

            if (result.IsSuccess && result.Content != null)
            {
                for (int j = 0; j < result.Content.Count; j++)
                {
                    if (DecontaminateTasks.Count(f => f.DecontaminateTaskID == result.Content[j].DecontaminateTaskID) == 0)
                    {
                        DecontaminateTasks.Insert(0, result.Content[j]);
                    }
                }
            }

            timer.Change(3000, 3000);
        }

        #endregion

        #region 列数

        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(DecontaminateTaskView), new PropertyMetadata(0.0));

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(DecontaminateTaskView), new PropertyMetadata(0.0));


        private int rowCount;
        

        private void GetColumnCount()
        {
            ColumnWidth = (lvDrawGrid.ActualWidth-5-150) / (double)7.0;

            rowCount = (int)(Math.Floor(lvDrawGrid.ActualHeight / 120));
            RowHeight =  lvDrawGrid.ActualHeight / (double)rowCount;
        }

        #endregion

        #region GRID


        private void DrawGrid()
        {
            List<int> drawGrids = new List<int>();
            for (int i = 0; i < rowCount; i++)
            {
                drawGrids.Add(i);
            }
            lvDrawGrid.ItemsSource = drawGrids;
        }

        #endregion
    }
}
