using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
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
            //var filePath = SaveJpeg(CacheHelper.TempPath);
            //var pd = new PrintDocument();
            //pd.DefaultPageSettings.PaperSize = new PaperSize();
            //pd.PrintPage += (s, args) =>
            //{
            //    var i = System.Drawing.Image.FromFile(filePath);
            //    var m = args.PageBounds;
            //    if (i.Width < i.Height)
            //        i.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //    if (i.Width >= i.Height)
            //    {
            //        if ((double)i.Width / (double)i.Height <= (double)m.Width / (double)m.Height)
            //        {
            //            int w = (int)((double)i.Width / (double)i.Height * (double)m.Height);
            //            int dx = (m.Width - w) / 2;
            //            m.X = dx;
            //            m.Y = 0;
            //            m.Width = w;
            //        }
            //        else
            //        {
            //            int h = (int)((double)i.Height / (double)i.Width * (double)m.Width);
            //            int dy = (m.Height - h) / 2;
            //            m.X = 0;
            //            m.Y = dy;
            //            m.Height = h;
            //        }
            //    }
            //    args.Graphics.DrawImage(i, m);
            //};
            //pd.PrintController = new StandardPrintController();
            //pd.Print();
            //File.Delete(filePath);
            //examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
            //var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
            //if (result.IsSuccess)
            //    Alert.ShowMessage(true, AlertType.Success, "打印完成");

            var printDialog = new System.Windows.Controls.PrintDialog();
            printDialog.PrintTicket.PageBorderless = System.Printing.PageBorderless.None;
            if (printDialog.ShowDialog().Value)
                printDialog.PrintVisual(this.gb_print, "检查单报告");
            examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
            var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
            if (result.IsSuccess)
                Alert.ShowMessage(true, AlertType.Success, "打印完成");
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
    }
}
