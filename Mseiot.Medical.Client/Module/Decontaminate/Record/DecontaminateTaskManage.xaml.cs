﻿using System;
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
    /// DecontaminateTaskManage.xaml 的交互逻辑
    /// </summary>
    public partial class DecontaminateTaskManage : UserControl
    {
        public DecontaminateTaskManage()
        {
            InitializeComponent();
            this.Loaded += DecontaminateTaskManage_Loaded;
        }

        private void DecontaminateTaskManage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DecontaminateTaskManage_Loaded;
            lvDatas.ItemsSource = DecontaminateTasks;
            LoadDecontaminateTasks();
        }


        #region 数据

        public ObservableCollection<DecontaminateTask> DecontaminateTasks { get; set; } = new ObservableCollection<DecontaminateTask>();

        private async void LoadDecontaminateTasks()
        {
            DecontaminateTasks.Clear();
            loading.Start("获取内窥镜列表中,请稍后");
            var result = await SocketProxy.Instance.GetDecontaminateTasks(new List<DecontaminateTaskStatus>() { DecontaminateTaskStatus.Complete });
            if (result.IsSuccess)
                DecontaminateTasks.AddRange(result.Content);
            loading.Stop();
        }

        #endregion
    }
}
