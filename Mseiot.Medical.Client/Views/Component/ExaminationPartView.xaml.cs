using MM.Medical.Client.Core;
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
        public bool ShowSelected
        {
            get { return (bool)GetValue(ShowSelectedProperty); }
            set { SetValue(ShowSelectedProperty, value); }
        }

        public static readonly DependencyProperty ShowSelectedProperty = DependencyProperty.Register("ShowSelected", typeof(bool), typeof(ExaminationPartView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedExaminationProperty = DependencyProperty.Register("SelectedExamination", typeof(Examination), typeof(ExaminationPartView), new PropertyMetadata(null, SelectedExaminationPropertyChanged));
        public static readonly DependencyProperty LoadingProperty = DependencyProperty.Register("Loading", typeof(Loading), typeof(ExaminationPartView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ExaminationPartView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedMediaProperty = DependencyProperty.Register("SelectedMedia", typeof(ExaminationMedia), typeof(ExaminationPartView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ExaminationPartView), new PropertyMetadata(false));

        public IEnumerable<MedicalWord> OriginMedicalWords { get; set; }
        public IEnumerable<MedicalWord> MedicalWords { get; set; }
        public List<string> BodyParts { get; set; }
        private SystemSetting systemSetting;
        private MediaPlayer player;

        private static void SelectedExaminationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExaminationPartView epv)
            {
                epv.video.Dispose();
                epv.bd_media.Visibility = Visibility.Hidden;
                epv.img_media.Source = null;
                epv.bt_close.Visibility = Visibility.Hidden;
                epv.ShowSelected = false;
            }
        }

        public ExaminationPartView()
        {
            InitializeComponent();
            this.Loaded += ExaminationPartView_Loaded;
        }

        private void ExaminationPartView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ExaminationPartView_Loaded;
            AddSoftKeyboard();
            this.Loading = this.Loading ?? this.cl;
            GetSystemSetting();
            GetBaseWords();
            GetMedicalDatas();
            this.DataContext = this;
        }

        private void AddSoftKeyboard()
        {
            sk.Keys.AddRange(new List<string> { "+", "-", "×", "÷", "/", "±", "＝", "≠", "≈", "＜", "＞", "≤", "≥" });
            sk.Keys.AddRange(new List<string> { "cm", "mm", "m/s", "cm/s", "m²", "cm²", "nm²" });
            sk.Keys.AddRange(new List<string> { "%", "‰" });
            sk.Keys.AddRange(new List<string> { "℃", "°" });
            sk.Keys.AddRange(new List<string> { "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "Ⅺ", "Ⅻ" });
            sk.Keys.AddRange(new List<string> { "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο", "π", "ρ", "σ", "τ", "υ", "φ", "χ", "ψ", "ω" });
            sk.NotifyKey += Sk_NotifyKey;
        }

        private void GetSystemSetting()
        {
            this.player = new MediaPlayer();
            var result = Loading.AsyncWait("获取系统设置中,请稍后", SocketProxy.Instance.GetSystemSetting());
            if (result.IsSuccess && !string.IsNullOrEmpty(result.Content.CutshotSound))
            {
                var soundPath = SocketProxy.Instance.GetFileRounter() + result.Content.CutshotSound;
                player.Open(new Uri(soundPath, UriKind.Absolute));
                var shotcutKey = result.Content.CutshotKeyboard;
                Console.WriteLine($"按键{ shotcutKey }为截图按键");
                Application.Current.MainWindow.KeyDown += (_, e) =>
                {
                    if (this.SelectedExamination != null && 
                    SelectedExamination.Appointment != null && 
                    SelectedExamination.Appointment.AppointmentStatus == AppointmentStatus.Checking &&
                    this.IsLoaded)
                    {
                        if (e.Key.ToString().Equals(shotcutKey))
                            Shotcut();
                        else if (e.Key.ToString().Equals("System"))
                            KeyHookHelper.ResetSystem();
                    }
                };
            }
            else player.Open(new Uri("screenshot.mp3", UriKind.Relative));
            this.systemSetting = result.Content;
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
                "术前用药"
            ));
            if (result.IsSuccess)
            {
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
            }
        }


        private void BodyPart_DropDownOpened(object sender, EventArgs e)
        {
            var selectedItems = new List<string>();
            if (!string.IsNullOrEmpty(SelectedExamination.BodyPart))
                selectedItems = SelectedExamination.BodyPart.Split(',').ToList();
            for (int i = 0; i < cb_body.Items.Count; i++)
            {
                var cbi = cb_body.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                if (cbi != null)
                {
                    var cb = ControlHelper.GetVisualChild<CheckBox>(cbi);
                    cb.IsChecked = selectedItems.Any(t => t.Equals(cb_body.Items[i].ToString()));
                }
            }
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
                if (medicalWord == null) tv_medicalWord.ItemsSource = null;
                else tv_medicalWord.ItemsSource = medicalWord.MedicalWords;
                if (title.Equals("内镜所见") || title.Equals("镜下诊断"))
                    ti_template.IsSelected = true;
                else ti_word.IsSelected = true;
                cgb_word.Visibility = Visibility.Visible;
            }
        }

        private void Sk_NotifyKey(object sender, string e)
        {
            if (Keyboard.FocusedElement is TextBox tb)
                tb.Text += e;
        }

        private void CloseWord_Click(object sender, RoutedEventArgs e)
        {
            cgb_word.Visibility = Visibility.Collapsed;
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

        private void Shotcut_Click(object sender, RoutedEventArgs e)
        {
            Shotcut();
        }

        private async void Record_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                tb.IsEnabled = false;
                if (tb.IsChecked.Value)
                {
                    if (SelectedExamination.Videos.Count >= systemSetting.MediaCount)
                    {
                        Alert.ShowMessage(true, AlertType.Warning, "采集视频数量超过上限");
                        tb.IsChecked = false;
                        tb.IsEnabled = true;
                        return;
                    }
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
                this.Dispatcher.Invoke(() => tb.IsEnabled = true);
            }
        }
        private void PlayMedia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ExaminationMedia media)
            {

                if (media.MediaType == MediaType.Image)
                {
                    video.Dispose();
                    var remoteAddress = SocketProxy.Instance.GetFileRounter() + media.Path;
                    bd_media.Visibility = Visibility.Visible;
                    img_media.Source = new BitmapImage(new Uri(remoteAddress, UriKind.Absolute));
                }
                else
                {
                    bd_media.Visibility = Visibility.Hidden;
                    img_media.Source = null;
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
                if (SelectedExamination.Appointment.AppointmentStatus == AppointmentStatus.Checking)
                    video.SetSource(CacheHelper.EndoscopeDeviceID, true);
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
                if (media.IsSelected)
                {
                    if (SelectedExamination.Images.Count(t => t.IsSelected) > systemSetting.PrintImageCount)
                    {
                        Alert.ShowMessage(true, AlertType.Warning, "报告图片数量超过上限");
                        media.IsSelected = false;
                        return;
                    }
                }
                var result = await SocketProxy.Instance.ModifyExaminationMedia(media);
                if (!result.IsSuccess)
                {
                    this.Dispatcher.Invoke(() => Alert.ShowMessage(true, AlertType.Error, $"设置是否打印失败,{ result.Error }"));
                    media.IsSelected = !media.IsSelected;
                }
            }
        }

        private async void Shotcut()
        {
            if (SelectedExamination.Images.Count >= systemSetting.CutshotImageCount)
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
                player.Position = TimeSpan.Zero;
            }
        }

        private void RemoveMedia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ExaminationMedia media)
            {
                var name = media.MediaType == MediaType.Image ? "图片" : "视频";
                var result = Loading.AsyncWait($"删除{ name }中,请稍后", SocketProxy.Instance.RemoveExaminationMedia(media.ExaminationMediaID));
                if (result.IsSuccess)
                {
                    if (media.MediaType == MediaType.Image)
                        SelectedExamination.Images.Remove(media);
                    else if (media.MediaType == MediaType.Video)
                        SelectedExamination.Videos.Remove(media);
                }
                else Alert.ShowMessage(true, AlertType.Error, $"删除{ name }失败,{ result.Error }");
            }
        }

        private void ExportMedia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ExaminationMedia media)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (media.MediaType == MediaType.Video && !string.IsNullOrEmpty(media.VideoPath))
                    {
                        var localVideoPath = Path.Combine(dialog.SelectedPath, $"{media.ExaminationMediaID}_{ TimeHelper.ToUnixTime(DateTime.Now) }.mp4");
                        var videoPath = "files/" + media.VideoPath;
                        var result = Loading.AsyncWait("导出视频文件中,请稍后", SocketProxy.Instance.HttpProxy.DownloadFile(videoPath, localVideoPath));
                        if (!result.IsSuccess)
                        {
                            Alert.ShowMessage(true, AlertType.Error, $"导出视频文件失败,{ result.Error }");
                            return;
                        }
                        Alert.ShowMessage(true, AlertType.Success, $"视频导出成功");
                    }
                    else if (!string.IsNullOrEmpty(media.Path)) 
                    {
                        var localVideoPath = Path.Combine(dialog.SelectedPath, $"{media.ExaminationMediaID}_{ TimeHelper.ToUnixTime(DateTime.Now) }.jpg");
                        var imagePath = "files/" + media.Path;
                        var result = Loading.AsyncWait("导出图片文件中,请稍后", SocketProxy.Instance.HttpProxy.DownloadFile(imagePath, localVideoPath));
                        if (!result.IsSuccess)
                        { 
                            Alert.ShowMessage(true, AlertType.Error, $"导出图片文件失败,{ result.Error }");
                            return;
                        }
                        Alert.ShowMessage(true, AlertType.Success, $"图片导出成功");
                    }
                }
            }
        }

        private void ShowSelect_Click(object sender, RoutedEventArgs e)
        {
            var collection = CollectionViewSource.GetDefaultView(SelectedExamination.Images);
            collection.Filter = null;
            if (this.ShowSelected)
                collection.Filter = t => t is ExaminationMedia media && media.IsSelected;
            collection.Refresh();
        }
    }
}
