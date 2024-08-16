using Ms.Controls;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private object videoWriterLocker = new object();
        private ManualResetEvent resetEvent;
        private VideoCapture videoCapture;
        private VideoWriter videoWriter;
        private Task playTask;

        public Video()
        {
            InitializeComponent();
            this.ImageSource = new BitmapImage(new Uri("/MM.Medical.Share;component/Images/no_signal.jpg", UriKind.Relative));
            this.DataContext = this;
        }

        public async void SetSource(object videoSource, bool autoPlay = false, VideoCaptureAPIs captureAPIs = VideoCaptureAPIs.ANY)
        {
            var isRealTime = false;
            await this.Stop();
            this.tokenSource = new CancellationTokenSource();
            this.resetEvent = new ManualResetEvent(autoPlay);
            if (videoSource is int deviceId)
            { 
                this.videoCapture = new VideoCapture(deviceId, captureAPIs);
                isRealTime = true;
            }
            else
                this.videoCapture = new VideoCapture(videoSource.ToString(), captureAPIs);
            if (videoCapture.IsOpened())
            {
                videoCapture.Set(VideoCaptureProperties.FrameWidth, 1920);
                videoCapture.Set(VideoCaptureProperties.FrameHeight, 1080);
                var token = tokenSource.Token;
                this.playTask = Task.Run(() =>
                {
                    var index = 0;
                    while (!token.IsCancellationRequested && videoCapture.Grab())
                    {
                        resetEvent.WaitOne();
                        var mat = videoCapture.RetrieveMat();
                        if (mat.Empty())
                            break;
                        lock (this.videoWriterLocker)
                        {
                            if (this.videoWriter != null && !videoWriter.IsDisposed)
                                videoWriter.Write(mat);
                        }
                        if (index++ % 2 == 0 || !isRealTime)
                        {
                            this.Dispatcher.Invoke(() => { ImageSource = mat.ToMemoryStream(".jpg"); });
                            if (index > 1000000)
                                index = 0;
                        }
                        mat.Dispose();
                        resetEvent.Set();
                    }
                }, token);
            }
        }

        public bool SettingROI
        {
            get { return (bool)GetValue(SettingROIProperty); }
            set { SetValue(SettingROIProperty, value); }
        }

        public static readonly DependencyProperty SettingROIProperty =
            DependencyProperty.Register("SettingROI", typeof(bool), typeof(Video), new PropertyMetadata(false, new PropertyChangedCallback(SettingROIPropertyChanged)));

        private static void SettingROIPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Video video)
            {
                video.canvas.Visibility = video.SettingROI ? Visibility.Visible : Visibility.Collapsed;
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
            if (this.videoCapture == null || videoCapture.IsDisposed || playTask == null || playTask.IsCompleted)
                return false;
            else this.StopRecord();
            lock (this.videoWriterLocker)
            {
                var fps = videoCapture.Fps == 0 ? 10 : videoCapture.Fps;
                this.videoWriter = new VideoWriter(videoFullPath, FourCC.MJPG, fps, new OpenCvSharp.Size(videoCapture.FrameWidth, videoCapture.FrameHeight));
            }
            return true;
        }

        public byte[] Shotcut()
        {
            if (this.ImageSource is byte[] datas)
                return datas;
            else if (this.ImageSource is MemoryStream ms)
                return ms.ToArray();
            return null;
        }

        public bool StopRecord()
        {
            lock (this.videoWriterLocker)
            {
                if (this.videoWriter != null && !videoWriter.IsDisposed)
                {
                    videoWriter.Dispose();
                    videoWriter = null;
                }
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
            this.Dispatcher.Invoke(() => this.ImageSource = new BitmapImage(new Uri("/MM.Medical.Share;component/Images/no_signal.jpg", UriKind.Relative)));
        }

        #region 绘制

        float x1;
        float y1;
        float x2;
        float y2;
        float x3;
        float y3;
        float x4;
        float y4;
        public event EventHandler<PlayCoreDraw> NotifyDrawComplete;

        PlayerCoreDrawType playerCoreDrawType = PlayerCoreDrawType.Rectangle;

        public void SetDrawStatus(PlayerCoreDrawType playerCoreDrawType = PlayerCoreDrawType.None)
        {
            this.playerCoreDrawType = playerCoreDrawType;
            if (playerCoreDrawType == PlayerCoreDrawType.None)
            {
                path.Data = null;
                canvas.Visibility = Visibility.Collapsed;
            }
            else
            {
                canvas.Visibility = Visibility.Visible;
            }
        }

        private void DrawStart_Click(object sender, MouseButtonEventArgs e)
        {
            var startPoint = e.GetPosition(canvas);
            x1 = (float)startPoint.X;
            y1 = (float)startPoint.Y;
        }

        private void Drawing_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var endPoint = e.GetPosition(canvas);
                x2 = (float)endPoint.X;
                y2 = (float)endPoint.Y;
                Draw();
            }
        }

        private void Draw()
        {
            switch (playerCoreDrawType)
            {
                case PlayerCoreDrawType.Rectangle:
                    x3 = x1;
                    y3 = y2;
                    x4 = x2;
                    y4 = y1;
                    path.Data = Geometry.Parse(string.Format("M{0},{1} {2},{3} {4},{5} {6},{7} Z", x1, y1, x3, y3, x2, y2, x4, y4));
                    break;
                case PlayerCoreDrawType.Line:
                    path.Data = Geometry.Parse(string.Format("M{0},{1} {2},{3}", x1, y1, x2, y2));
                    break;
            }
        }

        public void Draw(float x1, float y1, float x2, float y2)
        {
            this.x1 = (float)img.ActualWidth * x1;
            this.y1 = (float)img.ActualHeight * y1;
            this.x2 = (float)img.ActualWidth * x2;
            this.y2 = (float)img.ActualHeight * y2;
            Draw();
        }

        private void DrawComplete_Click(object sender, MouseButtonEventArgs e)
        {
            float lastX1 = x1;
            float lastY1 = y1;
            float lastX2 = x2;
            float lastY2 = y2;
            switch (playerCoreDrawType)
            {
                case PlayerCoreDrawType.Rectangle:
                    lastX1 = new List<float>() { x1, x2, x3, x4 }.Min();
                    lastY1 = new List<float>() { y1, y2, y3, y4 }.Min();
                    lastX2 = new List<float>() { x1, x2, x3, x4 }.Max();
                    lastY2 = new List<float>() { y1, y2, y3, y4 }.Max();
                    break;
                case PlayerCoreDrawType.Line:
                    break;
            }
            PlayCoreDraw playCoreDraw = new PlayCoreDraw()
            {
                X1 = lastX1 / (float)canvas.ActualWidth,
                Y1 = lastY1 / (float)canvas.ActualHeight,
                X2 = lastX2 / (float)canvas.ActualWidth,
                Y2 = lastY2 / (float)canvas.ActualHeight,
            };
            NotifyDrawComplete?.Invoke(this, playCoreDraw);
        }

        #endregion
    }
}
