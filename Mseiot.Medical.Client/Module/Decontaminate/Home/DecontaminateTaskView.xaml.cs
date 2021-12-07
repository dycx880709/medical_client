using MM.Libs.RFID;
using MM.Medical.Client.Core;
using MM.Medical.Client.Entities;
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
        private List<RFIDExProxy> proxys;
        private List<RFIDDevice> devices;
        public DecontaminateTaskView()
        {
            InitializeComponent();
            this.Loaded += DecontaminateTask_Loaded;
            this.Unloaded += DecontaminateTaskView_Unloaded;
            this.SizeChanged += DecontaminateTaskView_SizeChanged;
            lvTasks.ItemsSource = DecontaminateTasks;
            this.proxys = new List<RFIDExProxy>();
            this.devices = new List<RFIDDevice>();
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
            StopRFIDs();
        }

        private void DecontaminateTask_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new Timer(GetDatas, null, 0, 3000);
            timerCheck = new Timer(CheckStatus, null, 1000, 1000);
            StartRFIDs();
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

        private void StopRFIDs()
        {
            if (proxys.Count > 0)
            {
                foreach (var proxy in proxys)
                {
                    proxy.Close();
                }
                proxys.Clear();
            }
            devices.Clear();
        }

        private async void StartRFIDs()
        {

            var result = await SocketProxy.Instance.GetRFIDDevices();
            if (result.IsSuccess)
            {
                proxys.Clear();
                if (result.Content != null && result.Content.Count > 0)
                {
                    this.AddCom(result.Content.ToArray());
                    this.devices = result.Content;
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

        private void AddCom(params RFIDDevice[] devcices)
        {
            foreach (var devcice in devcices)
            {
                var proxy = proxys.FirstOrDefault(t => t.Com.Equals(devcice.Com));
                if (proxy == null)
                {
                    proxy = new RFIDExProxy { Com = devcice.Com };
                    proxy.NotifyEPCReceived += (s, e) =>
                    {
                        if (s is RFIDExProxy p)
                        {
                            RfidProxy_NotifyEPCReceived(p, e);
                        }
                    };
                    proxy.NotifyDeviceStatusChanged += (sender, status) =>
                    {
                        if (sender is RFIDExProxy rfid)
                        {
                            if (status)
                                LogHelper.Instance.Error($"串口{ rfid.Com }连接失败");
                            else
                                LogHelper.Instance.Info($"串口{ rfid.Com }连接成功");
                        }
                    };
                    proxy.Open(devcice.Com);
                    proxys.Add(proxy);
                }
            }
        }

        private User currentClearUser;

        private async void RfidProxy_NotifyEPCReceived(RFIDExProxy proxy, EPCInfo e)
        {
            this.Dispatcher.Invoke(() => loading.Start("数据读取中,请稍后"));
            areDecontaminateTasks.WaitOne();
            await ExecuteClearTask(proxy.Com, e);
            areDecontaminateTasks.Set();
            this.Dispatcher.Invoke(() => loading.Stop());
        }


        private async Task ExecuteClearTask(string com, EPCInfo info)
        {
            if (info.EPC < 100000000) //清洗内镜
            {
                var decontaminateTask = DecontaminateTasks.FirstOrDefault(t => t.EndoscopeID == info.EPC);
                if (decontaminateTask != null)
                {
                    if (string.IsNullOrEmpty(decontaminateTask.CleanUserID))
                    {
                        if (currentClearUser != null)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                decontaminateTask.CleanUserID = currentClearUser.UserID;
                                decontaminateTask.CleanName = currentClearUser.Name;
                            });
                            currentClearUser = null;
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, "未设置清洗人员"));
                            return;
                        }
                    }
                    if (decontaminateTask.DecontaminateTaskSteps == null || decontaminateTask.DecontaminateTaskSteps.Count == 0)
                    {
                        decontaminateTask.StartTime = TimeHelper.ToUnixTime(DateTime.Now);
                        var res2 = await SocketProxy.Instance.ChangeDecontaminateTaskStatus(decontaminateTask);
                        if (res2.IsSuccess)
                        {
                            var res1 = await SocketProxy.Instance.AddDecontaminateTaskSteps(decontaminateTask.DecontaminateTaskID, com);
                            if (res1.IsSuccess)
                            {
                                for (int i = 0; i < res1.Content.Count; i++)
                                {
                                    res1.Content[i].Index = i+1;
                                }
                                this.Dispatcher.Invoke(() => decontaminateTask.DecontaminateTaskSteps = res1.Content);
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"获取清洗流程失败,{ res1.Error }"));
                                return;
                            }
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"修改清洗任务状态失败,{ res2.Error }"));
                            return;
                        }
                        
                    }
                    var firstStep = decontaminateTask.DecontaminateTaskSteps.FirstOrDefault(t => t.DecontaminateStepStatus != DecontaminateStepStatus.Complete && t.RFIDDeviceCom.Equals(com));
                    if (firstStep != null)
                    {
                        var condition = firstStep.Copy();
                        if (condition.DecontaminateStepStatus == DecontaminateStepStatus.Wait)
                        {
                            condition.DecontaminateStepStatus = DecontaminateStepStatus.Run;
                            condition.StartTime = TimeHelper.ToUnixTime(DateTime.Now);
                        }
                        else
                        {
                            if (TimeHelper.ToUnixTime(DateTime.Now) - condition.StartTime < condition.Timeout)
                            {
                                this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"流程时间未达到最低限制"));
                                return;
                            }
                            else
                            {
                                condition.DecontaminateStepStatus = DecontaminateStepStatus.Complete;
                                condition.ResidueTime = 0;
                                condition.EndTime = TimeHelper.ToUnixTime(DateTime.Now);
                            }
                        }
                        var result = await SocketProxy.Instance.ChangeDecontaminateTaskStepStatus(condition);
                        if (result.IsSuccess)
                        {
                            this.Dispatcher.Invoke(() => condition.CopyTo(firstStep));
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"更新清洗流程失败,{ result.Error }"));
                            return;
                        }
                    }
                    if (decontaminateTask.DecontaminateTaskSteps.All(t => t.DecontaminateStepStatus == DecontaminateStepStatus.Complete))
                    {
                        decontaminateTask.DecontaminateTaskStatus = DecontaminateTaskStatus.Complete;
                        decontaminateTask.EndTime = TimeHelper.ToUnixTime(DateTime.Now);
                        var result = await SocketProxy.Instance.ChangeDecontaminateTaskStatus(decontaminateTask);
                        this.Dispatcher.Invoke(() =>
                        {
                            if (result.IsSuccess)
                            {
                                DecontaminateTasks.Remove(decontaminateTask);
                            }
                            else
                            {
                                Alert.ShowMessage(true, AlertType.Error, result.Error);
                            }
                        });
                    }
                }
            }
            else if (info.EPC > 100000000 && info.EPC < 200000000) //清洗人员
            {
                var result = await SocketProxy.Instance.GetUserById((info.EPC - 100000000).ToString());
                this.Dispatcher.Invoke(() =>
                {
                    if (!result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"获取清洗人员数据失败,{ result.Error }");
                    }
                    else
                    {
                        if (currentClearUser == null || currentClearUser.UserID.Equals(result.Content.UserID))
                        {
                            currentClearUser = result.Content;
                            Alert.ShowMessage(true, AlertType.Success, $"清理人员{ currentClearUser.Name }准备就绪");
                        }
                        else if (!currentClearUser.UserID.Equals(result.Content.UserID))
                        {
                            Alert.ShowMessage(true, AlertType.Warning, $"清理人员{ currentClearUser.Name }已被{ result.Content.Name }替换");
                        }
                    }
                });
            }
            else
            {
                this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, "错误的读卡信息"));
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
            List<DecontaminateTaskStatus> decontaminateTaskStatuss = new List<DecontaminateTaskStatus> { DecontaminateTaskStatus.Wait };
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
