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
        }

        private void DecontaminateTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateTask_Loaded;
            lvTasks.ItemsSource = DecontaminateTasks;
            timer = new Timer(GetDatas, null, 0, 3000);
        }

        #region 数据

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

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(DecontaminateTaskView), new PropertyMetadata(0));

        private void Tasks_Loaded(object sender, RoutedEventArgs e)
        {
            ColumnCount = (int)(Math.Floor(lvTasks.ActualWidth / 120));
        }

        #endregion
    }
}
