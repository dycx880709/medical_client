using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using MM.Libs.RFID;
using MM.Medical.Client.Core;
using MM.Medical.Client.Entities;
using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// DecontaminateTaskManage.xaml 的交互逻辑
    /// </summary>
    public partial class DecontaminateTaskManage : UserControl
    {
        public DecontaminateTaskManage()
        {
            InitializeComponent();
            this.Loaded += DecontaminateTaskManage_Loaded;
        }

        private void DecontaminateTaskManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateTaskManage_Loaded;
            pager.PageChanged += Pager_PageChanged;
            lvDatas.ItemsSource = DecontaminateTasks;
            var startTime = TimeHelper.ToUnixDate(DateTime.Now);
            var today = TimeHelper.FromUnixTime(startTime);
            dti.StartTime = today.AddDays(-7);
            dti.EndTime = today.AddDays(1);
            LoadDecontaminateTasks();
        }

        #region 数据
        AutoResetEvent areDecontaminateTask = new AutoResetEvent(true);
        public ObservableCollection<DecontaminateTask> DecontaminateTasks { get; set; } = new ObservableCollection<DecontaminateTask>();

        private async void LoadDecontaminateTasks()
        {
            areDecontaminateTask.WaitOne();
            pager.PageChanged -= Pager_PageChanged;
            pager.SelectedCount = lvDatas.GetFullCountWithoutScroll();
            DecontaminateTasks.Clear();
            loading.Start("获取内窥镜列表中,请稍后");
            var result = await SocketProxy.Instance.GetDecontaminateTasks(
                pager.PageIndex,
                pager.SelectedCount,
                new List<DecontaminateTaskStatus>() { DecontaminateTaskStatus.Complete },
                tbSearch.Text,
                dti.StartTime,
                dti.EndTime
            );
            this.Dispatcher.Invoke(() =>
            {
                if (result.IsSuccess)
                {
                    DecontaminateTasks.AddRange(result.Content.Results);
                    pager.TotalCount = result.Content.Total;
                }
                loading.Stop();
            });
            pager.PageChanged += Pager_PageChanged;
            areDecontaminateTask.Set();
        }

        #endregion

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadDecontaminateTasks();
        }

        private void Pager_PageChanged(object sender, PageChangedEventArgs args)
        {
            LoadDecontaminateTasks();
        }

        private async void Excel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = System.IO.Path.Combine(dialog.SelectedPath, $"清洗记录_{ TimeHelper.ToUnixTime(DateTime.Now) }.xls");
                loading.Start("导出清洗数据中,请稍后");
                var result = await SocketProxy.Instance.GetDecontaminateTasks(
                    0,
                    10000,
                    new List<DecontaminateTaskStatus>() { DecontaminateTaskStatus.Complete },
                    "",
                    dti.StartTime,
                    dti.EndTime
                );
                if (!result.IsSuccess)
                    this.Dispatcher.Invoke(() => Alert.ShowMessage(false, AlertType.Error, $"导出清洗数据失败,{ result.Error }"));
                else
                {
                    var excelDatas = result.Content.Results.Select(t => new DecontaminateTaskExcel(t));
                    var result2 = ExcelHelper.DataListToExcel(filePath, excelDatas);
                    if (result2.Item1)
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Success, "导出清洗数据完成"));
                    else
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(false, AlertType.Error, "导出清洗数据到Excel失败"));
                }
                this.Dispatcher.Invoke(() => loading.Stop());
            }
        }
    }
}
