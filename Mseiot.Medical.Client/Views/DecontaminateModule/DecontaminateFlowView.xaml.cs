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

namespace MM.Medical.Client.Views
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
            LoadDatas();
            lvDecontaminateFlows.ItemsSource = DecontaminateFlows;
            lvFlowSteps.ItemsSource = DecontaminateFlowSteps;
        }

        #region 流程

        #region 数据

        public ObservableCollection<DecontaminateFlow> DecontaminateFlows { get; set; } = new ObservableCollection<DecontaminateFlow>();

        private async void LoadDatas()
        {
            DecontaminateFlows.Clear();
            var result = await SocketProxy.Instance.GetDecontaminateFlows();
            if (result.IsSuccess)
            {
                if (result.Content != null)
                {
                    DecontaminateFlows.AddRange(result.Content);
                }
            }
        }

        #endregion

        #region 删除、添加、修改

        private void AddFlow_Click(object sender, RoutedEventArgs e)
        {
            AddDecontaminateFlow addDecontaminateFlow = new AddDecontaminateFlow();
            MsWindow.ShowDialog(addDecontaminateFlow);
            LoadDatas();
        }

        private async void RemoveFlow_Click(object sender, RoutedEventArgs e)
        {
            var result = await SocketProxy.Instance.RemoveDecontaminateFlows(DecontaminateFlows.Where(f => f.IsSelected).Select(f => f.DecontaminateFlowID).ToList());
            if (result.IsSuccess)
            {
                LoadDatas();
            }
            else
            {
                MsPrompt.ShowDialog("删除失败");
            }
        }

        private void ModifyFlow_Click(object sender, RoutedEventArgs e)
        {
            DecontaminateFlow decontaminateFlow = (sender as FrameworkElement).Tag as DecontaminateFlow;
            AddDecontaminateFlow addDecontaminateFlow = new AddDecontaminateFlow(decontaminateFlow);
            MsWindow.ShowDialog(addDecontaminateFlow);
            LoadDatas(); 
        }

        #endregion

        #region 选择

        private void AllDecontaminateFlow_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in DecontaminateFlows)
            {
                item.IsSelected = true;
            }
        }


        #endregion

        private void lvDecontaminateFlows_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                var decontaminateFlow = lvDecontaminateFlows.SelectedItem as DecontaminateFlow;
                var result = await SocketProxy.Instance.GetDecontaminateFlowSteps(decontaminateFlow.DecontaminateFlowID);
                if (result.IsSuccess)
                {
                    if (result.Content != null)
                    {
                        DecontaminateFlowSteps.AddRange(result.Content);
                    }
                }
            }
        }

        #endregion

        #region 删除、添加、修改

        private void AddFlowStep_Click(object sender, RoutedEventArgs e)
        {
            if (lvDecontaminateFlows.SelectedItem == null)
            {
                MsPrompt.ShowDialog("请选择流程");
                return;
            }
            DecontaminateFlowStep decontaminateFlowStep = new DecontaminateFlowStep();
            decontaminateFlowStep.DecontaminateFlowID = (lvDecontaminateFlows.SelectedItem as DecontaminateFlow).DecontaminateFlowID;
            AddDecontaminateFlowStep addDecontaminateFlowStep = new AddDecontaminateFlowStep(decontaminateFlowStep);
            MsWindow.ShowDialog(addDecontaminateFlowStep);
            LoadDecontaminateSteps();
        }

        private async void RemoveFlowStep_Click(object sender, RoutedEventArgs e)
        {
            var result = await SocketProxy.Instance.RemoveDecontaminateFlowSteps(DecontaminateFlowSteps.Where(f => f.IsSelected).Select(f => f.DecontaminateFlowStepID).ToList());
            if (result.IsSuccess)
            {
                LoadDecontaminateSteps();
            }
            else
            {
                MsPrompt.ShowDialog("删除失败");
            }
        }

        private void ModifyFlowStep_Click(object sender, RoutedEventArgs e)
        {
            DecontaminateFlowStep decontaminateFlowStep = (sender as FrameworkElement).Tag as DecontaminateFlowStep;
            AddDecontaminateFlowStep addDecontaminateFlow = new AddDecontaminateFlowStep(decontaminateFlowStep);
            MsWindow.ShowDialog(addDecontaminateFlow);
            LoadDecontaminateSteps();
        }

        #endregion

        #region 选择

        private void AllDecontaminateFlowStep_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in DecontaminateFlowSteps)
            {
                item.IsSelected = true;
            }
        }


        #endregion

        #endregion

        
    }
}
