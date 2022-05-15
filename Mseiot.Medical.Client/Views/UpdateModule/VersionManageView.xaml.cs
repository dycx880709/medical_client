using MM.Medical.Client.Core;
using Ms.Controls;
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
using Version = Mseiot.Medical.Service.Entities.Version;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// VersionManageView.xaml 的交互逻辑
    /// </summary>
    public partial class VersionManageView : UserControl
    {
        public VersionManageView()
        {
            InitializeComponent();
            DataContext = this;
            dgDatas.ItemsSource = versions;
            this.Loaded += VersionManageView_Loaded;
        }

        private void VersionManageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= VersionManageView_Loaded;
            LoadDatas();
        }

        #region 数据

        private ObservableCollection<Version> versions = new ObservableCollection<Version>();

        private async void LoadDatas()
        {
            versions.Clear();
            loading.Start("获取版本信息,请稍后");
            bt_version.IsEnabled = false;
            var result = await SocketProxy.Instance.GetVersions();
            this.Dispatcher.Invoke(() =>
            {
                if (result.IsSuccess)
                {
                    if (result.Content != null && result.Content.Count > 0)
                    {
                        versions.AddRange(result.Content);
                        var currentCodes = CacheHelper.ClientVersion.Split('.');
                        bt_version.IsEnabled = result.Content.Any(t =>
                        {
                            var codes = t.VersionCode.Split('.');
                            for (int i = 0; i < codes.Length; i++)
                            {
                                if (Convert.ToInt32(codes[i]) < Convert.ToInt32(currentCodes[i]))
                                {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                    else
                    {
                        bt_version.IsEnabled = true;
                    }
                }
                else
                {
                    Alert.ShowMessage(false, AlertType.Error, result.Error);
                }
                loading.Stop();
            });
        }

        #endregion

        #region 添加、删除、修改 版本

        private void AddVersion_Click(object sender, RoutedEventArgs e)
        {
            var version = new Version();
            AddVersion addVersion = new AddVersion(version, this.loading);
            if (sp.ShowDialog("添加版本", addVersion))
            {
                Alert.ShowMessage(true, AlertType.Success, "版本添加成功");
                LoadDatas();
            }
        }

        private async void RemoveVersion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Version version)
            {
                if (MsPrompt.ShowDialog($"确定删除版本{ version.VersionCode }"))
                {
                    loading.ShowDialog("删除版本中,请稍后");
                    var result = await SocketProxy.Instance.RemoveVersions(new List<int> { version.VersionID });
                    this.Dispatcher.Invoke(() =>
                    {
                        if (result.IsSuccess)
                        {
                            LoadDatas();
                        }
                        else
                        {
                            Alert.ShowMessage(false, AlertType.Error, result.Error);
                        }
                    });
                }
            }
        }

        #endregion
    }
}
