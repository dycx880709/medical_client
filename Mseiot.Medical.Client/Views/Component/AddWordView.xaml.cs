using Ms.Controls;
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

namespace Mseiot.Medical.Client.Views.Component
{
    /// <summary>
    /// AddWordView.xaml 的交互逻辑
    /// </summary>
    public partial class AddWordView : UserControl
    {
        private Loading loading;
        private BaseWord originWord;
        private BaseWord word;
        public AddWordView(BaseWord word, Loading loading)
        {
            InitializeComponent();
            this.originWord = word;
            this.word = word.Copy();
            this.loading = loading;
            this.DataContext = this.word;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (word.BaseWordID != 0)
            {
                var result = loading.AsyncWait("修改词条中,请稍后", SocketProxy.Instance.ModifyBaseWord(this.word));
                if (result.IsSuccess) 
                {
                    word.CopyTo(originWord);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"修改词条失败,{ result.Error }", "软件提示");
            }
            else
            {
                var result = loading.AsyncWait("添加词条中,请稍后", SocketProxy.Instance.AddBaseWord(this.word));
                if (result.IsSuccess)
                {
                    word.BaseWordID = result.Content;
                    word.CopyTo(originWord);
                    this.Close(true);
                }
                else MsWindow.ShowDialog($"添加词条失败,{ result.Error }", "软件提示");
            }
        }
    }
}
