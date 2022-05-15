using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// DecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class DecontaminateFlowView : UserControl
    {

        public DecontaminateFlowView()
        {
            InitializeComponent();
            this.Loaded += DecontaminateFlowView_Loaded;
        }

        private void DecontaminateFlowView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateFlowView_Loaded;
            lvDecontaminateFlows.ItemsSource = DecontaminateFlows;
            lvFlowSteps.ItemsSource = DecontaminateFlowSteps;
            this.LoadDecontaminateFlows();
        }

        #region 流程

        #region 数据
        public ObservableCollection<DecontaminateFlow> DecontaminateFlows { get; set; } = new ObservableCollection<DecontaminateFlow>();

        private void LoadDecontaminateFlows()
        {
            DecontaminateFlows.Clear();
            var result = loading.AsyncWait("获取流程列表中,请稍后", SocketProxy.Instance.GetDecontaminateFlows());
            if (result.IsSuccess) 
                DecontaminateFlows.AddRange(result.Content);
            else 
                Alert.ShowMessage(true, AlertType.Error, $"获取流程列表失败,{ result.Error }");
            loading.Stop();
            this.UpdateLayout();
            if (lvDecontaminateFlows.Items.Count > 0)
            {
                lvDecontaminateFlows.SelectedIndex = 0;
            }
        }

        #endregion

        #region 删除、添加、修改

        private void AddFlow_Click(object sender, RoutedEventArgs e)
        {
            var decontaminateFlow = new DecontaminateFlow();
            var addDecontaminateFlow = new AddDecontaminateFlow(decontaminateFlow, this.loading);
            if (child.ShowDialog("新建流程", addDecontaminateFlow))
                this.LoadDecontaminateFlows();
        }

        private void RemoveFlow_Click(object sender, RoutedEventArgs e)
        {
            if (MsPrompt.ShowDialog("是否继续?"))
            {
                if (sender is FrameworkElement element && element.DataContext is DecontaminateFlow decontaminateFlow)
                {
                    var result = loading.AsyncWait("删除流程列表中,请稍后", SocketProxy.Instance.RemoveDecontaminateFlows(new List<int> { decontaminateFlow.DecontaminateFlowID }));
                    if (result.IsSuccess) LoadDecontaminateFlows();
                    else Alert.ShowMessage(true, AlertType.Error, $"删除失败,{ result.Error }");
                }
            }
        }

        private void ModifyFlow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DecontaminateFlow decontaminateFlow)
            {
                var addDecontaminateFlow = new AddDecontaminateFlow(decontaminateFlow, this.loading);
                if (child.ShowDialog("修改流程", addDecontaminateFlow))
                    this.LoadDecontaminateFlows();
            }
        }

        #endregion

        #region 选择

        private void AllDecontaminateFlow_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in DecontaminateFlows)
                item.IsSelected = true;
        }


        #endregion

        private void DecontaminateFlows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDecontaminateSteps();
        }

        #endregion

        #region 流程步骤

        #region 数据

        public ObservableCollection<DecontaminateFlowStep> DecontaminateFlowSteps { get; set; } = new ObservableCollection<DecontaminateFlowStep>();

        private async void LoadDecontaminateSteps()
        {
            DecontaminateFlowSteps.Clear();
            if (lvDecontaminateFlows.SelectedItem != null)
            {
                loading.Start("获取流程步骤中,请稍后");
                var decontaminateFlow = lvDecontaminateFlows.SelectedItem as DecontaminateFlow;
                var result = await SocketProxy.Instance.GetDecontaminateFlowSteps(decontaminateFlow.DecontaminateFlowID);
                if (result.IsSuccess) DecontaminateFlowSteps.AddRange(result.Content);
                else Alert.ShowMessage(true, AlertType.Error, $"获取流程步骤失败,{ result.Error }");
                loading.Stop();
            }
        }

        #endregion

        #region 删除、添加、修改

        private void AddFlowStep_Click(object sender, RoutedEventArgs e)
        {
            if (lvDecontaminateFlows.SelectedItem == null)
            {
                Alert.ShowMessage(true, AlertType.Warning, "请选择流程");
                return;
            }
            var decontaminateFlowStep = new DecontaminateFlowStep { DecontaminateFlowID = (lvDecontaminateFlows.SelectedItem as DecontaminateFlow).DecontaminateFlowID };
            var addDecontaminateFlowStep = new AddDecontaminateFlowStep(decontaminateFlowStep, this.loading);
            if (child.ShowDialog("新增流程步骤", addDecontaminateFlowStep))
                LoadDecontaminateSteps();
        }

        private void RemoveFlowStep_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DecontaminateFlowStep decontaminateFlowStep)
            {
                if (MsPrompt.ShowDialog("是否继续"))
                {
                    var result = loading.AsyncWait("删除流程步骤中,请稍后", SocketProxy.Instance.RemoveDecontaminateFlowSteps(new List<int> { decontaminateFlowStep.DecontaminateFlowStepID }));
                    if (result.IsSuccess) LoadDecontaminateSteps();
                    else Alert.ShowMessage(true, AlertType.Error, $"流程步骤删除失败,{ result.Error }");
                }
            }
        }

        private void ModifyFlowStep_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DecontaminateFlowStep decontaminateFlowStep)
            {
                var addDecontaminateFlow = new AddDecontaminateFlowStep(decontaminateFlowStep, this.loading);
                if (child.ShowDialog("修改流程步骤", addDecontaminateFlow))
                    LoadDecontaminateSteps();
            }
        }

        #endregion

        #region 选择

        private void AllDecontaminateFlowStep_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in DecontaminateFlowSteps)
                item.IsSelected = true;
        }
        

        #endregion

        #endregion
    }
}
