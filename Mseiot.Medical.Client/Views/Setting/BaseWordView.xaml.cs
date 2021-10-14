using Ms.Controls;
using Mseiot.Medical.Client.Entities;
using Mseiot.Medical.Client.Views.Component;
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
        }

        private void GetBaseWords()
        {
            var result = loading.AsyncWait("获取基础词库中,请稍后", SocketProxy.Instance.GetBaseWords());
            if (result.IsSuccess) lb_words.ItemsSource = result.Content.Select(t => new BaseWordExtend(t));
            else MsWindow.ShowDialog($"获取基础词库失败,{ result.Error }", "软件提示");
        }

        private void ModifyWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BaseWordExtend word)
            {
                var view = new AddWordView(word, this.loading);
                sp.ShowDialog("编辑词条", view);
            }
        }

        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is BaseWordExtend word)
            {
                var result = loading.AsyncWait("删除词条中,请稍后", SocketProxy.Instance.RemoveBaseWord(word.BaseWordID));
                if (result.IsSuccess) GetBaseWords();
                else MsWindow.ShowDialog($"删除词条失败,{ result.Error }", "软件提示");
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            var word = new BaseWord();
            var view = new AddWordView(word, this.loading);
            if (sp.ShowDialog("添加词条", view))
                GetBaseWords();
        }
        private void RefreshWord_Click(object sender, RoutedEventArgs e)
        {
            GetBaseWords();
        }

        private void ModifyItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word && sender is FrameworkElement element && element.DataContext is string item)
            {
                var view = new AddWordItemView(word, word.Items.IndexOf(item), this.loading);
                sp.ShowDialog("编辑词条明细", view);
            }
            else MsWindow.ShowDialog("请选择需要编辑明细的词条", "软件提示");
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word && sender is FrameworkElement element && element.DataContext is string item)
            {
                var word_back = word.Copy();
                word_back.Items.Remove(item);
                var result = loading.AsyncWait("删除词条明细中,请稍后", SocketProxy.Instance.ModifyBaseWord(word_back));
                if (result.IsSuccess) word.Items.Remove(item);
                else MsWindow.ShowDialog($"删除词条明细失败,{ result.Error }", "软件提示");
            }
            else MsWindow.ShowDialog("请选择需要编辑明细的词条", "软件提示");
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (lb_words.SelectedValue is BaseWordExtend word)
            {
                var view = new AddWordItemView(word, -1, this.loading);
                sp.ShowDialog("添加词条明细", view);
            }
            else MsWindow.ShowDialog("请选择需要添加明细的词条", "软件提示");
        }

    }
}
