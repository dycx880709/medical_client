using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
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
    /// ResetPwdView.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPwdView : UserControl
    {
        private Loading loading;

        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ComfirmPwd { get; set; }
        public ResetPwdView(Loading loading)
        {
            InitializeComponent();
            this.DataContext = this;
            this.loading = loading;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.OldPwd))
                Alert.ShowMessage(true, AlertType.Warning, "旧密码不能为空");
            else if (string.IsNullOrEmpty(this.NewPwd))
                Alert.ShowMessage(true, AlertType.Warning, "新密码不能为空");
            else if (!NewPwd.Equals(this.ComfirmPwd))
                Alert.ShowMessage(true, AlertType.Warning, "新密码与确认密码不一致");
            else
            {
                var result = loading.AsyncWait("修改密码中,请稍后", SocketProxy.Instance.ModifyPwd(CacheHelper.CurrentUser.UserID, this.OldPwd, this.NewPwd));
                if (result.IsSuccess)
                {
                    Alert.ShowMessage(true, AlertType.Success, "修改密码成功,软件重启后生效");
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"修改密码失败,{ result.Error }");
            }
        }
    }
}
