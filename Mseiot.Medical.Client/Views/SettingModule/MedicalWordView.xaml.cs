using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Controls.Core;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// MedicalWordView.xaml 的交互逻辑
    /// </summary>
    public partial class MedicalWordView : UserControl
    {
        public MedicalWordView()
        {
            InitializeComponent();
            this.Loaded += MedicalWordView_Loaded;
        }

        private void MedicalWordView_Loaded(object sender, RoutedEventArgs e)
        {
            GetMedicalWords();
            this.MouseLeftButtonDown += (o, ex) => ResetMedicalWord();
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ResetMedicalWord();
                ListBox lb = null;
                int mediacalWordID = -1;
                if (element.DataContext == null)
                {
                    lb = lt_diagnosis;
                    mediacalWordID = 0;
                }
                else if (element.DataContext is MedicalWord && element.DataContext is MedicalWord medicalWord)
                {
                    var cgb = ControlHelper.GetParentObject<CustomGroupBox>(element);
                    lb = ControlHelper.GetVisualChild<ListBox>(cgb);
                    mediacalWordID = medicalWord.MedicalWordID;
                }
                if (lb != null && mediacalWordID >= 0 && lb.ItemsSource is ObservableCollection<MedicalWord> medicalWords)
                {
                    var medicalWord = new MedicalWord() { ParentID = mediacalWordID, IsSelected = true };
                    medicalWords.Add(medicalWord);
                    lb.UpdateLayout();
                    lb.ScrollIntoView(medicalWord);
                    lb.SelectedValue = medicalWord;
                    var lbv = lb.ItemContainerGenerator.ContainerFromIndex(lb.Items.Count - 1) as ListBoxItem;
                    if (lbv != null)
                    {
                        var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                        tb.Focus();
                    }
                }
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetMedicalWords();
        }
        private void ModifyWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalWord template)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                if (template.MedicalWordID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"新建医学词库名称不能为空");
                        return;
                    }
                    var add = new MedicalWord { Name = tb.Text, ParentID = template.ParentID };
                    var result = loading.AsyncWait("新建医学词库中,请稍后", SocketProxy.Instance.AddMedicalWord(add));
                    if (result.IsSuccess)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        template.MedicalWordID = result.Content;
                        template.IsSelected = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"编辑医学词库名称不能为空");
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(template.Name))
                    {
                        var update = new MedicalWord
                        {
                            MedicalWordID = template.MedicalWordID,
                            Name = tb.Text,
                            ParentID = template.ParentID
                        };
                        var result = loading.AsyncWait("更新医学词库中,请稍后", SocketProxy.Instance.ModifyMedicalWord(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            template.IsSelected = false;
                        }
                        else
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"更新医学词库失败,{ result.Error }");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }

        private void StartModify_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalWord medicalWord)
            {
                ResetMedicalWord();
                var lb = ControlHelper.GetParentObject<ListBox>(element);
                lb.SelectedValue = medicalWord;
                medicalWord.IsSelected = true;
                var grid = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(grid);
                tb.SelectionStart = tb.Text.Length;
                tb.Focus();
            }
        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalWord template)
            {
                ResetMedicalWord();
                var lb = ControlHelper.GetParentObject<ListBox>(element);
                if (lb.ItemsSource is ObservableCollection<MedicalWord> medicalWords)
                {
                    if (!MsPrompt.ShowDialog("确认删除此条医学词库内容,是否继续?"))
                        return;
                    var result = loading.AsyncWait("删除医学词库中,请稍后", SocketProxy.Instance.RemoveMedicalWord(template.MedicalWordID));
                    if (!result.IsSuccess) Alert.ShowMessage(true, AlertType.Error, $"删除医学词库失败,{ result.Error }");
                    else medicalWords.Remove(template);
                }
            }
        }

        private void GetMedicalWords()
        {
            var result = loading.AsyncWait("获取医学词库中,请稍后", SocketProxy.Instance.GetMedicalWords());
            if (result.IsSuccess) 
                lt_diagnosis.ItemsSource = CacheHelper.SortMedicalWords(result.Content, 0);
            else 
                Alert.ShowMessage(true, AlertType.Error, $"获取医学词库失败,{ result.Error }");
        }
        

        private void ResetMedicalWord()
        {
            void ResetListBox(ListBox lb, MedicalWord medicalWord)
            {
                if (medicalWord.MedicalWordID == 0)
                    (lb.ItemsSource as ObservableCollection<MedicalWord>).Remove(medicalWord);
                else
                {
                    var index = lb.Items.IndexOf(medicalWord);
                    var lbi = lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    medicalWord.IsSelected = false;
                }
            }
            var listbox = ControlHelper.FindChildren<ListBox>(grid).FirstOrDefault(t => t.Items.OfType<MedicalWord>().Any(p => p.IsSelected));
            if (listbox != null)
            {
                var medicalWord = listbox.Items.OfType<MedicalWord>().FirstOrDefault(p => p.IsSelected);
                ResetListBox(listbox, medicalWord);
            }
        }

        private void ImportWord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dialog.Filter = "医学词库文件（*.json）|*.json";
            if (dialog.ShowDialog().Value)
            {
                var json = File.ReadAllText(dialog.FileName);
                var datas = JsonConvert.DeserializeObject<List<MedicalWord>>(json);
                if (datas == null || datas.Count == 0)
                    Alert.ShowMessage(true, AlertType.Error, "医学词库文件格式异常");
                else
                {
                    var result = loading.AsyncWait("导入医学词库中,请稍后", SocketProxy.Instance.ImportMedicalWord(json));
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Success, "导入医学词库成功");
                        GetMedicalWords();
                    }
                    else
                        Alert.ShowMessage(true, AlertType.Error, $"导入医学词库失败,{ result.Error }");
                }
            }
        }

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (lt_diagnosis.ItemsSource is ObservableCollection<MedicalWord> medicalWords)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var jsonSource = JsonConvert.SerializeObject(medicalWords);
                    var filePath = System.IO.Path.Combine(dialog.SelectedPath, $"医学词库_{ TimeHelper.ToUnixTime(DateTime.Now) }.json");
                    File.WriteAllText(filePath, jsonSource);
                    Alert.ShowMessage(true, AlertType.Success, "医学词库导出完成");
                }
            }
        }
    }
}