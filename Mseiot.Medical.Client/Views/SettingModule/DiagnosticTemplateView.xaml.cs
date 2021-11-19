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
    /// DiagnosticTemplateView.xaml 的交互逻辑
    /// </summary>
    public partial class DiagnosticTemplateView : UserControl
    {
        public DiagnosticTemplateView()
        {
            InitializeComponent();
            this.Loaded += DiagnosticTemplateView_Loaded;
        }

        private void DiagnosticTemplateView_Loaded(object sender, RoutedEventArgs e)
        {
            GetMedicalTemplates();
            this.MouseLeftButtonDown += (o, ex)=> ResetMedicalTemplate();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_template.SelectedValue is MedicalTemplate parent)
            {
                ResetMedicalTemplate();
                var template = new MedicalTemplate { IsSelected = true, ParentID = parent.MedicalTemplateID };
                (lb_item.ItemsSource as ObservableCollection<MedicalTemplate>).Add(template);
                lb_item.SelectedValue = template;
                lb_item.ScrollIntoView(template);
                if (lb_item.ItemContainerGenerator.ContainerFromIndex(lb_item.Items.Count - 1) is ListBoxItem lbi)
                {
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.Focus();
                }
            }
        }

        private void AddTemplate_Click(object sender, RoutedEventArgs e)
        {
            ResetMedicalTemplate();
            var template = new MedicalTemplate { IsSelected = true };
            (lb_template.ItemsSource as ObservableCollection<MedicalTemplate>).Add(template);
            lb_template.SelectedValue = template;
            lb_template.ScrollIntoView(template);
            if (lb_template.ItemContainerGenerator.ContainerFromIndex(lb_template.Items.Count - 1) is ListBoxItem lbi)
            {
                var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                tb.Focus();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetMedicalTemplates();
        }
        private void StartModify_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalTemplate template)
            {
                ResetMedicalTemplate();
                if (template.ParentID == 0) lb_template.SelectedValue = template;
                else lb_item.SelectedValue = template;
                template.IsSelected = true;
                var grid = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(grid);
                tb.SelectionStart = tb.Text.Length;
                tb.Focus();
            }
        }
        private void ModifyTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalTemplate template)
            {
                var parent = ControlHelper.GetParentObject<Grid>(element);
                var tb = ControlHelper.GetVisualChild<TextBox>(parent);
                if (template.MedicalTemplateID == 0)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"新建模板项名称不能为空", "软件提示");
                        return;
                    }
                    var add = new MedicalTemplate { Name = tb.Text, ParentID = template.ParentID };
                    var result = loading.AsyncWait("新建模板项中,请稍后", SocketProxy.Instance.AddMedicalTemplate(add));
                    if (result.IsSuccess)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        template.MedicalTemplateID = result.Content;
                        template.IsSelected = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        MsWindow.ShowDialog($"编辑模板项名称不能为空", "软件提示");
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        return;
                    }
                    if (!tb.Text.Equals(template.Name))
                    {
                        var update = new MedicalTemplate
                        {
                            MedicalTemplateID = template.MedicalTemplateID,
                            Moddia = template.Moddia,
                            Name = tb.Text,
                            Modsee = template.Modsee,
                            ParentID = template.ParentID
                        };
                        var result = loading.AsyncWait("更新模板项中,请稍后", SocketProxy.Instance.ModifyMedicalTemplate(update));
                        if (result.IsSuccess)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            template.IsSelected = false;
                        }
                        else
                        {
                            MsWindow.ShowDialog($"更新模板项失败,{ result.Error }", "软件提示");
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }
        private void RemoveTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalTemplate template)
            {
                ResetMedicalTemplate();
                if (MsPrompt.ShowDialog("确定删除此模板目录内容,是否继续?"))
                {
                    if (template.ParentID == 0)
                    {
                        var result = loading.AsyncWait("删除模板项中,请稍后", SocketProxy.Instance.RemoveMedicalTemplate(template.MedicalTemplateID));
                        if (!result.IsSuccess) 
                            MsWindow.ShowDialog($"删除模板信息失败,{ result.Error }", "软件提示");
                        else 
                            (lb_template.ItemsSource as ObservableCollection<MedicalTemplate>).Remove(template);
                    }
                    else
                    {
                        var result = loading.AsyncWait("删除模板信息中,请稍后", SocketProxy.Instance.RemoveMedicalTemplate(template.MedicalTemplateID));
                        if (!result.IsSuccess) 
                            MsWindow.ShowDialog($"删除模板信息失败,{ result.Error }", "软件提示");
                        else 
                            (lb_item.ItemsSource as ObservableCollection<MedicalTemplate>).Remove(template);
                    }
                }
            }
        }
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is MedicalTemplate template)
            {
                ResetMedicalTemplate();
                var modify = template.Copy();
                modify.Moddia = tb_moddia.Text.Trim();
                modify.Modsee = tb_modsee.Text.Trim();
                var result = loading.AsyncWait("修改模板信息中,请稍后", SocketProxy.Instance.ModifyMedicalTemplate(modify));
                if (result.IsSuccess)
                {
                    tb_modsee.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    tb_moddia.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
                else 
                    MsWindow.ShowDialog($"修改模板信息失败,{ result.Error }", "软件提示");
            }
        }

        private void GetMedicalTemplates()
        {
            var result = loading.AsyncWait("获取模板信息中,请稍后", SocketProxy.Instance.GetMedicalTemplates());
            if (result.IsSuccess) 
                lb_template.ItemsSource = CacheHelper.SortMedicalTemplates(result.Content, 0);
            else 
                MsWindow.ShowDialog($"获取模板信息失败,{ result.Error }", "软件提示");
        }

        private void ResetMedicalTemplate()
        {
            void ResetListBox(ListBox lb, MedicalTemplate template)
            {
                if (template.MedicalTemplateID == 0)
                    (lb.ItemsSource as ObservableCollection<MedicalTemplate>).Remove(template);
                else
                {
                    var index = lb.Items.IndexOf(template);
                    var lbi = lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var tb = ControlHelper.GetVisualChild<TextBox>(lbi);
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    template.IsSelected = false;
                }
            }
            var medicalTemplate = lb_template.Items.OfType<MedicalTemplate>().FirstOrDefault(t => t.IsSelected);
            if (medicalTemplate != null)
                ResetListBox(lb_template, medicalTemplate);
            else
            {
                medicalTemplate = lb_item.Items.OfType<MedicalTemplate>().FirstOrDefault(t => t.IsSelected);
                if (medicalTemplate != null)
                    ResetListBox(lb_item, medicalTemplate);
            }
        }

        private void ExportTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (lb_template.ItemsSource is ObservableCollection<MedicalTemplate> medicalTemplates)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var jsonSource = JsonConvert.SerializeObject(medicalTemplates);
                    var filePath = System.IO.Path.Combine(dialog.SelectedPath, $"诊断模板_{ TimeHelper.ToUnixTime(DateTime.Now) }.json");
                    File.WriteAllText(filePath, jsonSource);
                    Alert.ShowMessage(true, AlertType.Success, "诊断模板导出完成");
                }
            }
        }

        private void ImportTemplate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dialog.Filter = "诊断模板文件（*.json）|*.json";
            if (dialog.ShowDialog().Value)
            {
                var json = File.ReadAllText(dialog.FileName);
                var datas = JsonConvert.DeserializeObject<List<MedicalTemplate>>(json);
                if (datas == null || datas.Count == 0)
                    Alert.ShowMessage(true, AlertType.Error, "诊断模板文件格式异常");
                else
                {
                    var result = loading.AsyncWait("导入诊断模板中,请稍后", SocketProxy.Instance.ImportMedicalTemplate(json));
                    if (result.IsSuccess)
                    {
                        Alert.ShowMessage(true, AlertType.Success, "导入诊断模板成功");
                        GetMedicalTemplates();
                    }
                    else
                        Alert.ShowMessage(true, AlertType.Error, $"导入诊断模板失败,{ result.Error }");
                }
            }
        }
    }
}