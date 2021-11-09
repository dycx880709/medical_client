﻿using MM.Medical.Client.Core;
using Ms.Controls;
using Ms.Controls.Core;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ExaminationPartView.xaml 的交互逻辑
    /// </summary>
    public partial class ExaminationPartView : UserControl
    {
        public Examination SelectedExamination
        {
            get { return (Examination)GetValue(SelectedExaminationProperty); }
            set { SetValue(SelectedExaminationProperty, value); }
        }
        public bool IsFullScreen
        {
            get { return (bool)GetValue(IsFullScreenProperty); }
            set { SetValue(IsFullScreenProperty, value); }
        }
        public ExaminationMedia SelectedMedia
        {
            get { return (ExaminationMedia)GetValue(SelectedMediaProperty); }
            set { SetValue(SelectedMediaProperty, value); }
        }
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public Loading Loading
        {
            get { return (Loading)GetValue(LoadingProperty); }
            set { SetValue(LoadingProperty, value); }
        }

        public static readonly DependencyProperty SelectedExaminationProperty = DependencyProperty.Register("SelectedExamination", typeof(Examination), typeof(ExaminationPartView), new PropertyMetadata(null));
        public static readonly DependencyProperty LoadingProperty = DependencyProperty.Register("Loading", typeof(Loading), typeof(ExaminationPartView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ExaminationPartView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedMediaProperty = DependencyProperty.Register("SelectedMedia", typeof(ExaminationMedia), typeof(ExaminationPartView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ExaminationPartView), new PropertyMetadata(false));

        public IEnumerable<MedicalWord> OriginMedicalWords { get; set; }
        public IEnumerable<MedicalWord> MedicalWords { get; set; }
        public List<string> BodyParts { get; set; }

        public ExaminationPartView()
        {
            InitializeComponent();
            this.Loaded += ExaminationPartView_Loaded;
        }

        private void ExaminationPartView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ExaminationPartView_Loaded;
            this.Loading = this.Loading ?? this.cl;
            GetBaseWords();
            GetMedicalDatas();
            this.DataContext = this;
        }

        private void GetMedicalDatas()
        {
            var result1 = Loading.AsyncWait("获取诊断模板中,请稍后", SocketProxy.Instance.GetMedicalTemplates());
            if (result1.IsSuccess) tv_template.ItemsSource = CacheHelper.SortMedicalTemplates(result1.Content, 0);
            else Alert.ShowMessage(false, AlertType.Error, $"获取诊断模板失败,{ result1.Error }");
            var result2 = Loading.AsyncWait("获取医学词库中,请稍后", SocketProxy.Instance.GetMedicalWords());
            if (result2.IsSuccess)
            {
                this.OriginMedicalWords = result2.Content;
                this.MedicalWords = CacheHelper.SortMedicalWords(this.OriginMedicalWords, 0);
            }
            else Alert.ShowMessage(false, AlertType.Error, $"获取医学词库失败,{ result2.Error }");
        }

        private void GetBaseWords()
        {
            var result = Loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetBaseWords(
                "HIV",
                "HCV",
                "HBasg",
                "组织采取",
                "细胞采取",
                "插入途径",
                "检查体位",
                "麻醉方法",
                "检查部位",
                "术前用药",
                "检查结果"
            ));
            cb_bodyLoc.ItemsSource = result.SplitContent("检查体位");
            cb_anesthesia.ItemsSource = result.SplitContent("麻醉方法");
            cb_preoperative.ItemsSource = result.SplitContent("术前用药");
            cb_insert.ItemsSource = result.SplitContent("插入途径");
            cb_org.ItemsSource = result.SplitContent("组织采取");
            cb_cell.ItemsSource = result.SplitContent("细胞采取");
            cb_hiv.ItemsSource = result.SplitContent("HIV");
            cb_hcv.ItemsSource = result.SplitContent("HCV");
            cb_hbasg.ItemsSource = result.SplitContent("HBasg");
            cb_body.ItemsSource = BodyParts = result.SplitContent("检查部位");
            cb_result.ItemsSource = result.SplitContent("检查结果");
        }

        private void SelectedBody_Click(object sender, RoutedEventArgs e)
        {
            cb_body.Text = string.Empty;
            for (int i = 0; i < cb_body.Items.Count; i++)
            {
                var cbi = cb_body.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                var cb = ControlHelper.GetVisualChild<CheckBox>(cbi);
                if (cb.IsChecked.Value)
                    cb_body.Text += cb.Content.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(cb_body.Text))
                cb_body.Text = cb_body.Text.Substring(0, cb_body.Text.Length - 1);
        }

        private void ExaminationText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && !this.IsReadOnly)
            {
                var expander = ControlHelper.GetParentObject<Expander>(element);
                var title = expander.Header.ToString();
                var medicalWord = MedicalWords.FirstOrDefault(t => t.Name.Equals(title));
                if (title.Equals("内镜所见") || title.Equals("镜下诊断"))
                    ti_template.IsSelected = true;
                else
                {
                    if (medicalWord == null) tv_medicalWord.ItemsSource = null;
                    else tv_medicalWord.ItemsSource = medicalWord.MedicalWords;
                    ti_word.IsSelected = true;
                }
            }
        }

        private void MedicalWord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element)
            {
                if (element.DataContext is MedicalWord word && (word.MedicalWords == null || word.MedicalWords.Count == 0))
                {
                    var rootMedicalWord = CacheHelper.GetMedicalWordParent(this.OriginMedicalWords, word.MedicalWordID, 0);
                    switch (rootMedicalWord.Name)
                    {
                        case "临床诊断":
                            if (string.IsNullOrEmpty(SelectedExamination.ClinicalDiagnosis))
                                SelectedExamination.ClinicalDiagnosis = word.Name;
                            else if (!SelectedExamination.ClinicalDiagnosis.Contains(word.Name))
                                SelectedExamination.ClinicalDiagnosis += "\n" + word.Name;
                            break;
                        case "内镜所见":
                            if (string.IsNullOrEmpty(SelectedExamination.EndoscopicFindings))
                                SelectedExamination.EndoscopicFindings = word.Name;
                            else if (!SelectedExamination.EndoscopicFindings.Contains(word.Name))
                                SelectedExamination.EndoscopicFindings += "\n" + word.Name;
                            break;
                        case "镜下诊断":
                            if (string.IsNullOrEmpty(SelectedExamination.MicroscopicDiagnosis))
                                SelectedExamination.MicroscopicDiagnosis = word.Name;
                            else if (!SelectedExamination.MicroscopicDiagnosis.Contains(word.Name))
                                SelectedExamination.MicroscopicDiagnosis += "\n" + word.Name;
                            break;
                        case "活检部位":
                            if (string.IsNullOrEmpty(SelectedExamination.BiopsySite))
                                SelectedExamination.BiopsySite = word.Name;
                            else if (!SelectedExamination.BiopsySite.Contains(word.Name))
                                SelectedExamination.BiopsySite += "\n" + word.Name;
                            break;
                        case "病理诊断":
                            if (string.IsNullOrEmpty(SelectedExamination.PathologicalDiagnosis))
                                SelectedExamination.PathologicalDiagnosis = word.Name;
                            else if (!SelectedExamination.PathologicalDiagnosis.Contains(word.Name))
                                SelectedExamination.PathologicalDiagnosis += "\n" + word.Name;
                            break;
                        case "医生建议":
                            if (string.IsNullOrEmpty(SelectedExamination.DoctorAdvice))
                                SelectedExamination.DoctorAdvice = word.Name;
                            else if (!SelectedExamination.DoctorAdvice.Contains(word.Name))
                                SelectedExamination.DoctorAdvice += "\n" + word.Name;
                            break;
                    }
                }
            }
        }

        private void MedicalTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element)
            {
                if (element.DataContext is MedicalTemplate template && (!string.IsNullOrEmpty(template.Moddia) || !string.IsNullOrEmpty(template.Modsee)))
                {
                    if (string.IsNullOrEmpty(SelectedExamination.EndoscopicFindings))
                        SelectedExamination.EndoscopicFindings = template.Modsee;
                    else if (!SelectedExamination.EndoscopicFindings.Contains(template.Modsee))
                        SelectedExamination.EndoscopicFindings += "\n" + template.Modsee;
                    if (string.IsNullOrEmpty(SelectedExamination.MicroscopicDiagnosis))
                        SelectedExamination.MicroscopicDiagnosis = template.Moddia;
                    else if (!SelectedExamination.MicroscopicDiagnosis.Contains(template.Moddia))
                        SelectedExamination.MicroscopicDiagnosis += "\n" + template.Moddia;
                }
            }
        }

        private async void Shotcut_Click(object sender, RoutedEventArgs e)
        {
            var image = video.Shotcut();
            if (image != null && image.Length > 0)
            {
                var media = new ExaminationMedia
                {
                    Buffer = image,
                    ExaminationID = SelectedExamination.ExaminationID,
                    MediaType = MediaType.Image,
                };
                SelectedExamination.Images.Add(media);
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
                    else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传数据失败,{ result2.Error }");
                }
                else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传图片失败,{ result.Error }");
            }
        }

        private async void Record_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                if (tb.IsChecked.Value)
                {
                    var image = video.Shotcut();
                    if (image != null && image.Length > 0)
                    {
                        var media = new ExaminationMedia
                        {
                            Buffer = image,
                            ExaminationID = SelectedExamination.ExaminationID,
                            MediaType = MediaType.Video,
                        };
                        SelectedExamination.Videos.Add(media);
                        var result = await SocketProxy.Instance.HttpProxy.UploadFile<string>(image);
                        if (result.IsSuccess)
                        {
                            media.Path = result.Content;
                            var result2 = await SocketProxy.Instance.AddExaminationMedia(media);
                            if (result2.IsSuccess)
                            {
                                media.ExaminationMediaID = result2.Content;
                                media.ErrorMsg = null;
                                var filePath = Path.Combine(CacheHelper.VideoPath, TimeHelper.ToUnixTime(DateTime.Now).ToString() + ".avi");
                                if (video.StartRecord(filePath))
                                    media.LocalVideoPath = filePath;
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        Alert.ShowMessage(true, AlertType.Error, "开启录像失败");
                                        tb.IsChecked = tb.IsChecked.Value;
                                    });
                                }
                            }
                            else this.Dispatcher.Invoke(() =>
                            {
                                media.ErrorMsg = $"上传视频数据失败,{ result2.Error }";
                                tb.IsChecked = tb.IsChecked.Value;
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                media.ErrorMsg = $"上传视频预览图片失败,{ result.Error }";
                                tb.IsChecked = tb.IsChecked.Value;
                            });
                        }
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Error, "预览图获取失败,录像已停止");
                        tb.IsChecked = tb.IsChecked.Value;
                    }
                }
                else
                {
                    if (video.StopRecord())
                    {
                        var media = SelectedExamination.Videos.FirstOrDefault(t => !string.IsNullOrEmpty(t.LocalVideoPath));
                        var result = await SocketProxy.Instance.HttpProxy.UploadFile<string>(media.LocalVideoPath);
                        if (result.IsSuccess)
                        {
                            media.VideoPath = result.Content;
                            var result2 = await SocketProxy.Instance.ModifyExaminationMedia(media);
                            if (!result2.IsSuccess)
                                this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频文件失败,{ result2.Error }");
                            else
                            {
                                File.Delete(media.LocalVideoPath);
                                media.LocalVideoPath = null;
                            }
                        }
                        else this.Dispatcher.Invoke(() => media.ErrorMsg = $"上传视频文件失败,{ result.Error }");
                    }
                    else
                    {
                        Alert.ShowMessage(true, AlertType.Error, "停止录像失败");
                        tb.IsChecked = tb.IsChecked.Value;
                    }
                }
            }
        }
        private void PlayMedia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ExaminationMedia media)
            {
                if (media.MediaType == MediaType.Image)
                {
                    var remoteAddress = SocketProxy.Instance.GetFileRounter() + media.Path;
                    bd_media.Visibility = Visibility.Visible;
                    img_media.Source = new BitmapImage(new Uri(remoteAddress, UriKind.Absolute));
                }
                else
                {
                    var remoteAddress = SocketProxy.Instance.GetFileRounter() + media.VideoPath;
                    video.SetSource(remoteAddress, true);
                }
                bt_close.Visibility = Visibility.Visible;
                bt_close.Tag = media;
            }
        }

        private void CloseMedia_Click(object sender, RoutedEventArgs e)
        {
            if (bt_close.Tag is ExaminationMedia media)
            {
                if (media.MediaType == MediaType.Image)
                {
                    bd_media.Visibility = Visibility.Hidden;
                    img_media.Source = null;
                }
                else if (media.MediaType == MediaType.Video)
                {
                    if (SelectedExamination.Appointment.AppointmentStatus == AppointmentStatus.Checking)
                        video.SetSource(CacheHelper.EndoscopeDeviceID, true);
                    else video.Dispose();
                }
                bt_close.Visibility = Visibility.Hidden;
            }
        }

        private async void BodyPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is FrameworkElement element && element.IsLoaded && element.DataContext is ExaminationMedia media)
            {
                var result = await SocketProxy.Instance.ModifyExaminationMedia(media);
                if (!result.IsSuccess)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Alert.ShowMessage(true, AlertType.Error, $"检查部位设置失败,{ result.Error }");
                        media.BodyPart = null;
                    });
                }
            }
        }

        private async void MediaPrint_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ExaminationMedia media)
            {
                var result = await SocketProxy.Instance.ModifyExaminationMedia(media);
                if (!result.IsSuccess)
                    this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"检查部位设置失败,{ result.Error }"));
            }
        }
    }
}
