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

namespace MM.Medical.Decontaminate.Views.Decontaminate
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
    }
}
