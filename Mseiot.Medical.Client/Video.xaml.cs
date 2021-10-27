using Ms.Controls;
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
using System.Windows.Shapes;

namespace MM.Medical.Client
{
    /// <summary>
    /// Video.xaml 的交互逻辑
    /// </summary>
    public partial class Video : Window
    {
        public Video()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Loaded += Video_Loaded; ;
        }

        private void Video_Loaded(object sender, RoutedEventArgs e)
        {
            OpenCvSharp.VideoCapture videoCapture = new OpenCvSharp.VideoCapture(0, OpenCvSharp.VideoCaptureAPIs.DSHOW);
            var a = videoCapture.IsOpened();
            videoCapture.Set(OpenCvSharp.VideoCaptureProperties.FrameWidth, 1920);
            videoCapture.Set(OpenCvSharp.VideoCaptureProperties.FrameHeight, 1080);
            Task.Run(() =>
            {
                while(true){
                    OpenCvSharp.Mat mat = videoCapture.RetrieveMat();
                    byte[] buffer = mat.ToBytes(".jpg");
                    this.Dispatcher.Invoke(() =>
                    {
                        ImageSource = buffer;
                    });
                    Console.WriteLine("{0} {1}", mat.Width, mat.Height);
                }
            });
        }

        public object ImageSource
        {
            get { return (object)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(object), typeof(Video), new PropertyMetadata(null));


    }
}
