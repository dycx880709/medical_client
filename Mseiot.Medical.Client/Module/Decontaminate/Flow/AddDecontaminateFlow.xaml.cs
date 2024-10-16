﻿using Ms.Controls;
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

namespace MM.Medical.Client.Module.Decontaminate
{
    /// <summary>
    /// AddDecontaminateFlow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDecontaminateFlow : UserControl
    {
        private readonly DecontaminateFlow decontaminateFlow;
        private readonly DecontaminateFlow decontaminateFlow_origin;

        private readonly Loading loading;
        public AddDecontaminateFlow(DecontaminateFlow decontaminateFlow, Loading loading)
        {
            InitializeComponent();
            this.decontaminateFlow = decontaminateFlow.Copy();
            this.decontaminateFlow_origin = decontaminateFlow;
            this.Loaded += AddDecontaminateFlow_Loaded;
            this.loading = loading;
        }

        private async void AddDecontaminateFlow_Loaded(object sender, RoutedEventArgs e)
        {
            loading.Start("获取内镜中,请稍后");
            var result = await SocketProxy.Instance.GetEndoscopes();
            this.Dispatcher.Invoke(() =>
            {
                loading.Stop();
                if (result.IsSuccess)
                {
                    cb_model.ItemsSource = result.Content.Select(t => t.Model);
                    cb_model.UpdateLayout();
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取内镜失败:{result.Error}");
                }
                this.DataContext = this.decontaminateFlow;
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (decontaminateFlow.DecontaminateFlowID == 0)
            {
                var result = loading.AsyncWait("新建流程中,请稍后", SocketProxy.Instance.AddDecontaminateFlow(decontaminateFlow));
                if (result.IsSuccess)
                {
                    decontaminateFlow.DecontaminateFlowID = result.Content;
                    decontaminateFlow.CopyTo(decontaminateFlow_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "保存失败:"+result.Error);
            }
            else
            {
                var result = loading.AsyncWait("编辑流程中,请稍后", SocketProxy.Instance.ModifyDecontaminateFlow(decontaminateFlow));
                if (result.IsSuccess)
                {
                    decontaminateFlow.CopyTo(decontaminateFlow_origin);
                    this.Close(true);
                }
                else Alert.ShowMessage(true, AlertType.Error, "保存失败:" + result.Error);
            }
        }
    }
}
