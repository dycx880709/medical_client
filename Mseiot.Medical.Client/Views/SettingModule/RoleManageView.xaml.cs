using Ms.Controls;
using MM.Medical.Client.Entities;
using MM.Medical.Client.Views;
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
using System.Collections.ObjectModel;
using Ms.Controls.Core;

namespace MM.Medical.Client.Views
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
            this.MouseLeftButtonDown += (o, ex) => ResetRole();
        }

        private void GetRoles()
        {
            var result = loading.AsyncWait("获取角色信息中,请稍后", SocketProxy.Instance.GetRoles());
            if (result.IsSuccess) lb_role.ItemsSource = new ObservableCollection<Role>(result.Content);
            else MsWindow.ShowDialog($"获取角色信息失败,{ result.Error }", "软件提示");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (lb_role.ItemsSource is ObservableCollection<Role> roles)
            {
                ResetRole();
                var role = new Role() { IsSelected = true };
                roles.Add(role);
                lb_role.ScrollIntoView(role);
                lb_role.SelectedValue = role;
                var lbv = lb_role.ItemContainerGenerator.ContainerFromIndex(lb_role.Items.Count - 1) as ListBoxItem;
                if (lbv != null)
                {
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                    tb.Focus();
                }
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Role role)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                if (role.RoleID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"新建角色名称不能为空", "软件提示");
                        return;
                    }
                    var add = new Role { Name = tb.Text };
                    var result = loading.AsyncWait("新建角色中,请稍后", SocketProxy.Instance.AddRole(add));
                    if (result.IsSuccess)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        role.RoleID = result.Content;
                        role.IsSelected = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"编辑角色名称不能为空", "软件提示");
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(role.Name))
                    {
                        var update = role.Copy();
                        update.Name = tb.Text;
                        var result = loading.AsyncWait("更新角色中,请稍后", SocketProxy.Instance.ModifyRole(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            role.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新角色失败,{ result.Error }", "软件提示");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Role role)
            {
                ResetRole();
                lb_role.SelectedValue = role;
                role.IsSelected = true;
                var grid = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(grid);
                tb.SelectionStart = tb.Text.Length;
                tb.Focus();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Role role)
            {
                ResetRole();
                var lb = ControlHelper.GetParentObject<ListBox>(element);
                if (lb.ItemsSource is ObservableCollection<Role> roles)
                {
                    if (!MsPrompt.ShowDialog("删除角色将该角色用户无法正常使用,是否继续"))
                        return;
                    var result = loading.AsyncWait("删除角色中,请稍后", SocketProxy.Instance.RemoveRole(role.RoleID));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"删除角色失败,{ result.Error }", "软件提示");
                    else roles.Remove(role);
                }
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

        private void ResetRole()
        {
            var role = lb_role.Items.OfType<Role>().FirstOrDefault(p => p.IsSelected);
            if (role != null)
            {
                if (role.RoleID == 0)
                    (lb_role.ItemsSource as ObservableCollection<Role>).Remove(role);
                else
                {
                    var index = lb_role.Items.IndexOf(role);
                    var lbi = lb_role.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    role.IsSelected = false;
                }
            }
        }
    }
}
