using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Controls.Core;
using Ms.Libs.Models;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ExaminationVideoVIew.xaml 的交互逻辑
    /// </summary>
    public partial class ExaminationVideoView : MsWindow, INotifyPropertyChanged
    {
        public ObservableCollection<ExaminationMedia> ExaminationMedias { get; set; }
        public Examination examination { get; set; }
        private SystemSetting systemSetting = CacheHelper.SystemSetting;
        private MediaPlayer player;
        private AutoResetEvent are;
        private bool isRecording;

        public bool IsRecording
        {
            get { return isRecording; }
            set 
            { 
                isRecording = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsRecording)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExaminationVideoView()
        { 
            InitializeComponent();
            this.ExaminationMedias = new ObservableCollection<ExaminationMedia>();
            SetWindowLocation();
            this.Loaded += ExaminationVideoView_Loaded;
            this.DataContext = this;
        }


        public void SetExam(Examination examination)
        {
            ExaminationMedias.Clear();
            this.examination = examination;
            if (examination != null)
            {
                ExaminationMedias.AddRange(examination.Images);
                ExaminationMedias.AddRange(examination.Videos);
            }
        }

        private void SetWindowLocation()
        {
            if (Screen.AllScreens.Length > 1)
            {
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    var item = Screen.AllScreens[i];
                    if (!item.Primary)
                    {
                        Width = item.Bounds.Width;
                        Height = item.Bounds.Height;
                        Top = item.WorkingArea.Top;
                        Left = item.WorkingArea.Left;
                        break;
                    }
                }
            }
            video.SetSource(CacheHelper.EndoscopeDeviceID, true, OpenCvSharp.VideoCaptureAPIs.DSHOW);
        }

        private void ExaminationVideoView_Loaded(object sender, RoutedEventArgs e)
        {
            this.player = new MediaPlayer();
            if (!string.IsNullOrEmpty(systemSetting.CutshotSound))
            {
                var soundPath = SocketProxy.Instance.GetFileRounter() + systemSetting.CutshotSound;
                player.Open(new Uri(soundPath, UriKind.Absolute));
            }
            else
            {
                player.Open(new Uri("screenshot.mp3", UriKind.Relative));
            }
            if (!string.IsNullOrEmpty(systemSetting.CutshotKeyboard))
            {
                RegistHotKey(systemSetting.CutshotKeyboard, Shotcut);
            }
            if (!string.IsNullOrEmpty(systemSetting.RecordKeyboard))
            {
                RegistHotKey(systemSetting.RecordKeyboard, Record);
                this.are = new AutoResetEvent(true);
            }
        }

        private void RegistHotKey(string shotcutKey, Action action)
        {
            var items = shotcutKey.Split('+');
            uint modifiers = 0;
            if (items.Length == 2)
            {
                var modifierStr = items[0].ToUpper();
                if (modifierStr == "CTRL" || modifierStr == "CONTROL")
                    modifiers |= HotKeyManager.MOD_CONTROL;
                else if (modifierStr == "ALT")
                    modifiers |= HotKeyManager.MOD_ALT;
                else if (modifierStr == "SHIFT")
                    modifiers |= HotKeyManager.MOD_SHIFT;
            }
            var key = (Key)Enum.Parse(typeof(Key), items[items.Length - 1], true);
            var id = key.GetHashCode();
            HotKeyManager.Instance.RegisterHotKey(this, id, key, modifiers, action);
            this.Closed += (_, ex) => HotKeyManager.Instance.UnregisterHotKey(this, id);
        }

        private async void Shotcut()
        {
            if (examination == null)
                return;
            if (examination.Images == null)
            {
                examination.Images = new ObservableCollection<ExaminationMedia>();
            }
            if (examination.Images.Count >= systemSetting.CutshotImageCount)
            {
                Alert.ShowMessage(true, AlertType.Warning, "采集图片数量超过上限");
                return;
            }
            var image = video.Shotcut();
            if (image != null && image.Length > 0)
            {
                player.Play();
                var media = new ExaminationMedia
                {
                    Buffer = image,
                    ExaminationID = examination.ExaminationID,
                    MediaType = MediaType.Image,
                };
                examination.Images.Add(media);
                ExaminationMedias.Insert(0, media);
                var result = await SocketProxy.Instance.HttpProxy.UploadFile<string>(image);
                if (result.IsSuccess)
                {
                    media.Path = result.Content;
                    var result2 = await SocketProxy.Instance.AddExaminationMedia(media);
                    if (result2.IsSuccess)
                    {
                        media.ExaminationMediaID = result2.Content;
                        media.Buffer = null;
                        media.ErrorMsg = null;
                    }
                    else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传数据失败,{result2.Error}");
                }
                else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传图片失败,{result.Error}");
                player.Position = TimeSpan.Zero;
            }
        }

        private async void Record()
        {
            are.WaitOne();
            if (this.isRecording)
            {
                var result = await StopRecord();
                this.IsRecording = !result;
            }
            else
            {
                var result = await StartRecord();
                this.IsRecording = result;

            }
            are.Set();
        }

        private async Task<bool> StartRecord()
        {
            if (examination == null)
                return false;
            if (examination.Videos.Count >= systemSetting.MediaCount)
            {
                Alert.ShowMessage(true, AlertType.Warning, "采集视频数量超过上限");
                return false;
            }
            var image = video.Shotcut();
            if (image != null && image.Length > 0)
            {
                var media = new ExaminationMedia
                {
                    Buffer = image,
                    ExaminationID = examination.ExaminationID,
                    MediaType = MediaType.Video,
                };
                examination.Videos.Add(media);
                ExaminationMedias.Insert(0, media);
                var result = await SocketProxy.Instance.HttpProxy.UploadFile<string>(image);
                if (result.IsSuccess)
                {
                    media.Path = result.Content;
                    var result2 = await SocketProxy.Instance.AddExaminationMedia(media);
                    if (result2.IsSuccess)
                    {
                        media.ExaminationMediaID = result2.Content;
                        media.ErrorMsg = null;
                        var filePath = System.IO.Path.Combine(CacheHelper.VideoPath, TimeHelper.ToUnixTime(DateTime.Now).ToString() + ".avi");
                        if (video.StartRecord(filePath))
                        { 
                            media.LocalVideoPath = filePath;
                            return true;
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, "开启录像失败"));
                        }
                    }
                    else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频数据失败,{result2.Error}");
                }
                else
                {
                    this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频预览图片失败,{result.Error}");
                }
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, "预览图获取失败,录像已停止");
            }
            return false;
        }

        private async Task<bool> StopRecord()
        {
            if (examination == null)
                return false;
            if (video.StopRecord())
            {
                var media = examination.Videos.FirstOrDefault(t => !string.IsNullOrEmpty(t.LocalVideoPath));
                var result = await SocketProxy.Instance.HttpProxy.UploadFile<string>(media.LocalVideoPath);
                if (result.IsSuccess)
                {
                    media.VideoPath = result.Content;
                    var result2 = await SocketProxy.Instance.ModifyExaminationMedia(media);
                    if (!result2.IsSuccess)
                        this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频文件失败,{result2.Error}");
                    else
                    {
                        File.Delete(media.LocalVideoPath);
                        media.LocalVideoPath = null;
                        return true;
                    }
                }
                else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频文件失败,{result.Error}");
            }
            else
            {
                Alert.ShowMessage(true, AlertType.Error, "停止录像失败");
            }
            return false;
        }

        private void Shotcut_Click(object sender, RoutedEventArgs e)
        {
            Shotcut();
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            Record();
        }
    }
}
