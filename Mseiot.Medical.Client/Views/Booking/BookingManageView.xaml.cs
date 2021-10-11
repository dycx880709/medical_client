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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// BookingManageView.xaml 的交互逻辑
    /// </summary>
    public partial class BookingManageView : UserControl
    {
        public BookingManageView()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddBookingView(new PatientInfo(), this.loading);
            if (child.ShowDialog("预约登记", view))
                pmv.Refresh();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (pmv.SelectedPatient != null)
            {
                var result = loading.AsyncWait("移除预约中,请稍后", SocketProxy.Instance.RemovePatient(pmv.SelectedPatient.PatientID));
                if (result.IsSuccess) pmv.Refresh();
                else MsWindow.ShowDialog($"删除预约失败,{ result.Error }", "软件提示");
            }
            else MsWindow.ShowDialog($"请选择移除预约项", "软件提示");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            pmv.Refresh();
        }
    }
}
