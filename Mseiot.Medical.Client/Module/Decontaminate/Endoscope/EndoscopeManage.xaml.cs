using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// EndoscopeManage.xaml 的交互逻辑
    /// </summary>
    public partial class EndoscopeManage : UserControl
    {
        public EndoscopeManage()
        {
            InitializeComponent();
            this.Loaded += EndoscopeManage_Loaded;
        }

        private void EndoscopeManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= EndoscopeManage_Loaded;
            lvDatas.ItemsSource = Endoscopes;
            LoadEndoscopes();
        }


        #region 数据

        public ObservableCollection<Endoscope> Endoscopes { get; set; } = new ObservableCollection<Endoscope>();

        private async void LoadEndoscopes()
        {
            Endoscopes.Clear();
            loading.Start("获取内窥镜列表中,请稍后");
            var result = await SocketProxy.Instance.GetEndoscopes();
            if (result.IsSuccess)
                Endoscopes.AddRange(result.Content);
            loading.Stop();
        }

        #endregion

        #region 添加、删除、修改

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var endoscope = new Endoscope();
            AddEndoscope addEndoscope = new AddEndoscope(endoscope, this.loading);
            if (child.ShowDialog("添加内窥镜", addEndoscope))
                LoadEndoscopes();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmWindow.Show("是否继续?"))
            {
                if (sender is FrameworkElement element && element.DataContext is Endoscope endoscope)
                {
                    var result = loading.AsyncWait("删除内窥镜中,请稍后", SocketProxy.Instance.RemoveEndoscopes(new List<int> { endoscope.EndoscopeID }));
                    if (result.IsSuccess)
                        LoadEndoscopes();
                    else Alert.ShowMessage(true, AlertType.Error, $"删除内窥镜失败,{ result.Error }");
                }
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Endoscope endoscope)
            {
                AddEndoscope addEndoscope = new AddEndoscope(endoscope, this.loading);
                if (child.ShowDialog("编辑内窥镜", addEndoscope))
                    LoadEndoscopes();
            }
        }

        #endregion

        #region 选择

        private void All_Selected(object sender, RoutedEventArgs e)
        {
            foreach(var item in Endoscopes)
                item.IsSelected = true;
        }


        #endregion

        #region RFID

        private async void CreateRFID_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Endoscope endoscope)
            {
                RFIDProxy rfidProxy = new RFIDProxy();
                try
                {
                    rfidProxy.OpenWait(CacheHelper.RFIDCom);
                    await rfidProxy.WriteEPC(endoscope.EndoscopeID);
                    Alert.ShowMessage(true, AlertType.Success, "写卡成功");
                }
                catch (Exception ex)
                {
                    Alert.ShowMessage(true, AlertType.Error, "写卡失败:" + ex.Message);
                }
                finally
                {
                    rfidProxy.Close();
                }
            }
        }

        #endregion
    }
}
