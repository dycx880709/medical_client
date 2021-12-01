using MM.Libs.RFID;
using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Libs.SysLib;
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
        private List<RFIDProxy> rfidProxys;
        public DecontaminateTaskView()
        {
            InitializeComponent();
            this.Loaded += DecontaminateTask_Loaded;
            this.Unloaded += DecontaminateTaskView_Unloaded;
            this.SizeChanged += DecontaminateTaskView_SizeChanged;
            lvTasks.ItemsSource = DecontaminateTasks;
            LoadRFID();
        }

        private void DecontaminateTaskView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GetColumnCount();
            DrawGrid();
        }

        private void DecontaminateTaskView_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Dispose();
            timerCheck.Dispose();
        }

        private void DecontaminateTask_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(GetDatas, null, 0, 3000);
            timerCheck = new Timer(CheckStatus, null, 1000, 1000);
        }

        #region RFID

        AutoResetEvent areDecontaminateTasks = new AutoResetEvent(true);

        private void CheckStatus(object obj)
        {
            timerCheck.Change(Timeout.Infinite, Timeout.Infinite);
            areDecontaminateTasks.WaitOne();
            foreach (var decontaminateTask in DecontaminateTasks)
            {
                if (decontaminateTask.DecontaminateTaskSteps != null)
                {
                    foreach (var decontaminateTaskStep in decontaminateTask.DecontaminateTaskSteps)
                    {
                        if (decontaminateTaskStep.DecontaminateStepStatus == DecontaminateStepStatus.Run)
                        {
                            var timeout = (int)(decontaminateTaskStep.Timeout - (TimeHelper.ToUnixTime(DateTime.Now) - decontaminateTaskStep.StartTime));
                            if (timeout < 0)
                            {
                                timeout = 0;
                            }
                            decontaminateTaskStep.ResidueTime = timeout;
                        }
                        else if (decontaminateTaskStep.DecontaminateStepStatus == DecontaminateStepStatus.Wait)
                        {
                            decontaminateTaskStep.ResidueTime = decontaminateTaskStep.Timeout;
                        }
                    }
                }
              
            }
            areDecontaminateTasks.Set();
            timerCheck.Change(1000, 1000);
        }

        private async void LoadRFID()
        {

            var result = await SocketProxy.Instance.GetRFIDDevices();
            if (result.IsSuccess)
            {
                if (result.Content != null)
                {
                    foreach(var item in result.Content)
                    {
                        RFIDProxy rfidProxy = new RFIDProxy();
                        rfidProxy.Open(item.Com);
                        rfidProxy.NotifyEPCReceived += RfidProxy_NotifyEPCReceived;
                    }
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Alert.ShowMessage(false, AlertType.Error, $"获取内窥镜串口信息失败,{ result.Error }");
                });
            }
        }

        private async void RfidProxy_NotifyEPCReceived(object sender, EPCInfo e)
        {
            var isLoaded = false;
            isLoaded = this.Dispatcher.Invoke(() => isLoaded = this.IsLoaded);
            if (isLoaded)
            {
                areDecontaminateTasks.WaitOne();
                for (int i = 0; i < DecontaminateTasks.Count; i++)
                {
                    var item = DecontaminateTasks[i];
                    if (item.EndoscopeID == e.EPC && item.DecontaminateTaskSteps != null)
                    {
                        item.CleanUserID = CacheHelper.CurrentUser.UserID;
                        if (item.StartTime == 0)
                        {
                            item.StartTime = TimeHelper.ToUnixTime(DateTime.Now);
                        }
                        DecontaminateTaskStep decontaminateTaskStep = item.DecontaminateTaskSteps.FirstOrDefault(f => f.RFIDDeviceSN == e.DeviceID && f.DecontaminateStepStatus != DecontaminateStepStatus.Complete);
                        if (decontaminateTaskStep != null)
                        {
                            if (decontaminateTaskStep.DecontaminateStepStatus == DecontaminateStepStatus.Wait)
                            {
                                decontaminateTaskStep.StartTime = TimeHelper.ToUnixTime(DateTime.Now);
                                decontaminateTaskStep.DecontaminateStepStatus = DecontaminateStepStatus.Run;
                            }
                            else
                            {
                                if (TimeHelper.ToUnixTime(DateTime.Now) - decontaminateTaskStep.StartTime >= decontaminateTaskStep.Timeout)
                                {
                                    decontaminateTaskStep.EndTime = TimeHelper.ToUnixTime(DateTime.Now);
                                    decontaminateTaskStep.DecontaminateStepStatus = DecontaminateStepStatus.Complete;
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        Alert.ShowMessage(false, AlertType.Error, "清洗时间不足,无法结束当前步骤");
                                    });
                                }

                            }
                        }
                        if (item.DecontaminateTaskSteps.Count(f => f.DecontaminateStepStatus == DecontaminateStepStatus.Wait || f.DecontaminateStepStatus == DecontaminateStepStatus.Run) == 0)
                        {
                            item.DecontaminateTaskStatus = DecontaminateTaskStatus.Complete;
                            item.EndTime = TimeHelper.ToUnixTime(DateTime.Now);
                            var result = await SocketProxy.Instance.ChangeDecontaminateTaskStatus(item);
                            if (result.IsSuccess)
                            {

                                this.Dispatcher.Invoke(() =>
                                {
                                    DecontaminateTasks.Remove(item);
                                });

                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Alert.ShowMessage(true, AlertType.Error, result.Error);
                                });
                            }

                        }
                        break;
                    }
                }
                areDecontaminateTasks.Set();
            }
        }


        #endregion

        #region 数据


        Timer timer;
        Timer timerCheck;

        public ObservableCollection<DecontaminateTask> DecontaminateTasks { get; set; } = new ObservableCollection<DecontaminateTask>();

        public async void GetDatas(object obj)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            List<DecontaminateTaskStatus> decontaminateTaskStatuss = new List<DecontaminateTaskStatus> 
            {
                DecontaminateTaskStatus.Wait
            };
            var result = await SocketProxy.Instance.GetDecontaminateTasks(0, 10000, decontaminateTaskStatuss,"",null,null);

            if (result.IsSuccess && result.Content != null && result.Content.Results != null)
            {
                var results = result.Content.Results;
                for (int j = 0; j < results.Count; j++)
                {
                    if (DecontaminateTasks.Count(f => f.DecontaminateTaskID == results[j].DecontaminateTaskID) == 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            results[j].CleanName = CacheHelper.UserName;
                            if (results[j].DecontaminateTaskSteps != null)
                            {
                                for (int i = 0; i < results[j].DecontaminateTaskSteps.Count; i++)
                                {
                                    results[j].DecontaminateTaskSteps[i].Index = i + 1;
                                }
                            }
                          
                            DecontaminateTasks.Insert(0, results[j]);
                        });
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

        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(double), typeof(DecontaminateTaskView), new PropertyMetadata(0.0));

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register("RowHeight", typeof(double), typeof(DecontaminateTaskView), new PropertyMetadata(0.0));


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
