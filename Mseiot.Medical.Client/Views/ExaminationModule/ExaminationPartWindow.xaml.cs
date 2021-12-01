using Ms.Controls;
using Ms.Libs.SysLib;
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
using System.Windows.Shapes;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ExaminationPartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExaminationPartWindow : BaseWindow
    {
        public ExaminationPartWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyExamination(epv.SelectedExamination));
            if (result.IsSuccess)
            {
                result = loading.AsyncWait("保存检查信息中,请稍后", SocketProxy.Instance.ModifyAppointment(epv.SelectedExamination.Appointment));
                if (result.IsSuccess)
                {
                    Alert.ShowMessage(true, AlertType.Success, "保存检查信息成功");
                    epv.video.Dispose();
                    this.DialogResult = true;
                }
                else Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败2,{ result.Error }");
            }
            else Alert.ShowMessage(true, AlertType.Error, $"保存检查信息失败1,{ result.Error }");
        }
    }
}
