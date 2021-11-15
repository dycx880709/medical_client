using Ms.Controls;
using MM.Medical.Management.Views.Component;
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
using MM.Medical.Share.Core;

namespace MM.Medical.Management.Views
{
    /// <summary>
    /// UserManageView.xaml 的交互逻辑
    /// </summary>
    public partial class UserManageView : UserControl
    {
        public UserManageView()
        {
            InitializeComponent();
            this.Loaded += UserManageView_Loaded;
        }

        private void UserManageView_Loaded(object sender, RoutedEventArgs e)
        {
            GetUsers();
        }

        private void GetUsers()
        {
            RefreshItemCount();
            var result = loading.AsyncWait("获取用户中,请稍后", SocketProxy.Instance.GetUsers(pager.PageIndex + 1, pager.SelectedCount));
            if (result.IsSuccess)
            {
                pager.TotalCount = result.Content.Total;
                dg_user.ItemsSource = result.Content.Results;
            }
            else MsWindow.ShowDialog($"获取用户失败,{ result.Error }", "软件提示");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var user = new User();
            if (sp.ShowDialog("新建用户", new AddUserView(user, this.loading)))
            {
                pager.PageIndex = 0;
                GetUsers();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg_user.SelectedValue is User user)
            {
                var view = new AddUserView(user, this.loading);
                sp.ShowDialog("编辑用户", view);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (dg_user.SelectedValue is User user)
            {
                var result = loading.AsyncWait("删除用户中", SocketProxy.Instance.RemoveUser(user.UserID));
                if (result.Content)
                    GetUsers();
                else MsWindow.ShowDialog($"删除用户{ user.Name }失败,{ result.Error }", "软件提示");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetUsers();
        }

        private int RefreshItemCount()
        {
            var columnHeight = CacheHelper.GetResource<int>("DataGrdiColumnHeight");
            var rowHeight = CacheHelper.GetResource<int>("DataGrdiRowHeight");
            var height = dg_user.ActualHeight - columnHeight;
            var count = (int)(height / rowHeight);
            pager.SelectedCount = count;
            return count;
        }
    }
}
