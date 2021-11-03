using Ms.Controls;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// Video.xaml 的交互逻辑
    /// </summary>
    public partial class Video : UserControl, IDisposable
    {
        private CancellationTokenSource tokenSource;
        private ManualResetEvent resetEvent;
        private VideoCapture videoCapture;
        private VideoWriter videoWriter;
        private Task playTask;

        public Video()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public async void SetSource(object videoSource, VideoCaptureAPIs captureAPIs = VideoCaptureAPIs.ANY)
        {
            await this.Stop();
            this.tokenSource = new CancellationTokenSource();
            this.resetEvent = new ManualResetEvent(false);
            if (videoSource is int deviceId)
                this.videoCapture = new VideoCapture(deviceId, captureAPIs);
            else
                this.videoCapture = new VideoCapture(videoSource.ToString(), captureAPIs);
            if (videoCapture.IsOpened())
            {
                videoCapture.Set(VideoCaptureProperties.FrameWidth, 1920);
                videoCapture.Set(VideoCaptureProperties.FrameHeight, 1080);
                var token = tokenSource.Token;
                this.playTask = Task.Run(() =>
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                            return;
                        resetEvent.WaitOne();
                        var mat = videoCapture.RetrieveMat();
                        if (this.videoWriter != null && !videoWriter.IsDisposed)
                            videoWriter.Write(mat);
                        byte[] buffer = mat.ToBytes(".jpg");
                        this.Dispatcher.Invoke(() => { ImageSource = buffer; });
                        Console.WriteLine("{0} {1}", mat.Width, mat.Height);
                        mat.Dispose();
                        resetEvent.Set();
                    }
                }, token);
            }
        }

        public object ImageSource
        {
            get { return (object)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(object), typeof(Video), new PropertyMetadata(null));

        public bool StartRecord(string videoFullPath)
        {
            if (this.videoCapture == null || videoCapture.IsDisposed)
                return false;
            else this.StopRecord();
            this.videoWriter = new VideoWriter(videoFullPath, FourCC.MPG4, videoCapture.Fps, new OpenCvSharp.Size(videoCapture.FrameWidth, videoCapture.FrameHeight));
            return true;
        }

        public bool StopRecord()
        {
            if (this.videoWriter != null && !videoWriter.IsDisposed)
            {
                videoWriter.Dispose();
                videoWriter = null;
            }
            return true;
        }

        public void Start()
        {
            resetEvent.Set();
        }

        public void Pause()
        {
            this.ImageSource = null;
            resetEvent.WaitOne();
        }

        public async Task Stop()
        {
            if (this.playTask != null && !playTask.IsCompleted)
            {
                this.ImageSource = null;
                tokenSource.Cancel();
                resetEvent.Set();
                await playTask;
                resetEvent.Dispose();
                videoCapture.Dispose();
                this.StopRecord();
                this.tokenSource = null;
                this.resetEvent = null;
                this.videoCapture = null;
                this.playTask = null;
            }
        }

        public async void Dispose()
        {
            await Stop();
        }
    }
}
