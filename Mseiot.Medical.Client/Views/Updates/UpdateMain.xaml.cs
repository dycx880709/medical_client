using Ms.Controls;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Version = Mseiot.Medical.Service.Entities.Version;

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// UpdateMain.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateMain : UserControl, IDisposable
    {
        #region 类成员

        private Version version;

        #endregion

        #region 构造器

        public UpdateMain(Version version)
        {
            InitializeComponent();
            this.version = version;
            tbContent.Text = version.Content;
            tbTime.Text = TimeHelper.FromUnixTime(version.Time).ToString("yyyy/MM/dd HH:mm:ss");
            tbVersion.Text = version.Code;

            if ((this.Tag is Window window) && window != null)
                window.Closed += (o, e) => Dispose();
            else
                this.Unloaded += (o, e) => Dispose();
        }

        #endregion

        #region 事件

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            
        }

        bool isDisposed = false;

        public void Dispose()
        {
            isDisposed = true;
        }

        #endregion
    }
}
