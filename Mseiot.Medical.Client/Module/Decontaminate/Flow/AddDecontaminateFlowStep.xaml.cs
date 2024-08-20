using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// AddDecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDecontaminateFlowStep : UserControl
    {
        private readonly DecontaminateFlowStep decontaminateFlowStep;
        private readonly DecontaminateFlowStep decontaminateFlowStep_orgin;
        private readonly Loading loading;

        public RFIDDevice RFIDDevice
        {
            get { return (RFIDDevice)GetValue(RFIDDeviceProperty); }
            set { SetValue(RFIDDeviceProperty, value); }
        }

        public static readonly DependencyProperty RFIDDeviceProperty = DependencyProperty.Register("RFIDDevice", typeof(RFIDDevice), typeof(AddDecontaminateFlowStep), new PropertyMetadata(null));

        public int CleanTime
        {
            get { return (int)GetValue(CleanTimeProperty); }
            set { SetValue(CleanTimeProperty, value); }
        }

        public static readonly DependencyProperty CleanTimeProperty =
            DependencyProperty.Register("CleanTime", typeof(int), typeof(AddDecontaminateFlowStep), new PropertyMetadata(20));

        public ObservableCollection<RFIDDevice> RFIDDevices { get; set; }

        private ICollectionView CollectionView { get { return CollectionViewSource.GetDefaultView(RFIDDevices); } }

        public AddDecontaminateFlowStep(DecontaminateFlowStep decontaminateFlowStep, Loading loading)
        {
            InitializeComponent();
            if (decontaminateFlowStep.Chooses == null)
                decontaminateFlowStep.Chooses = new ObservableCollection<DecontaminateChoose>();
            this.decontaminateFlowStep = decontaminateFlowStep.Copy();
            this.decontaminateFlowStep_orgin = decontaminateFlowStep;
            this.RFIDDevices = new ObservableCollection<RFIDDevice>();
            this.loading = loading;
            LoadRFIDDevices();
            this.DataContext = this.decontaminateFlowStep;
        }

        private void LoadRFIDDevices()
        {
            var result = loading.AsyncWait("获取采集设备中,请稍后", SocketProxy.Instance.GetRFIDDevices());
            this.Dispatcher.Invoke(() =>
            {
                if (result.IsSuccess)
                {
                    RFIDDevices.AddRange(result.Content);
                    if (decontaminateFlowStep.Chooses.Count > 0)
                    {
                        foreach (var choose in decontaminateFlowStep.Chooses)
                        {
                            foreach (var device in RFIDDevices)
                            {
                                if (device.RFIDDeviceID == choose.RFIDDeviceID)
                                {
                                    choose.RFIDDeviceCOM = device.Com;
                                    choose.RFIDDeviceName = device.Name;
                                    break;
                                }
                            }
                        }
                        CollectionView.Filter = t => t is RFIDDevice item && !decontaminateFlowStep.Chooses.Any(s => s.RFIDDeviceID == item.RFIDDeviceID);
                        CollectionView.Refresh();
                    }
                }
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(decontaminateFlowStep.Name))
            {
                Alert.ShowMessage(true, AlertType.Warning, "流程名称不能为空");
                return;
            }
            if (decontaminateFlowStep.Chooses.Count == 0)
            {
                Alert.ShowMessage(true, AlertType.Warning, "流程绑定设备不能为空");
                return;
            }
            if (decontaminateFlowStep.DecontaminateFlowStepID == 0)
            {
                var result = loading.AsyncWait("新建流程步骤中,请稍后", SocketProxy.Instance.AddDecontaminateFlowStep(decontaminateFlowStep));
                if (result.IsSuccess)
                {
                    decontaminateFlowStep.DecontaminateFlowStepID = result.Content;
                    decontaminateFlowStep.CopyTo(decontaminateFlowStep_orgin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "新建保存失败:"+result.Error);
            }
            else
            {
                var result = loading.AsyncWait("编辑流程步骤中,请稍后", SocketProxy.Instance.ModifyDecontaminateFlowStep(decontaminateFlowStep));
                if (result.IsSuccess)
                {
                    decontaminateFlowStep.CopyTo(decontaminateFlowStep_orgin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "编辑保存失败:" + result.Error);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DecontaminateChoose choose)
            {
                decontaminateFlowStep.Chooses.Remove(choose);
                CollectionView.Filter = t => t is DecontaminateChoose item && !decontaminateFlowStep.Chooses.Any(s => s.RFIDDeviceID == item.RFIDDeviceID);
                CollectionView.Refresh();
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            if (RFIDDevice != null)
            {
                if (!decontaminateFlowStep.Chooses.Any(t => t.RFIDDeviceID == RFIDDevice.RFIDDeviceID))
                {
                    decontaminateFlowStep.Chooses.Add(new DecontaminateChoose
                    {
                        RFIDDeviceID = RFIDDevice.RFIDDeviceID,
                        RFIDDeviceName = RFIDDevice.Name,
                        RFIDDeviceCOM = RFIDDevice.Com,
                        Timeout = CleanTime
                    });
                    CollectionView.Filter = t => t is DecontaminateChoose item && !decontaminateFlowStep.Chooses.Any(s => s.RFIDDeviceID == item.RFIDDeviceID);
                    CollectionView.Refresh();
                    RFIDDevice = null;
                    CleanTime = 20;
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, "该设备已在已选列表中");
                }
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Warning, "请选择要添加设备");
            }
        }
    }
}
