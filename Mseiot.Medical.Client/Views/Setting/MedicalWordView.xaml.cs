using Ms.Controls;
using Ms.Controls.Core;
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

namespace Mseiot.Medical.Client.Views
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
            if (sender is FrameworkElement element && element.DataContext is MedicalWord word)
            {
                var cgb = ControlHelper.GetParentObject<CustomGroupBox>(element);
                var lb = ControlHelper.GetVisualChild<ListBox>(cgb);
                if (lb.ItemsSource is ObservableCollection<MedicalWord> medicalWords)
                {
                    var medicalWord = new MedicalWord() { ParentID = word.MedicalWordID, IsSelected = true };
                    medicalWords.Add(medicalWord);
                    lb.ScrollIntoView(medicalWord);
                    lb.SelectedValue = medicalWord;
                    var lbv = lb.ItemContainerGenerator.ContainerFromIndex(lb.Items.Count - 1) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbv);
                    tb.Focus();
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
                        MsWindow.ShowDialog($"新建医疗信息项名称不能为空", "软件提示");
                        return;
                    }
                    var add = new MedicalWord { Name = tb.Text, ParentID = template.ParentID };
                    var result = loading.AsyncWait("新建医疗信息项中,请稍后", SocketProxy.Instance.AddMedicalWord(add));
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
                        MsWindow.ShowDialog($"编辑医疗信息项名称不能为空", "软件提示");
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
                        var result = loading.AsyncWait("更新医疗信息项中,请稍后", SocketProxy.Instance.ModifyMedicalWord(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            template.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新医疗信息项失败,{ result.Error }", "软件提示");
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
                    if (template.ParentID == 0)
                    {
                        if (!MsPrompt.ShowDialog("删除医疗信息项将清空其列表中的全部子项,是否继续"))
                            return;
                    }
                    var result = loading.AsyncWait("删除医疗信息项中,请稍后", SocketProxy.Instance.RemoveMedicalWord(template.MedicalWordID));
                    if (!result.IsSuccess) MsWindow.ShowDialog($"删除医疗信息失败,{ result.Error }", "软件提示");
                    else medicalWords.Remove(template);
                }
            }
        }

        private void GetMedicalWords()
        {
            var result = loading.AsyncWait("获取医疗信息中,请稍后", SocketProxy.Instance.GetMedicalWords());
            if (result.IsSuccess) lt_diagnosis.ItemsSource = SortMedicalWords(result.Content, 0);
            else MsWindow.ShowDialog($"获取医疗信息失败,{ result.Error }", "软件提示");
        }
        private ObservableCollection<MedicalWord> SortMedicalWords(IEnumerable<MedicalWord> words, int parentId)
        {
            var results = new ObservableCollection<MedicalWord>();
            foreach (var word in words)
            {
                if (word.ParentID == parentId)
                {
                    word.MedicalWords = SortMedicalWords(words, word.MedicalWordID);
                    results.Add(word);
                }
            }
            return results;
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
    }
}