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

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// PreviewMediaView.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewMediaView : Window
    {
        private ExaminationMedia media;
        public PreviewMediaView()
        {
            InitializeComponent();
        }

        public PreviewMediaView(ExaminationMedia media) : this()
        { 
            this.media = media;
            this.Loaded += PreviewMediaView_Loaded;
        }

        private void PreviewMediaView_Loaded(object sender, RoutedEventArgs e)
        {
            gd.Width = this.ActualWidth * 0.8;
            gd.UpdateLayout();
            if (media.MediaType == MediaType.Image)
            {
                video.Visibility = Visibility.Collapsed;
                var remoteAddress = SocketProxy.Instance.GetFileRounter() + media.Path;
                img.Source = new BitmapImage(new Uri(remoteAddress, UriKind.Absolute));
            }
            else
            {
                var remoteAddress = SocketProxy.Instance.GetFileRounter() + media.VideoPath;
                video.SetSource(remoteAddress, true);
            }
            gd.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            video.Stop();
            this.Close();
        }
    }
}
