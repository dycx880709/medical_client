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
        private List<AppLevel> levels;
        public RoleManageView()
        {
            InitializeComponent();
            this.levels = AppLevel.Levels.Copy();
            lb_level.ItemsSource = this.levels;
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
                        Alert.ShowMessage(true, AlertType.Error, $"新建角色名称不能为空");
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
                        Alert.ShowMessage(true, AlertType.Error, $"编辑角色名称不能为空", "软件提示");
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
                            Alert.ShowMessage(true, AlertType.Error, $"更新角色失败,{ result.Error }", "软件提示");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                    else
                    {
                        role.IsSelected = false;
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
                    if (!MsPrompt.ShowDialog("删除角色将该角色用户无法正常使用,是否继续?"))
                        return;
                    var result = loading.AsyncWait("删除角色中,请稍后", SocketProxy.Instance.RemoveRole(role.RoleID));
                    if (!result.IsSuccess) 
                        Alert.ShowMessage(true, AlertType.Error, $"删除角色失败,{ result.Error }", "软件提示");
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
            if (sender is CheckBox checkBox && checkBox.DataContext is AppLevel appLevel && lb_role.SelectedValue is Role role)
            {
                var levels = role.Authority.Split(',').ToList();
                if (levels.Count == 1 && string.IsNullOrEmpty(levels[0]))
                    levels.Clear();
                var parent = GetParentLevel(this.levels, appLevel.Level);
                if (parent != null && !parent.Children.Any(t => t.IsSelected))
                { 
                    if (levels.Contains(parent.Level))
                        levels.Remove(parent.Level);
                    parent.IsSelected = false;
                }

                if (checkBox.IsChecked.Value)
                {
                    var children = LevelAllSelected(appLevel, true);
                    foreach (var changeLevel in children)
                    {
                        if (!levels.Contains(changeLevel))
                            levels.Add(changeLevel);
                    }
                    if (!levels.Contains(appLevel.Level))
                        levels.Add(appLevel.Level);
                }
                else
                {
                    var children = LevelAllSelected(appLevel, false);
                    foreach (var changeLevel in children)
                    {
                        if (levels.Contains(changeLevel))
                            levels.Remove(changeLevel);
                    }
                    if (levels.Contains(appLevel.Level))
                        levels.Remove(appLevel.Level);
                }
                var orgin = role.Authority;
                role.Authority = String.Join(",", levels);
                var result = loading.AsyncWait("编辑角色权限中,请稍后", SocketProxy.Instance.ModifyRole(role));
                if (!result.IsSuccess)
                {
                    Alert.ShowMessage(true, AlertType.Error, $"编辑角色权限失败,{ result.Error }", "软件提示");
                    role.Authority = orgin;
                }
            }
        }

        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_role.SelectedValue is Role role)
            {
                if (role.Authority == null) role.Authority = string.Empty;
                SetSelectedLevels(levels, role.Authority.Split(','));
            }
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

        private void SetSelectedLevels(List<AppLevel> applevels, string[] levels)
        {
            foreach (var appLevel in applevels)
            {
                appLevel.IsSelected = levels.Any(t => t.Equals(appLevel.Level));
                if (appLevel.Children != null && appLevel.Children.Count > 0)
                    SetSelectedLevels(appLevel.Children, levels);
            }
        }

        private AppLevel GetParentLevel(List<AppLevel> applevels, string level)
        {
            foreach (var applevel in applevels)
            {
                if (applevel.Children != null && applevel.Children.Count > 0)
                {
                    foreach (var item in applevel.Children)
                    {
                        if (item.Level.Equals(level))
                            return applevel;
                        else
                        {
                            var result = GetParentLevel(applevel.Children, level);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }
            return null;
        }

        private string[] LevelAllSelected(AppLevel appLevel, bool isSelected)
        {
            List<string> changeLevel = new List<string>();
            if (appLevel.Children != null && appLevel.Children.Count > 0)
            {
                foreach (var child in appLevel.Children)
                {
                    child.IsSelected = isSelected;
                    changeLevel.Add(child.Level);
                    changeLevel.AddRange(LevelAllSelected(child, isSelected));
                }
            }
            return changeLevel.ToArray();
        }
    }
}
