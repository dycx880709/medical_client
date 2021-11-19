using Ms.Controls;
using MM.Medical.Client.Entities;
using MM.Medical.Client.Views;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
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
using Ms.Controls.Core;
using Newtonsoft.Json;
using System.IO;
using Ms.Libs.SysLib;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// BaseWordView.xaml 的交互逻辑
    /// </summary>
    public partial class BaseWordView : UserControl
    {
        public BaseWordView()
        {
            InitializeComponent();
            this.Loaded += BaseWordView_Loaded;
        }

        private void BaseWordView_Loaded(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
            this.MouseLeftButtonDown += (o, ex)=> ResetBaseWord();
        }

        private void ModifyWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BaseWordExtend word)
            {
                ResetBaseWord();
                lb_words.SelectedValue = word;
                word.IsSelected = true;
                var grid = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(grid);
                tb.SelectionStart = tb.Text.Length;
                tb.Focus();
            }
        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BaseWordExtend word)
            {
                ResetBaseWord();
                var lb = ControlHelper.GetParentObject<ListBox>(element);
                if (lb.ItemsSource is ObservableCollection<BaseWordExtend> words)
                {
                    var result = loading.AsyncWait("删除词条中,请稍后", SocketProxy.Instance.RemoveBaseWord(word.BaseWordID));
                    if (!result.IsSuccess) 
                        MsWindow.ShowDialog($"删除词条失败,{ result.Error }", "软件提示");
                    else 
                        words.Remove(word);
                }
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.ItemsSource is ObservableCollection<BaseWordExtend> words)
            {
                ResetBaseWord();
                var word = new BaseWordExtend() { IsSelected = true };
                words.Add(word);
                lb_words.ScrollIntoView(word);
                lb_words.SelectedValue = word;
                var lbv = lb_words.ItemContainerGenerator.ContainerFromIndex(lb_words.Items.Count - 1) as ListBoxItem;
                var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                tb.Focus();
            }
        }

        private void RefreshWord_Click(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
        }
        private void SaveWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BaseWordExtend word)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                if (word.BaseWordID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"新建词条名称不能为空", "软件提示");
                        return;
                    }
                    var add = new BaseWordExtend { Title = tb.Text };
                    var result = loading.AsyncWait("新建词条中,请稍后", SocketProxy.Instance.AddBaseWord(add));
                    if (result.IsSuccess)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        word.BaseWordID = result.Content;
                        word.IsSelected = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"编辑词条名称不能为空", "软件提示");
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(word.Title))
                    {
                        var update = word.Copy();
                        update.Title = tb.Text;
                        var result = loading.AsyncWait("更新词条中,请稍后", SocketProxy.Instance.ModifyBaseWord(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            word.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新词条失败,{ result.Error }", "软件提示");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word && sender is FrameworkElement element && element.DataContext is string item)
            {
                var word_back = word.Copy();
                word_back.Items.Remove(item);
                var result = loading.AsyncWait("删除词条明细中,请稍后", SocketProxy.Instance.ModifyBaseWord(word_back));
                if (result.IsSuccess) 
                    word.Items.Remove(item);
                else 
                    MsWindow.ShowDialog($"删除词条明细失败,{ result.Error }", "软件提示");
            }
            else 
                MsWindow.ShowDialog("请选择需要编辑明细的词条", "软件提示");
        }

        private void ModifyItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word && sender is FrameworkElement element && element.DataContext is string item)
            {
                var view = new AddWordItemView(word, word.Items.IndexOf(item), this.loading);
                sp.ShowDialog("编辑词条明细", view);
            }
            else 
                MsWindow.ShowDialog("请选择需要编辑明细的词条", "软件提示");
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word)
            {
                var view = new AddWordItemView(word, -1, this.loading);
                sp.ShowDialog("添加词条明细", view);
            }
            else 
                MsWindow.ShowDialog("请选择需要添加明细的词条", "软件提示");
        }

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.ItemsSource is ObservableCollection<BaseWordExtend> list)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { 
                    var jsonSource = JsonConvert.SerializeObject(list);
                    var filePath = System.IO.Path.Combine(dialog.SelectedPath, $"基础词库_{ TimeHelper.ToUnixTime(DateTime.Now) }.json");
                    File.WriteAllText(filePath, jsonSource);
                    Alert.ShowMessage(true, AlertType.Success, "基础词库导出完成");
                }
            }
        }
        private void ImportWord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dialog.Filter = "基础词库文件（*.json）|*.json";
            if (dialog.ShowDialog().Value)
            {
                var json = File.ReadAllText(dialog.FileName);
                var datas = JsonConvert.DeserializeObject<List<BaseWord>>(json);
                if (datas == null || datas.Count == 0)
                    Alert.ShowMessage(true, AlertType.Error, "基础词库文件格式异常");
                else
                {
                    var result = loading.AsyncWait("导入基础词库中,请稍后", SocketProxy.Instance.ImportBaseWord(json));
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Success, "导入基础词库成功");
                        GetBaseWords();
                    }
                    else
                        Alert.ShowMessage(true, AlertType.Error, $"导入基础词库失败,{ result.Error }");
                }
            }
        }

        private void ResetBaseWord()
        {
            var word = lb_words.Items.OfType<BaseWordExtend>().FirstOrDefault(p => p.IsSelected);
            if (word != null)
            {
                if (word.BaseWordID == 0)
                    (lb_words.ItemsSource as ObservableCollection<BaseWordExtend>).Remove(word);
                else
                {
                    var index = lb_words.Items.IndexOf(word);
                    var lbi = lb_words.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    word.IsSelected = false;
                }
            }
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取基础词库中,请稍后", SocketProxy.Instance.GetBaseWords());
            if (result.IsSuccess) 
                lb_words.ItemsSource = new ObservableCollection<BaseWordExtend>(result.Content.Select(t => new BaseWordExtend(t)));
            else 
                MsWindow.ShowDialog($"获取基础词库失败,{ result.Error }", "软件提示");
        }

    }
}
