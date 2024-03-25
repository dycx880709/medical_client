using Ms.Controls;
using Mseiot.Medical.Service.Entities;
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
    /// AddUserView.xaml 的交互逻辑
    /// </summary>
    public partial class AddUserView : UserControl
    {
        private User user;
        private User originUser;
        private Loading loading;

        public AddUserView(User user, Loading loading)
        {
            InitializeComponent();
            this.user = user.Copy();
            this.originUser = user;
            this.loading = loading;
            this.Loaded += AddUserView_Loaded;
        }

        private void AddUserView_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.user;
            GetRoles();
        }

        private void GetRoles()
        {
            var result = loading.AsyncWait("获取角色列表中,请稍后", SocketProxy.Instance.GetRoles());
            if (result.IsSuccess)
            {
                cb_roles.ItemsSource = result.Content;
                var condition = result.Content.FirstOrDefault(t => t.RoleID.Equals(user.RoleID));
                if (condition != null)
                    cb_roles.SelectedValue = result.Content.FirstOrDefault(t => t.RoleID.Equals(user.RoleID)).RoleID;
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"获取角色列表失败,{ result.Error }", "软件提示");
                this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(user.LoginName))
            {
                Alert.ShowMessage(true, AlertType.Error, "登录名不能为空", "软件提示");
                return;
            }
            if (string.IsNullOrEmpty(user.LoginName))
            {
                Alert.ShowMessage(true, AlertType.Error, "用户姓名不能为空", "软件提示");
                return;
            }
            if (user.RoleID <= 0)
            {
                Alert.ShowMessage(true, AlertType.Error, "用户角色不能为空", "软件提示");
                return;
            }
            if (cb_roles.SelectedValue == null)
            {
                Alert.ShowMessage(true, AlertType.Error, "用户角色不能为空", "软件提示");
                return;
            }
            if (string.IsNullOrEmpty(user.UserID))
            {
                var result = loading.AsyncWait("添加用户中,请稍后", SocketProxy.Instance.AddUser(user));
                if (result.IsSuccess)
                {
                    user.UserID = result.Content;
                    user.CopyTo(originUser);
                    this.Close();
                }
                else Alert.ShowMessage(true, AlertType.Error, $"添加用户失败,{ result.Error }", "软件提示");
            }
            else
            {
                var result = loading.AsyncWait("编辑用户中,请稍后", SocketProxy.Instance.ModifyUser(user));
                if (result.IsSuccess)
                {
                    user.CopyTo(originUser);
                    this.Close();
                }
                else Alert.ShowMessage(true, AlertType.Error, $"编辑用户失败,{ result.Error }", "软件提示");
            }
        }
    }
}
