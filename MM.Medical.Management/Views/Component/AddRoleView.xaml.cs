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

namespace MM.Medical.Management.Views.Component
{
    /// <summary>
    /// AddRoleView.xaml 的交互逻辑
    /// </summary>
    public partial class AddRoleView : UserControl
    {
        private Role role;
        private Role originRole;
        private Loading loading;
        public AddRoleView(Role role, Loading loading)
        {
            InitializeComponent();
            this.role = role.Copy();
            this.originRole = role;
            this.loading = loading;
            this.DataContext = this.role;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (role.RoleID != 0)
            {
                var result = loading.AsyncWait("编辑角色中,请稍后", SocketProxy.Instance.ModifyRole(role));
                if (result.IsSuccess && result.Content)
                {
                    role.CopyTo(originRole);
                    this.Close(true);
                }
                else
                {
                    MsWindow.ShowDialog($"新增角色列表失败,{ result.Error }", "软件提示");
                    this.Close();
                }
            }
            else
            {
                var result = loading.AsyncWait("新增角色中,请稍后", SocketProxy.Instance.AddRole(role));
                if (result.IsSuccess)
                {
                    role.RoleID = result.Content;
                    role.CopyTo(originRole);
                    this.Close(true);
                }
            }
        }
    }
}
