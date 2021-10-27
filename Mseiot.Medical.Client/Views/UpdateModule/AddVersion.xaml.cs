using Microsoft.Win32;
using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Version = Mseiot.Medical.Service.Entities.Version;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// AddVersion.xaml 的交互逻辑
    /// </summary>
    public partial class AddVersion : UserControl
    {
        private Loading loading;
        private Version version;

        #region 构造器

        public AddVersion(Version version, Loading loading)
        {
            InitializeComponent();
            this.version = version;
            this.loading = loading;
            this.Loaded += AddVersion_Loaded;
        }

        private void AddVersion_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.version;
        }

        #endregion

        public async Task<bool> Save()
        {
            #region 输入有效性验证
            if (string.IsNullOrEmpty(version.Code))
            {
                MsWindow.ShowDialog("版本号不能为空");
                return false;
            }

            #region 版本号有效性校验
            if (!VersionCodeValidate())
            {
                MsWindow.ShowDialog("版本号校验未通过");
                tbVersionCode.Focus();
                return false;
            }
            #endregion

            if (string.IsNullOrEmpty(version.LocalPath))
            {
                MsWindow.ShowDialog("上传新版本文件不能为空");
                return false;
            }

            #endregion

            #region  验证版本

            var result = await SocketProxy.Instance.VerifyVersion(this.version);
            if (!result.IsSuccess)
            {
                MsWindow.ShowDialog(result.Error);
                return false;
            }

            #endregion

            var panelResult = false;
            loading.Start("上传版本中,请稍后");
            #region 文件上传
            if (string.IsNullOrEmpty(version.LocalPath))
            {
                loading.SetMessage("版本文件上传中");
                var fileSize = FileHelper.GetFileLength(version.LocalPath);
                var upload = await SocketProxy.Instance.UploadFile(version.LocalPath);
                if(!upload.IsSuccess)
                {
                    MsWindow.ShowDialog($"上传版本失败:{ upload.Error }");
                    return false;
                }
                else
                {
                    version.Path = upload.Content;
                    version.Size = fileSize;
                    version.Time = TimeHelper.ToUnixTime(DateTime.Now);
                }
            }
            #endregion
            var res = await SocketProxy.Instance.AddVersion(this.version);
            loading.Stop();
            if (res.IsSuccess)
            {
                version.VersionID = res.Content;
                MsWindow.ShowDialog("版本更新成功,软件将重启");
                panelResult = true;
            }
            else
            {
                MsWindow.ShowDialog($"版本更新失败,{ res.Error }");
                panelResult = false;
            }
            return panelResult;
        }

        private bool VersionCodeValidate()
        {
            bool isValidVersionCode = true;
            if (this.version != null && !string.IsNullOrEmpty(version.Code) && version.Code.Contains("."))
            {
                var codes = version.Code.Split('.');
                if (codes.Length == 4)
                {
                    foreach (var code in codes)
                    {
                        if (!uint.TryParse(code, out var ucode))
                        {
                            isValidVersionCode = false;
                            break;
                        }
                    }
                }
                else isValidVersionCode = false;
            }
            else isValidVersionCode = false;
            return isValidVersionCode;
        }

        #region 选择文件

        //选择文件
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = $"Zip压缩包|*.zip;",
                Multiselect = false,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog().Value)
                version.LocalPath = openFileDialog.FileName;
        }

        #endregion
    }
}
