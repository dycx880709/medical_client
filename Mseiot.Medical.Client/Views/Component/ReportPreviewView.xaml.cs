using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ReportPreviewView.xaml 的交互逻辑
    /// </summary>
    public partial class ReportPreviewView : System.Windows.Controls.UserControl
    {
        private Examination examination;
        private int appointmentID;

        public ReportPreviewView(int appointmentID)
        {
            InitializeComponent();
            this.appointmentID = appointmentID;
            this.MaxHeight = 1123;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.92;
            this.Width = this.Height / 1123 * 794;
            this.Loaded += ReportPreviewView_Loaded;
        }

        private void ReportPreviewView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ReportPreviewView_Loaded;
            LoadExaminationInfo();
            LoadSystemSetting();
        }

        private void LoadExaminationInfo()
        {
            if (this.appointmentID != 0)
            {
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(this.appointmentID));
                if (result.IsSuccess)
                {
                    this.examination = result.Content;
                    var CollectionView = CollectionViewSource.GetDefaultView(examination.Images ?? new ObservableCollection<ExaminationMedia>());
                    CollectionView.Filter = t => t is ExaminationMedia media && media.IsSelected;
                    gd_content.DataContext = result.Content;
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
                    this.Close();
                }
            }
        }

        private void LoadSystemSetting()
        {
            var result = loading.AsyncWait("获取模板配置中,请稍后", SocketProxy.Instance.GetSystemSetting());
            if (result.IsSuccess) sp_title.DataContext = result.Content;
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"获取模板配置失败,{ result.Error }");
                this.Close();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CacheHelper.Printer))
            {
                var view = new SetPrinterView();
                if (!cw.ShowDialog("打印机设置", view))
                    return;
            }
            var filePath = SaveJpeg(CacheHelper.TempPath);
            var pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = CacheHelper.LocalSetting.Printer;
            var i = Image.FromFile(filePath);
            pd.DefaultPageSettings.PaperSize = new PaperSize("检 查报告单", i.Width, i.Height);
            pd.DefaultPageSettings.Landscape = false;
            pd.PrintPage += (_, ev) =>
            {
                var m = ev.PageBounds;
                ev.Graphics.DrawImage(i, m);
                i.Dispose();
                File.Delete(filePath);
                examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
                var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
                if (result.IsSuccess)
                    Alert.ShowMessage(true, AlertType.Success, "打印完成");
            };
            pd.Print();
            //var printDialog = new System.Windows.Controls.PrintDialog();
            //printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            //printDialog.PrintTicket.PageMediaSize = new PageMediaSize(794, 1123);
            //if (printDialog.ShowDialog().Value)
            //{
            //    printDialog.PrintVisual(this.gb_print, "检查单报告");
            //    examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
            //    var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
            //    if (result.IsSuccess)
            //        Alert.ShowMessage(true, AlertType.Success, "打印完成");
            //}
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveJpeg(dialog.SelectedPath);
                Alert.ShowMessage(true, AlertType.Success, "检查单另存为成功");
            }
        }

        private string SaveJpeg(string folderPath)
        {
            var filePath = Path.Combine(folderPath, appointmentID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg");
            var brush = new VisualBrush(this.gb_print);
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(brush, null, new Rect(0, 0, gb_print.ActualWidth, gb_print.ActualHeight));
                context.Close();
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)gb_print.ActualWidth, (int)gb_print.ActualHeight, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);
            JpegBitmapEncoder encode = new JpegBitmapEncoder();
            encode.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream file = new FileStream(filePath, FileMode.Create))
                encode.Save(file);
            return filePath;
        }

        private void PrinterSetting_Click(object sender, RoutedEventArgs e)
        {
            var view = new SetPrinterView();
            cw.ShowDialog("打印机设置", view);
        }
    }
}
