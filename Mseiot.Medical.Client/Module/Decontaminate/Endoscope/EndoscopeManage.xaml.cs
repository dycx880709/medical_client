﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            if (MsPrompt.ShowDialog("是否继续?"))
            {
                if (sender is FrameworkElement element && element.DataContext is Endoscope endoscope)
                {
                    var result = loading.AsyncWait("删除内窥镜中,请稍后", SocketProxy.Instance.RemoveEndoscopes(new List<int> { endoscope.EndoscopeID }));
                    if (result.IsSuccess)
                        LoadEndoscopes();
                    else
                        Alert.ShowMessage(true, AlertType.Error, $"删除内窥镜失败,{ result.Error }");
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
            foreach (var item in Endoscopes)
                item.IsSelected = true;
        }


        #endregion

        #region RFID

        private async void CreateRFID_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Endoscope endoscope)
            {
                if (string.IsNullOrEmpty(CacheHelper.LocalSetting.RFIDCom))
                {
                    Alert.ShowMessage(true, AlertType.Error, "制卡器未配置,请先配置制卡器");
                    var view = new DecontaminateSetting();
                    child.ShowDialog("配置制卡器", view);
                }
                else
                {
                    RFIDProxy rfidProxy = new RFIDProxy();
                    try
                    {
                        rfidProxy.OpenWait(CacheHelper.LocalSetting.RFIDCom);
                        await rfidProxy.WriteEPC(endoscope.EndoscopeID);
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Success, "写卡成功"));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, "写卡失败:" + ex.Message));
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
            var view = new DecontaminateSetting();
            child.ShowDialog("配置制卡器", view);
        }

        #endregion

        private async void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && tb.DataContext is Endoscope endoscope)
            {
                var state = endoscope.State == EndoscopeState.Disabled ? "禁用" : "启用";
                loading.Start($"{ state }内窥镜中,请稍后");
                var result = await SocketProxy.Instance.ModifyEndoscope(endoscope);
                this.Dispatcher.Invoke(() =>
                {
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Success, $"{ state }内窥镜成功");
                        LoadEndoscopes();
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"{ state }内窥镜失败,{ result.Error }");
                        tb.IsChecked = !tb.IsChecked.Value;
                    }
                    loading.Stop();
                });
            }
        }
    }
}