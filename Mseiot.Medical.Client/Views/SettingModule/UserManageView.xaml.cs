using Ms.Controls;
using MM.Medical.Client.Core;
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
using MM.Libs.RFID;

namespace MM.Medical.Client.Views
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
            pager.SelectedCount = dg_user.GetFullCountWithoutScroll();
        }

        private void GetUsers()
        {
            pager.SelectedCount = dg_user.GetFullCountWithoutScroll();
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
                if (sp.ShowDialog("编辑用户", view))
                    GetUsers();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (dg_user.SelectedValue is User user && MsPrompt.ShowDialog("确定删除该用户,是否继续?"))
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

        private void Pager_PageChanged(object sender, PageChangedEventArgs args)
        {
            GetUsers();
        }

        private async void CreateRFID_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is User user)
            {
                if (string.IsNullOrEmpty(CacheHelper.LocalSetting.RFIDCom))
                {
                    Alert.ShowMessage(false, AlertType.Error, "制卡器未配置,请先配置制卡器");
                    var view = new Module.Decontaminate.DecontaminateSetting();
                    sp.ShowDialog("配置制卡器", view);
                }
                else
                {

                    var rfidProxy = new RFIDProxy();
                    try
                    {
                        rfidProxy.OpenWait(CacheHelper.LocalSetting.RFIDCom);
                        await rfidProxy.WriteEPC(user.ID + 100000000);
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Success, "写卡成功"));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(false, AlertType.Error, "写卡失败:" + ex.Message));
                    }
                    finally
                    {
                        rfidProxy.Close();
                    }
                }
            }
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            var view = new Module.Decontaminate.DecontaminateSetting();
            sp.ShowDialog("配置制卡器", view);
        }
    }
}
