using Ms.Controls;
using MM.Medical.Client.Entities;
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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// AddWordItemView.xaml 的交互逻辑
    /// </summary>
    public partial class AddWordItemView : UserControl
    {
        private readonly BaseWordExtend word;
        private readonly Loading loading;
        private readonly int index;

        public AddWordItemView(BaseWordExtend word, int index, Loading loading)
        {
            InitializeComponent();
            if (index >= 0)  
                tb_name.Text = word.Items.ElementAt(index);
            this.word = word;
            this.index = index;
            this.loading = loading;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_name.Text))
            {
                var word_back = word.Copy();
                if (this.index >= 0)
                {
                    word_back.Items[this.index] = tb_name.Text.Trim();
                    word_back.Content = string.Join(",", word_back.Items);
                    var result = loading.AsyncWait("修改明细名称中,请稍后", SocketProxy.Instance.ModifyBaseWord(word_back));
                    if (result.IsSuccess)
                    {
                        word.Items[this.index] = tb_name.Text.Trim();
                        this.Close(true);
                    }
                    else MsWindow.ShowDialog($"修改明细名称失败,{ result.Error }", "软件提示");
                }
                else
                {
                    word_back.Items.Add(tb_name.Text.Trim());
                    var result = loading.AsyncWait("添加明细名称中,请稍后", SocketProxy.Instance.ModifyBaseWord(word_back));
                    if (result.IsSuccess)
                    {
                        word.Items.Add(tb_name.Text.Trim());
                        this.Close(true);
                    }
                    else MsWindow.ShowDialog($"添加明细名称失败,{ result.Error }", "软件提示");
                }
            }
            else MsWindow.ShowDialog("明细名称不能为空", "软件提示");
        }
    }
}
