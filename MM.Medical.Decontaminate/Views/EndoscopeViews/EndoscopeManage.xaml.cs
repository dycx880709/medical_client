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
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;

namespace MM.Medical.Decontaminate.Views.EndoscopeViews
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
            LoadDatas();
        }


        #region 数据

        public ObservableCollection<Endoscope> Endoscopes { get; set; } = new ObservableCollection<Endoscope>();

        private async void LoadDatas()
        {
            Endoscopes.Clear();
            var result = await SocketProxy.Instance.GetEndoscopes();
            if (result.IsSuccess)
            {
                if (result.Content != null)
                {
                    Endoscopes.AddRange(result.Content);
                }
            }
        }

        #endregion

        #region 添加、删除、修改

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEndoscope addEndoscope = new AddEndoscope();
            MsWindow.ShowDialog(addEndoscope);
            LoadDatas();
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = await SocketProxy.Instance.RemoveEndoscopes(Endoscopes.Where(f => f.IsSelected).Select(f => f.EndoscopeID).ToList());
            if (result.IsSuccess)
            {
                LoadDatas();
            }
            else
            {
                MsPrompt.ShowDialog("删除失败");
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            Endoscope endoscope = (sender as FrameworkElement).Tag as Endoscope;
            AddEndoscope addEndoscope = new AddEndoscope(endoscope);
            MsWindow.ShowDialog(addEndoscope);
            LoadDatas();
        }

        #endregion

        #region 选择

        private void All_Selected(object sender, RoutedEventArgs e)
        {
            foreach(var item in Endoscopes)
            {
                item.IsSelected = true;
            }
        }


        #endregion

        #region RFID

        private void CreateRFID_Click(object sender, RoutedEventArgs e)
        {
            Endoscope endoscope = (sender as FrameworkElement).Tag as Endoscope;

            RFIDProxy rfidProxy = new RFIDProxy();
            try
            {
                rfidProxy.OpenWait("COM3");
                rfidProxy.WriteEPC(endoscope.EndoscopeID);
                Alert.ShowMessage(true, AlertType.Success, "写卡成功");
            }
            catch(Exception ex)
            {
                MsPrompt.ShowDialog("写卡失败:"+ex.Message);
            }
            finally
            {
                rfidProxy.Close();
            }
        }

        #endregion
    }
}
