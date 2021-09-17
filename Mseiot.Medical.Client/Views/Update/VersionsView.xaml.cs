using Ms.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// VersionsView.xaml 的交互逻辑
    /// </summary>
    public partial class VersionsView : UserControl, IDisposable
    {
        #region 类成员
        public ObservableCollection<Version> versions { get; set; } = new ObservableCollection<Version>();
        #endregion

        #region 构造器
        public VersionsView()
        {
            InitializeComponent();
            this.Loaded += UpdateManager_Loaded;
        }

        private void UpdateManager_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UpdateManager_Loaded;
            //dgVersions.ItemsSource = versions;

            CollectionViewSource.GetDefaultView(versions).SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
            LoadDatas();
            DataContext = this;
        }
        #endregion

        #region 获取数据

        private async void LoadDatas()
        {
            loading.Start(LanguageHelper.Instance.GetStrResource("Str_GettingData"));
            var result = await SocketProxy.Instance.GetVersions();
            if (result.IsSuccess)
            {
                result.Content.ForEach(f =>
                {
                    if (f.VersionType == VersionTypes.Watrix)
                        versions.Add(f);
                });
                recordExpanderList.Clear();
                loading.Stop();
            }
            else
            {
                loading.Stop();
                MsWindow.ShowDialog(result.Error, LanguageHelper.Instance.GetStrResource("Str_FailToLoadVer"));
            }
            CollectionViewSource.GetDefaultView(versions).Refresh();
        }

        #endregion

        #region 事件
        //添加
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            AddVersion addVersion = new AddVersion(loading);
            if (await slidePanel.Open(addVersion))
            {
                versions.Add(addVersion.Version);
            }
        }
        //删除
        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            Version version = (sender as FrameworkElement).Tag as Version;
            if (!MsPrompt.ShowDialog(LanguageHelper.Instance.GetStrResource("Str_ConfitmDeleteVer"))) return;

            var result = await SocketProxy.Instance.RemoveVersions(new System.Collections.Generic.List<string> { version.VersionID });
            if (result.IsSuccess)
            {
                versions.Remove(version);
            }
            else
            {
                MsWindow.ShowDialog(result.Error, LanguageHelper.Instance.GetStrResource("Str_FailToDeleteData"));
            }
        }
        #endregion


        #region 展开/收起
        public bool IsAllExpander
        {
            get { return (bool)GetValue(IsAllExpanderProperty); }
            set { SetValue(IsAllExpanderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllExpander.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllExpanderProperty =
            DependencyProperty.Register("IsAllExpander", typeof(bool), typeof(VersionsView), new PropertyMetadata(true));

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            IsAllExpander = !IsAllExpander;
            recordExpanderList?.ForEach(it => it.IsExpanded = IsAllExpander);
            ExpandButton.Content = IsAllExpander ? LanguageHelper.Instance.GetStrResource("Str_Fold") : LanguageHelper.Instance.GetStrResource("Str_Expand");
        }

        private List<Expander> recordExpanderList = new List<Expander>();
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander && expander != null)
            {
                if (!recordExpanderList.Contains(expander))
                {
                    recordExpanderList.Add(expander);
                }
                if (recordExpanderList.All(it => it.IsExpanded != IsAllExpander))
                {
                    IsAllExpander = !IsAllExpander;
                    ExpandButton.Content = IsAllExpander ? LanguageHelper.Instance.GetStrResource("Str_Fold") : LanguageHelper.Instance.GetStrResource("Str_Expand");
                }
            }
        }
        #endregion

        private void ShowContent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MediaButton mediaButton && mediaButton != null)
            {
                if (mediaButton.DataContext is Version version && !string.IsNullOrEmpty(version?.Content))
                {
                    if (mediaButton.IsHitTestVisible)
                    {
                        popupSetting.PlacementTarget = mediaButton;
                        popupSetting.IsOpen = true;
                        mediaButton.IsHitTestVisible = false;
                        popupSetting.HorizontalOffset = -165;
                        tbContent.Text = version.Content;
                    }
                    else
                    {
                        popupSetting.IsOpen = false;
                    }
                }
            }
        }

        private void dgVersions_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void dgVersions_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            eventArg.RoutedEvent = UIElement.MouseWheelEvent;

            eventArg.Source = sender;

            this.dgVersions.RaiseEvent(eventArg);
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is Version version && version != null)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = LanguageHelper.Instance.GetStrResource("Str_SelSaveFolder");
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        loading.Start();
                        loading.SetMessage(LanguageHelper.Instance.GetStrResource("Str_GettingNewVer"));
                    });
                    var runPath = dialog.SelectedPath;

                    var localFilePath = runPath + "/" + version.Code + version.Ext;

                    var proxy = new MinioProxy();
                    proxy.Load();
                    proxy.MinioConfig = new MinioConfig()
                    {
                        Region = "center",
                        SName = ((int)AccountType.FS).ToString(),
                        Bucket = "version",
                        Address = string.Format("http://{0}:{1}", CacheHelper.LocalSetting.SelectedSetting.Host, CacheHelper.LocalSetting.SelectedSetting.Port)
                    };
                    var fastTask = new DfsTask()
                    {
                        DfsProxy = proxy,
                        LocalPath = localFilePath,
                        RemotePath = version.Path,
                        Bucket = "version",
                    };

                    fastTask.ProgressHandler += (obj, args) =>
                    {
                        var percentage = (double)args.Position / (double)args.Size;
                        Console.WriteLine($"{(percentage).ToString("P0")}【{args.Position}/{args.Size}】");
                        this.Dispatcher.Invoke(() => loading.SetMessage(string.Format("{0}{1:P0}", LanguageHelper.Instance.GetStrResource("Str_GettingNewVer"), (double)args.Position / (double)args.Size)));
                    };
                    await proxy.DownloadFileAsync(fastTask);
                    loading.Stop();

                    if (isDisposed) return;

                    if (fastTask.TaskState == TaskState.Completed)
                    {
                    }
                    else
                    {
                        if (fastTask.TaskState == TaskState.Error)
                        {
                            if (!isDisposed) MsWindow.ShowDialog(fastTask.Error, LanguageHelper.Instance.GetStrResource("Str_DownloadFailure"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    loading.Stop();
                    if (!isDisposed)
                        MsWindow.ShowDialog($"下载失败:" + ex.Message);
                }

            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is Version version && version != null)
            {
                try
                {
                    Clipboard.SetDataObject(version.Content);
                    MsWindow.ShowDialog($"{LanguageHelper.Instance.GetStrResource("Str_Copy")}{LanguageHelper.Instance.GetStrResource("Str_Success")}");
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error(ex);
                    MsWindow.ShowDialog($"拷贝更新内容失败:" + ex.Message);
                }
            }
        }

        private bool isDisposed;
        public void Dispose()
        {
            isDisposed = true;
        }
    }
}
