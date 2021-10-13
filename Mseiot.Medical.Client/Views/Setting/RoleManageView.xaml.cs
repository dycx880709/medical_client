using Ms.Controls;
using Mseiot.Medical.Client.Entities;
using Mseiot.Medical.Client.Views.Component;
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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// RoleManageView.xaml 的交互逻辑
    /// </summary>
    public partial class RoleManageView : UserControl
    {
        public List<AppLevel> AppLevels { get; set; }

        public RoleManageView()
        {
            InitializeComponent();
            this.AppLevels = new List<AppLevel>()
            {
                new AppLevel { Name = "内镜综合管理系统", Level = "0" },
                new AppLevel { Name = "内镜预约登记系统", Level = "1" },
                new AppLevel { Name = "内镜消洗追溯系统", Level = "2" },
                new AppLevel { Name = "内镜数据分析系统", Level = "3" },
            };
            lb_level.ItemsSource = this.AppLevels;
            this.Loaded += RoleManageView_Loaded;
        }

        private void RoleManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetRoles();
        }

        private void GetRoles()
        {
            var result = loading.AsyncWait("获取角色信息中,请稍后", SocketProxy.Instance.GetRoles());
            if (result.IsSuccess) lb_role.ItemsSource = result.Content;
            else MsWindow.ShowDialog($"获取角色信息失败,{ result.Error }", "软件提示");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var role = new Role();
            var view = new AddRoleView(role, loading);
            if (sp.ShowDialog("新建角色", view))
                GetRoles();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Role role)
            {
                var view = new AddRoleView(role, loading);
                sp.ShowDialog("编辑角色", view);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Role role)
            {
                var result = loading.AsyncWait("删除角色中", SocketProxy.Instance.RemoveRole(role.RoleID));
                if (result.Content) GetRoles();
                else MsWindow.ShowDialog($"删除角色{ role.Name }失败,{ result.Error }", "软件提示");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetRoles();
        }

        private void ModifyLevel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is AppLevel appLevel)
            {
                if (lb_role.SelectedValue is Role role)
                {
                    role.Authority = appLevel.Level;
                    var result = loading.AsyncWait("编辑角色权限中,请稍后", SocketProxy.Instance.ModifyRole(role));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"编辑角色权限失败,{ result.Error }", "软件提示");
                    AppLevels.ForEach(t => t.IsSelected = t.Level == role.Authority);
                }
                else appLevel.IsSelected = false;
            }
        }

        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_role.SelectedValue is Role role)
                AppLevels.ForEach(t => t.IsSelected = t.Level.Equals(role.Authority));
        }
    }
}
