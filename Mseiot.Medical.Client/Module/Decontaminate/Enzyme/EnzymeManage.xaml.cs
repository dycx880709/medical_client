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
    /// EnzymeManage.xaml 的交互逻辑
    /// </summary>
    public partial class EnzymeManage : UserControl
    {
        public EnzymeManage()
        {
            InitializeComponent();
            this.Loaded += EnzymeManage_Loaded;
        }

        private void EnzymeManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= EnzymeManage_Loaded;
            lvDatas.ItemsSource = Enzymes;
            LoadEnzymes();
        }


        #region 数据

        public ObservableCollection<Enzyme> Enzymes { get; set; } = new ObservableCollection<Enzyme>();

        private async void LoadEnzymes()
        {
            Enzymes.Clear();
            loading.Start("获取清洗酶列表中,请稍后");
            var result = await SocketProxy.Instance.GetEnzymes();
            if (result.IsSuccess)
                Enzymes.AddRange(result.Content);
            loading.Stop();
        }

        #endregion

        #region 添加、删除、修改

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var enzyme = new Enzyme();
            AddEnzyme addEnzyme = new AddEnzyme(enzyme, this.loading);
            if (child.ShowDialog("添加清洗酶", addEnzyme))
                LoadEnzymes();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (MsPrompt.ShowDialog("是否继续?"))
            {
                if (sender is FrameworkElement element && element.DataContext is Enzyme enzyme)
                {
                    var result = loading.AsyncWait("删除清洗酶中,请稍后", SocketProxy.Instance.RemoveEnzymes(new List<int> { enzyme.EnzymeID }));
                    if (result.IsSuccess)
                        LoadEnzymes();
                    else Alert.ShowMessage(true, AlertType.Error, $"删除清洗酶失败,{ result.Error }");
                }
            }
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Enzyme enzyme)
            {
                AddEnzyme addEnzyme = new AddEnzyme(enzyme, this.loading);
                if (child.ShowDialog("编辑清洗酶", addEnzyme))
                    LoadEnzymes();
            }
        }

        #endregion

        private async void Select_Click(object sender, RoutedEventArgs e)
        {
            Enzyme enzyme = (sender as FrameworkElement).Tag as Enzyme;
            if (enzyme != null)
            {
                foreach (var item in Enzymes)
                {
                    if (item.EnzymeID == enzyme.EnzymeID)
                    {
                        item.Enabled = true;
                    }
                    else
                    {
                        item.Enabled = false;
                    }
                }
            }

            var result = await SocketProxy.Instance.ChangeModifyEnzymeSelected(enzyme);
            if (result.IsSuccess)
            {
                Alert.ShowMessage(true, AlertType.Success, "设置成功");
            }
            else
            {
                Alert.ShowMessage(false, AlertType.Error, result.Error);
            }
        }
    }
}