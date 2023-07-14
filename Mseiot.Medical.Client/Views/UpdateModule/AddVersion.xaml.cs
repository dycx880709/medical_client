using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Libs.SysLib;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Version = Mseiot.Medical.Service.Entities.Version;
using Mseiot.Medical.Service.Services;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// AddVersion.xaml 的交互逻辑
    /// </summary>
    public partial class AddVersion : UserControl
    {
        public Version Version { get; private set; }
        public bool IsSuccess { get; private set; }

        private Version origin;
        private Loading loading;

        public AddVersion(Version version, Loading loading)
        {
            InitializeComponent();
            this.loading = loading;
            if (version != null)
            {
                this.Version = version.Copy();
                this.origin = version; 
            }
            else
            {
                Version = new Version();
            }
            Version.Code = CacheHelper.ClientVersion;
            DataContext = this;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loading.Start("压缩文件中,请稍后");
            string zipFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.zip");
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }
            var zipResult = await ZipHelper.ZipDirectory(AppDomain.CurrentDomain.BaseDirectory, zipFileName);
            if (zipResult.IsSuccess)
            {
                this.Dispatcher.Invoke(() => loading.SetMessage("上传版本中,请稍后"));
                var fi = new FileInfo(zipFileName);
                Version.Size = fi.Length;
                var uploadResult = await SocketProxy.Instance.UploadFile(zipFileName);
                if (uploadResult.IsSuccess)
                {
                    loading.SetMessage("保存版本信息中,请稍后");
                    Version.UserID = CacheHelper.CurrentUser.UserID;
                    Version.Path = uploadResult.Content;
                    var result = await SocketProxy.Instance.AddVersion(Version);
                    this.Dispatcher.Invoke(() =>
                    {
                        loading.Stop();
                        if (result.IsSuccess)
                        {
                            Version.VersionID = result.Content;
                            Version.CopyTo(origin);
                            this.Close(true);
                        }
                        else
                        {
                            Alert.ShowMessage(false, AlertType.Error, result.Error);
                        }
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Alert.ShowMessage(false, AlertType.Error, "上传版本失败");
                        loading.Stop();
                    }); 
                }
            }
            else
            {
                this.Dispatcher.Invoke(()=>
                {
                    Alert.ShowMessage(false, AlertType.Error, zipResult.Error);
                    loading.Stop();
                });
            }
        }
    }
}
