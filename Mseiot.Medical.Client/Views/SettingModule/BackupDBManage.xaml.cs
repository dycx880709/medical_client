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
using MM.Libs.RFID;
using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// SQLInfoManage.xaml 的交互逻辑
    /// </summary>
    public partial class BackupDBManage : UserControl
    {
        public BackupDBManage()
        {
            InitializeComponent();
            this.Loaded += SQLInfoManage_Loaded;
        }

        private void SQLInfoManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SQLInfoManage_Loaded;
            lvDatas.ItemsSource = SQLInfos;
            LoadSQLInfos();
        }


        #region 数据

        public ObservableCollection<SQLInfo> SQLInfos { get; set; } = new ObservableCollection<SQLInfo>();

        private async void LoadSQLInfos()
        {
            SQLInfos.Clear();
            loading.Start("获取备份列表中,请稍后");
            var result = await SocketProxy.Instance.GetDBRecords();
            if (result.IsSuccess)
                SQLInfos.AddRange(result.Content);
            loading.Stop();
        }

        #endregion

        #region 添加、删除、修改


        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmWindow.Show("是否继续?"))
            {
                if (sender is FrameworkElement element && element.DataContext is SQLInfo sqlInfo)
                {
                    var result = loading.AsyncWait("删除备份中,请稍后", SocketProxy.Instance.RemoveDBRecords(new List<string> { sqlInfo.Path }));
                    if (result.IsSuccess)
                        LoadSQLInfos();
                    else Alert.ShowMessage(true, AlertType.Error, $"删除备份失败,{ result.Error }");
                }
            }
        }

        #endregion

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            SQLInfo sqlInfo=(sender as FrameworkElement).DataContext as SQLInfo;
            if (sqlInfo != null)
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "数据库备份文件（*.sql）|*.sql";
                sfd.FilterIndex = 1;

                sfd.RestoreDirectory = true;
                sfd.FileName = sqlInfo.Path;

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    loading.Start("正在下载备份文件");
                    var result = await SocketProxy.Instance.DownloadDBRecord(sqlInfo.Path,sfd.FileName);
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"下载成功");
                    }
                    else
                    {
                        Alert.ShowMessage(false, AlertType.Error, $"下载失败");
                    }
                    loading.Stop();
                }
            }
        }
    }
}
